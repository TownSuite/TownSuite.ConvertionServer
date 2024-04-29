using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Bytes;
using TownSuite.ConversionServer.Common.Models.Errors;
using TownSuite.ConversionServer.Common.Validation;
using TownSuite.ConversionServer.Interfaces.Common.Bytes;

namespace TownSuite.ConversionServer.Tests.GeneralTests.Common.Validation
{
    [TestFixture]
    public class UploadedStreamHandlerTests
    {
        private IByteCount _defaultByteCount;
        private Stream _streamToCopyTo;

        [SetUp]
        public void SetUp()
        {
            _defaultByteCount = new ByteCount()
            {
                Megabytes = 10
            };
            _streamToCopyTo = new MemoryStream();
        }

        [TearDown]
        public void TearDown()
        {
            _streamToCopyTo.Dispose();
        }

        [Test]
        public void Does_ensure_stream_has_data()
        {
            using var streamToCopyFrom = new MemoryStream([1]);

            var streamValidator = new UploadedStreamHandler(streamToCopyFrom);
            var test = () => streamValidator.CopyToAsync(_streamToCopyTo, _defaultByteCount, default);

            Assert.That(test, Throws.Nothing);
        }

        [Test]
        public void Does_throw_error_when_no_data_in_stream()
        {
            using var streamToCopyFrom = new MemoryStream([]);

            var streamValidator = new UploadedStreamHandler(streamToCopyFrom);
            var test = () => streamValidator.CopyToAsync(_streamToCopyTo, _defaultByteCount, default);

            Assert.That(test, Throws.InstanceOf<ValidationException>());
        }

        [Test]
        public void Does_throw_error_when_stream_is_too_large()
        {
            using var streamToCopyFrom = new RestrictedStream([0x1, 0x2, 0x3]);
            var maxBytes = new ByteCount()
            {
                Bytes = 2
            };

            var streamValidator = new UploadedStreamHandler(streamToCopyFrom);
            var test = () => streamValidator.CopyToAsync(_streamToCopyTo, maxBytes, default);

            Assert.That(test, Throws.InstanceOf<ValidationException>());
        }

        [Test]
        public async Task Does_copy_stream_data()
        {
            using var streamToCopyFrom = new RestrictedStream([0x1]);

            var streamValidator = new UploadedStreamHandler(streamToCopyFrom);
            await streamValidator.CopyToAsync(_streamToCopyTo, _defaultByteCount, default);

            Assert.That(_streamToCopyTo.Length, Is.EqualTo(1));
            Assert.That(_streamToCopyTo.ReadByte(), Is.EqualTo(0x1));
        }
    }
}
