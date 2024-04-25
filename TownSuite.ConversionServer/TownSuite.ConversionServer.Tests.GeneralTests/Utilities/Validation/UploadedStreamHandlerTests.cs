using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Models.Errors;
using TownSuite.ConversionServer.Common.Validation;

namespace TownSuite.ConversionServer.Tests.GeneralTests.Utilities.Validation
{
    [TestFixture]
    public class UploadedStreamHandlerTests
    {
        [Test]
        public void Does_ensure_stream_has_data()
        {
            using var stream = new MemoryStream([1]);

            var streamValidator = new UploadedStreamHandler(stream);
            var test = () => streamValidator.EnsureStreamHasData();

            Assert.That(test, Throws.Nothing);
        }

        [Test]
        public void Does_throw_error_when_no_data_in_stream()
        {
            using var stream = new MemoryStream([]);

            var streamValidator = new UploadedStreamHandler(stream);
            var test = () => streamValidator.EnsureStreamHasData();

            Assert.That(test, Throws.InstanceOf<ValidationException>());
        }

        [Test]
        public void Does_not_throw_error_when_no_data_in_stream_and_seek_unavailable()
        {
            using var stream = new RestrictedStream([]);

            var streamValidator = new UploadedStreamHandler(stream);
            var test = () => streamValidator.EnsureStreamHasData();

            Assert.That(test, Throws.Nothing);
        }

        [Test]
        public async Task Does_copy_stream_data()
        {
            using var streamToCopyFrom = new RestrictedStream([0x1]);
            using var streamToCopyTo = new MemoryStream();

            var streamValidator = new UploadedStreamHandler(streamToCopyFrom);
            await streamValidator.CopyToAsync(streamToCopyTo);

            Assert.That(streamToCopyTo.Length, Is.EqualTo(1));
            Assert.That(streamToCopyTo.ReadByte(), Is.EqualTo(0x1));
        }
    }
}
