using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Models.Errors;
using TownSuite.ConversionServer.Interfaces.Common.Bytes;

namespace TownSuite.ConversionServer.Common.Validation
{
    public class UploadedStreamHandler
    {
        private readonly Stream _stream;
        private const int MAX_BUFFER_SIZE = 80000;

        public UploadedStreamHandler(Stream stream)
        {
            _stream = stream;
        }

        public async Task CopyToAsync(Stream streamToCopyTo, IByteCount maxBytes, CancellationToken token)
        {
            var totalBytesRead = 0L;
            var lastRead = -1;
            while (lastRead != 0 && maxBytes.Bytes > totalBytesRead) 
            {
                var buffer_length = (int)Math.Min(MAX_BUFFER_SIZE, maxBytes.Bytes - totalBytesRead);
                var buffer = new byte[buffer_length];
                lastRead = await _stream.ReadAsync(buffer, 0, buffer_length, token);
                await streamToCopyTo.WriteAsync(buffer, 0, lastRead, token);
                totalBytesRead += lastRead;
                token.ThrowIfCancellationRequested();
            }

            if (_stream.ReadByte() != -1)
            {
                throw new ValidationException($"PDF SIZE TOO LARGE. Greater than {maxBytes.Megabytes} megabytes.");
            }
            streamToCopyTo.Seek(0, SeekOrigin.Begin);
            EnsureStreamHasData(streamToCopyTo);
        }

        private void EnsureStreamHasData(Stream stream)
        {
            if (stream.Length == 0)
            {
                throw new ValidationException("The file given is empty.");
            }
        }
    }
}
