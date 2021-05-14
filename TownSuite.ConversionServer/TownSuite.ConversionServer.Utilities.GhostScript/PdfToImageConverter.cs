using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Interfaces.Utilities.Converters;

namespace TownSuite.ConversionServer.Utilities.GhostScript
{
    public class PdfToImageConverter: IPdfToImageBytesConverter
    {
        public const long DEFAULT_MAX_GIGABYTE_SIZE = 10;
        public const int DEFAULT_MAX_JOB_DURATION_MINUTES = 30;
        public long MaxBytes { get; set; }
        public decimal MaxKilobytes
        {
            get => MaxBytes / 1000M;
            set => MaxBytes = (long)Math.Floor(value * 1000M);
        }
        public decimal MaxMegabytes
        {
            get => MaxKilobytes / 1000M;
            set => MaxKilobytes = value * 1000M;
        }
        public decimal MaxGigabytes
        {
            get => MaxMegabytes / 1000M;
            set => MaxKilobytes = value * 1000M;
        }

        public TimeSpan MaxJobDuration;

        public PdfToImageConverter()
        {
            MaxGigabytes = DEFAULT_MAX_GIGABYTE_SIZE;
            MaxJobDuration = TimeSpan.FromMinutes(DEFAULT_MAX_JOB_DURATION_MINUTES);
        }

        public async Task<IEnumerable<byte[]>> Convert(byte[] pdf, CancellationToken cancellationToken = default)
        {
            if (pdf.Length <= 0)
            {
                return new List<byte[]>();
            }
            else if (pdf.Length > MaxBytes)
            {
                throw new Exception($"PDF SIZE TOO LARGE. Greater than {MaxMegabytes} megabytes.");
            }

            CancellationTokenSource jobDurationToken = new CancellationTokenSource(MaxJobDuration);
            var fullCancelToken = CancellationTokenSource.CreateLinkedTokenSource(jobDurationToken.Token, cancellationToken);

            string tempPath = System.IO.Path.GetTempFileName();
            string pdfPath = tempPath + ".pdf";
            System.IO.File.Move(tempPath, pdfPath);

            await System.IO.File.WriteAllBytesAsync(pdfPath, pdf, fullCancelToken.Token);
            IEnumerable<string> pngPaths = await Convert(pdfPath, fullCancelToken.Token);

            List<byte[]> pngBytes = new List<byte[]>();
            foreach (string path in pngPaths)
            {
                pngBytes.Add(await System.IO.File.ReadAllBytesAsync(path, fullCancelToken.Token));
            }

            CleanFiles(pngPaths, pdfPath);

            return pngBytes;
        }

        private async Task<IEnumerable<string>> Convert(string filePath, CancellationToken cancellationToken = default)
        {
            var fileInfo = new System.IO.FileInfo(filePath);
            var fileExtensionlessName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            var resultUnformatted = System.IO.Path.Combine(fileInfo.DirectoryName, fileExtensionlessName + "-{0}.png");
            var resultPath = string.Format(resultUnformatted, "%03d");
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = @"C:\Program Files (x86)\gs\gs9.54.0\bin\gswin32c.exe",
                    Arguments = $"-sDEVICE=pngalpha -o \"{resultPath}\" -r144 \"{filePath}\" -dNOPAUSE -dBATCH",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            try
            {
                StringBuilder errorMessage = new StringBuilder();
                while (!process.StandardError.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    errorMessage.Append(await process.StandardError.ReadLineAsync());
                }

                while (!process.HasExited)
                {
                    await Task.Delay(200);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (process.ExitCode == 0)
                {
                    List<string> files = new List<string>();
                    int i = 1;
                    for (string potentialFile = string.Format(resultUnformatted, i.ToString("D3")); System.IO.File.Exists(potentialFile) & i < 1000; potentialFile = string.Format(resultUnformatted, i.ToString("D3")))
                    {
                        files.Add(potentialFile);
                        i++;
                    }

                    return files;
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
    }
}
