using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownSuite.ConversionServer.Tests.GeneralTests.Common.Validation
{
    /// <summary>
    /// Stream's given over networks may not allow Length and other methods. 
    /// This class prevents cheating in automated tests, since some properties 
    ///   in the live environment would throw an exception when used.
    /// </summary>
    public class RestrictedStream : Stream, IDisposable
    {
        private readonly Stream _stream;

        public RestrictedStream(byte[] data)
        {
            _stream = new MemoryStream(data);
        }

        public new void Dispose()
        {
            _stream.Dispose();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
