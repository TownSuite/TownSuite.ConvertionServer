using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Models.Errors;
using TownSuite.ConversionServer.Interfaces.Utilities.Logging;

namespace TownSuite.ConversionServer.Utilities.TownSuiteLogging
{
    public class SimpleLogger: ISimpleLogger
    {
        public const int DEFAULT_ERROR_LEVEL = ISimpleLogger.DEFAULT_ERROR_LEVEL;

        IModelLogger<LoggingErrorModel> _errorClient;
        IOptions<Common.Models.ApplicationSettings> _applicationInformation;
        public SimpleLogger(IModelLogger<LoggingErrorModel> errorClient,
            IOptions<Common.Models.ApplicationSettings> applicationInformation)
        {
            _errorClient = errorClient;
            _applicationInformation = applicationInformation;
        }

        public async Task LogError(Exception ex)
        {
            await LogError(ex, DEFAULT_ERROR_LEVEL);
        }

        public async Task LogError(Exception ex, int level)
        {
            await LogError(ex?.Message, ex?.ToString(), level);
        }

        public async Task LogError(Exception ex, string messageOverride, int level = DEFAULT_ERROR_LEVEL)
        {
            await LogError(messageOverride, ex?.ToString(), level);
        }

        public async Task LogError(string message, string details, int level = DEFAULT_ERROR_LEVEL)
        {
            var client = _errorClient;
            
            await client.PostAsync(GetErrorModel(message, details, level));
        }

        private LoggingErrorModel GetErrorModel(string message, string exception, int level = DEFAULT_ERROR_LEVEL)
        {
            return new LoggingErrorModel()
            {
                TimeStamp = DateTime.UtcNow,
                AptType = _applicationInformation.Value.Name,
                Exception = exception,
                Level = level,
                MachineName = System.Environment.MachineName,
                Message = message,
                MessageTemplate = string.Empty,
                Properties = string.Empty,
                LicenseKey = string.Empty
            };
        }
    }
}
