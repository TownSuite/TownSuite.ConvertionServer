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
    public class UploadedStreamValidatorTests
    {
        [Test]
        public void Does_throw_error_when_no_data_in_stream()
        {
            var stream = new RestrictedStream(new byte[0]);

            var streamValidator = new UploadedStreamValidator(stream);
            var test = () => streamValidator.EnsureStreamHasData();

            Assert.That(test, Throws.InstanceOf<ValidationException>());
        }
    }
}
