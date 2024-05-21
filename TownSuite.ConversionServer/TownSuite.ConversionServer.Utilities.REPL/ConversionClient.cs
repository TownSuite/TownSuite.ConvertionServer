using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using TownSuite.ConversionServer.Common.Models.Services;

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
            ValidateSettings();

            using var client = new HttpClient();
            using HttpRequestMessage request = GetRequest("/PdfConverter/StreamToPng");
            request.Headers.Authorization = GetAuthHeader(request);

            using var content = new StreamContent(body);
            request.Content = content;

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            await withPng(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.MediaType ?? string.Empty);
        }

        public async Task<ItemResponseModel<IEnumerable<byte[]>>?> ConvertPdfAsBytesAsync(byte[] file)
        {
            ValidateSettings();

            using var client = new HttpClient();
            using HttpRequestMessage request = GetRequest("/PdfConverter");
            request.Headers.Authorization = GetAuthHeader(request);

            var body = new ItemRequestModel<byte[]>()
            {
                Data = file,
            };
            using var content = JsonContent.Create(body);
            request.Content = content;

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ItemResponseModel<IEnumerable<byte[]>>>(result);
        }

        private void ValidateSettings()
        {
            if (_settings.ApiUrl is null || _settings.Username is null || _settings.Password is null)
            {
                throw new Exception("Missing settings. The ApiUrl, Username and Password are required.");
            }
        }

        private HttpRequestMessage GetRequest(string path)
        {
            var url = _settings.ApiUrl?.TrimEnd('/') + path;
            return new HttpRequestMessage(HttpMethod.Post, url);
        }

        private AuthenticationHeaderValue GetAuthHeader(HttpRequestMessage request)
        {
            var credentials = $"{_settings.Username}:{_settings.Password}";
            var basicAuth = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(credentials));
            return new AuthenticationHeaderValue("Basic", basicAuth);
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
