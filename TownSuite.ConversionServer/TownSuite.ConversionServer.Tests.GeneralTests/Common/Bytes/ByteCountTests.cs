using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Bytes;

namespace TownSuite.ConversionServer.Tests.GeneralTests.Common.Bytes
{
    [TestFixture]
    public class ByteCountTests
    {
        [Test]
        public void Does_calculate_megabytes_from_given_gigabytes()
        {
            var gigaBytes = 2;
            var byteCount = new ByteCount();

            byteCount.Gigabytes = gigaBytes;

            Assert.That(byteCount.Megabytes, Is.EqualTo(gigaBytes * 1000));
        }
    }
}
