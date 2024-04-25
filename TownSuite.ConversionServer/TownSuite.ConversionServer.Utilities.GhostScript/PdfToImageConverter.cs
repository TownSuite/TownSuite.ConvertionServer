using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Models;
using TownSuite.ConversionServer.Common.Models.Conversions;
using TownSuite.ConversionServer.Interfaces.Common.Bytes;
using TownSuite.ConversionServer.Interfaces.Common.Validation;
using TownSuite.ConversionServer.Interfaces.Utilities.Converters;

namespace TownSuite.ConversionServer.Utilities.GhostScript
{
    public class PdfToImageConverter : IPdfToImageBytesConverter
    {
        public const long DEFAULT_MAX_GIGABYTE_SIZE = 10;
        public const int DEFAULT_MAX_JOB_DURATION_MINUTES = 30;
        public IByteCount MaxBytes;

        public TimeSpan MaxJobDuration { get; set; }

        public string ExecutableLocation { get; set; }

        public PdfToImageConverter(IOptions<GhostScriptSettings> settings, IByteCountFactory byteCount)
        {
            MaxBytes = byteCount.FromGigabytes(settings.Value.MaxGigabytesUpload);
            MaxJobDuration = TimeSpan.FromMinutes(settings.Value.MaxJobDurationMinutes);
            ExecutableLocation = settings.Value.ExecutableLocation;
        }

        public async Task<IEnumerable<byte[]>> Convert(byte[] pdf, CancellationToken cancellationToken = default)
        {
            if (pdf.Length <= 0)
            {
                return new List<byte[]>();
            }
            else if (pdf.Length > MaxBytes.Bytes)
            {
                throw new Exception($"PDF SIZE TOO LARGE. Greater than {MaxBytes.Megabytes} megabytes.");
            }

            var fullCancelToken = mergeTokenWithDefaultDuration(cancellationToken);

            string pdfPath = CreateTempPdfPath();

            await System.IO.File.WriteAllBytesAsync(pdfPath, pdf, fullCancelToken.Token);

            IEnumerable<string> pngPaths = await Convert(pdfPath, fullCancelToken.Token);

            var pngBytes = await GetBytesFromPaths(pngPaths, fullCancelToken.Token);

            CleanFiles(pngPaths, pdfPath);

            return pngBytes;
        }

        public async Task<StreamFileResults> Convert(IUploadedStreamHandler streamHandler, CancellationToken cancellationToken = default)
        {
            var fullCancelToken = mergeTokenWithDefaultDuration(cancellationToken);

            var pdfPath = CreateTempPdfPath();

            using (var fileStream = System.IO.File.OpenWrite(pdfPath))
            {
                await streamHandler.CopyToAsync(fileStream, MaxBytes, fullCancelToken.Token);
            }

            var pngPaths = await Convert(pdfPath, fullCancelToken.Token);

            var results = await GetStreamFromPathsAsync(pngPaths);

            CleanFiles(pngPaths, pdfPath);

            return results;
        }

        #region "Private Methods"
        private async Task<IEnumerable<byte[]>> GetBytesFromPaths(IEnumerable<string> filePaths, CancellationToken cancellationToken)
        {
            List<byte[]> bytes = new List<byte[]>();

            foreach (string path in filePaths)
            {
                bytes.Add(await System.IO.File.ReadAllBytesAsync(path, cancellationToken));
            }

            return bytes;
        }

        private async Task<StreamFileResults> GetStreamFromPathsAsync(IEnumerable<string> filePaths)
        {
            var fileCount = filePaths.Count();
            if (fileCount == 1)
            {
                var stream = System.IO.File.OpenRead(filePaths.First());
                return new StreamFileResults(stream, "image/png");
            }
            else
            {
                var stream = await GetZipFileAsync(filePaths);
                stream.Seek(0, SeekOrigin.Begin);
                return new StreamFileResults(stream, MediaTypeNames.Application.Zip);
            }
        }

