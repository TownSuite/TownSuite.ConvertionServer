using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TownSuite.ConversionServer.Utilities.REPL
{
    public class ConversionClient
    {
        private readonly Settings _settings;

        public ConversionClient()
        {
            _settings = new Settings();
            PopulateSettings("appsettings.json");
            PopulateSettings("appsettings.Development.json");
        }

        public async Task ConvertPdfAsync(Stream body, Func<Stream, string, Task> withPng)
        {
            using var client = new HttpClient();
            var url = _settings.ApiUrl.TrimEnd('/') + "/PdfConverter/StreamToPng";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            var credentials = $"{_settings.Username}:{_settings.Password}";
            var basicAuth = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(credentials));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basicAuth);

            using var content = new StreamContent(body);
            request.Content = content;
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            await withPng(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.MediaType ?? string.Empty);
        }

        private void PopulateSettings(string configFile)
        {
            if (File.Exists(configFile))
            {
                var jsonOverwrite = File.ReadAllText("appsettings.json");
                JsonConvert.PopulateObject(jsonOverwrite, _settings);
            }
        }
    }
}
