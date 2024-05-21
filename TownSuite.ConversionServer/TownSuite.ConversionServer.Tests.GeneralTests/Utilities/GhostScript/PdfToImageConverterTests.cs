using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TownSuite.ConversionServer.StandardServices;
using TownSuite.ConversionServer.Interfaces.Utilities.Converters;
using TownSuite.ConversionServer.Common.Validation;

namespace TownSuite.ConversionServer.Tests.GeneralTests.Utilities.GhostScript
{
    [TestFixture]
    class PdfToImageConverterTests
    {
        private IPdfToImageBytesConverter _converter;

        [SetUp]
        public void InitConfiguration()
        {
            var dependencyInjection = new DefaultDependencyInjection();
            _converter = dependencyInjection.ServiceProvider.GetService<IPdfToImageBytesConverter>();
        }
        
        [Test]
        public async Task ConvertTest()
        {
            var singlePage = System.IO.Path.Combine(GetAssetsDirectory(), "single_page_test.pdf");
            Assert.That(System.IO.File.Exists(singlePage), Is.True);
            var pageBytes = await System.IO.File.ReadAllBytesAsync(singlePage);

            var res = await _converter.Convert(pageBytes);

            int resCount = 0;
            foreach (var image in res)
            {
                Assert.That(image.Length, Is.GreaterThan(0));
                resCount++;
            }
            Assert.That(resCount, Is.EqualTo(1));
            
        }

        [Test]
        public async Task ConvertMultiPageTest()
        {
            var multiPage = System.IO.Path.Combine(GetAssetsDirectory(), "multi_page_pdf.pdf");
            Assert.That(System.IO.File.Exists(multiPage), Is.True);
            var pageBytes = await System.IO.File.ReadAllBytesAsync(multiPage);

            var res = await _converter.Convert(pageBytes);

            int resCount = 0;
            foreach (var image in res)
            {
                Assert.That(image.Length, Is.GreaterThan(0), "Image cannot be zero bytes");
                resCount++;
            }
            Assert.That(resCount, Is.GreaterThan(1), "Multipage test requires png file count greater than 1.");
        }

        [Test]
        public async Task ConvertStreamTest()
        {
            var singlePage = System.IO.Path.Combine(GetAssetsDirectory(), "single_page_test.pdf");
            using var pageStream = System.IO.File.OpenRead(singlePage);
            var streamHandler = new UploadedStreamHandler(pageStream);

            var results = await _converter.Convert(streamHandler);

            Assert.That(results.File.Length, Is.GreaterThan(0), "Image cannot be zero bytes");
            Assert.That(results.MediaType, Is.EqualTo("image/png"), "Expected a png image.");
        }

        private string GetAssetsDirectory()
        {
            string exeLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var dir = System.IO.Path.Combine(exeLocation, "Utilities", "GhostScript", "Assets");
            Assert.That(System.IO.Directory.Exists(dir), Is.True, "Testing assets directory missing. Ensure in same directory as executable.");
            return dir;
        }
    }
}
