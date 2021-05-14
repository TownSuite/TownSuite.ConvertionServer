using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Models.Errors;
using TownSuite.ConversionServer.Interfaces.Utilities.Logging;
using TownSuite.ConversionServer.Interfaces.Utilities.Serializers;

namespace TownSuite.ConversionServer.Utilities.TownSuiteLogging
{
    /// <summary>
    /// Sends logging information based on the <see cref="LoggingErrorModel"/> class to a server.
    /// </summary>
    public class ErrorClient: IModelLogger<LoggingErrorModel>
    {
        private IJsonSerializer _jsonSerializer;
        private IHttpClientFactory _httpClient;
        private IOptions<LoggingServerSettings> _config;

        public ErrorClient(IJsonSerializer jsonSerializer, IHttpClientFactory httpClient, IOptions<LoggingServerSettings> config)
        {
            _jsonSerializer = jsonSerializer;
            _httpClient = httpClient;
            _config = config;
        }

        /// <summary>
        /// Send an error log to the server asynchronously.
        /// </summary>
        /// <remarks>
        /// log.Id will not be used by TownSuite.Logging. Instead,
        /// it will generate an Id for the rest of the given data.
        /// </remarks>
        /// <param name="log">Information about the error.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        public async Task PostAsync(LoggingErrorModel log, System.Threading.CancellationToken cancellationToken = default)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(_config.Value.BaseUrl != null ? _config.Value.BaseUrl.TrimEnd('/') : "").Append(_config.Value.SitePath);

            var client_ = _httpClient.CreateClient();
            try
            {
                using (var request_ = new HttpRequestMessage())
                {
                    var content_ = new StringContent(_jsonSerializer.Serialize(log));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new HttpMethod("POST");

                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new Uri(url_, UriKind.RelativeOrAbsolute);

                    var response_ = await client_.SendAsync(request_, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "200")
                        {
                            return;
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new Exception("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").");
                        }
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (client_ != null)
                    client_.Dispose();
            }
        }
    }
}
