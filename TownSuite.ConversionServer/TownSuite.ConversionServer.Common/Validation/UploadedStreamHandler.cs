using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Models.Errors;

namespace TownSuite.ConversionServer.Common.Validation
{
    public class UploadedStreamHandler
    {
        private readonly Stream _stream;

        public UploadedStreamHandler(Stream stream)
        {
            _stream = stream;
        }

        public async Task CopyToAsync(Stream streamToCopyTo)
        {
            await _stream.CopyToAsync(streamToCopyTo);
            streamToCopyTo.Seek(0, SeekOrigin.Begin);
        }

        public void EnsureStreamHasData()
        {
            if (_stream.CanSeek == true && _stream.Length == 0)
            {
                throw new ValidationException("The file given is empty.");
            }
        }
    }
}