        private async Task<Stream> GetZipFileAsync(IEnumerable<string> filePaths)
        {
            var stream = new MemoryStream();
            using var zip = new ZipArchive(stream, ZipArchiveMode.Create, true);

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);
                var entry = zip.CreateEntry(fileName);

                using var fileStream = System.IO.File.OpenRead(filePath);
                using var zipStream = entry.Open();
                await fileStream.CopyToAsync(zipStream);
            }
            return stream;
        }

        private string CreateTempPdfPath()
        {
            string tempPath = System.IO.Path.GetTempFileName();
            string pdfPath = tempPath + ".pdf";
            System.IO.File.Move(tempPath, pdfPath);
            return pdfPath;
        }

        private CancellationTokenSource mergeTokenWithDefaultDuration(CancellationToken cancellationToken)
        {
            CancellationTokenSource jobDurationToken = new CancellationTokenSource(MaxJobDuration);
            return CancellationTokenSource.CreateLinkedTokenSource(jobDurationToken.Token, cancellationToken);
        }

        private async Task<IEnumerable<string>> Convert(string filePath, CancellationToken cancellationToken = default)
        {
            var resultUnformatted = GetUnformattedResultString(filePath);
            string intArgumentFormatThreePlaces = "%03d";
            var process = CreateProcess(resultUnformatted, filePath, intArgumentFormatThreePlaces);
            process.Start();

            try
            {
                string errorMessage = await GetMessagesFromStream(process.StandardError, cancellationToken);

                await WaitForProcessExit(process, cancellationToken);

                bool isApprovedExitCode = process.ExitCode == 0;
                if (isApprovedExitCode)
                {
                    string intFormatThreePlaces = "D3";
                    return GetExistingFileNamesFromIncrementalName(resultUnformatted, intFormatThreePlaces);
                }
                else
                {
                    throw new Exception($"Ghostscript failed with {process.ExitCode}. Message:{System.Environment.NewLine}{errorMessage}");
                }
            }
            finally
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                finally
                {
                    process.Close();
                }
            }
        }

        private List<string> GetExistingFileNamesFromIncrementalName(string resultUnformatted, string integerStringFormat)
        {
            List<string> files = new List<string>();
            int i = 1;
            for (string potentialFile = string.Format(resultUnformatted, i.ToString(integerStringFormat)); System.IO.File.Exists(potentialFile) & i < 1000; potentialFile = string.Format(resultUnformatted, i.ToString(integerStringFormat)))
            {
                files.Add(potentialFile);
                i++;
            }

            return files;
        }

        private async Task WaitForProcessExit(Process process, CancellationToken cancellationToken)
        {
            while (!process.HasExited)
            {
                await Task.Delay(200);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async Task<string> GetMessagesFromStream(System.IO.StreamReader stream, CancellationToken cancellationToken)
        {
            StringBuilder message = new StringBuilder();
            while (!stream.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();
                message.Append(await stream.ReadLineAsync());
            }
            return message.ToString();
        }

        private string GetUnformattedResultString(string filePath)
        {
            var fileInfo = new System.IO.FileInfo(filePath);
            var fileExtensionlessName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            return System.IO.Path.Combine(fileInfo.DirectoryName, fileExtensionlessName + "-{0}.png");
        }

        private Process CreateProcess(string resultUnformatted, string filePath, string intProcessArgumentFormat)
        {
            var resultPath = string.Format(resultUnformatted, intProcessArgumentFormat);

            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = ExecutableLocation,
                    Arguments = $"-sDEVICE=pngalpha -o \"{resultPath}\" -r144 \"{filePath}\" -dNOPAUSE -dBATCH",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
        }

        private void CleanFiles(IEnumerable<string> pngFiles = null, string pdfTempFile = null)
        {
            if (pngFiles != null)
            {
                foreach (var file in pngFiles)
                {
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            if (pdfTempFile != null && System.IO.File.Exists(pdfTempFile))
            {
                System.IO.File.Delete(pdfTempFile);
            }
        }
        #endregion
    }
}
