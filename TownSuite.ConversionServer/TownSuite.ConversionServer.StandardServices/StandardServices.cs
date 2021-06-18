using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.StandardServices
{
    public class StandardServices
    {
        public void AddStandardServices(IServiceCollection services)
        {
            services.AddTransient<Interfaces.Common.Errors.IResponseErrorModelFactory<Common.Models.Errors.ResponseErrorModel>, Common.Errors.ResponseErrorModelFactory>();
            services.AddTransient<Interfaces.Common.Bytes.IByteCountFactory, Common.Bytes.ByteCountFactory>();

            services.AddTransient<Interfaces.Utilities.Converters.IPdfToImageBytesConverter, Utilities.GhostScript.PdfToImageConverter>();
            services.AddTransient<Interfaces.Utilities.Logging.ISimpleLogger, Utilities.TownSuiteLogging.SimpleLogger>();
            services.AddTransient<Interfaces.Utilities.Logging.IModelLogger<Common.Models.Errors.LoggingErrorModel>, Utilities.TownSuiteLogging.ErrorClient>();
            services.AddTransient<Interfaces.Utilities.Serializers.IJsonSerializer, Utilities.Newtonsoft.NewtonsoftJsonSerializer>();
            services.AddTransient<Interfaces.Utilities.Serializers.IGeneralSerializer, Utilities.Newtonsoft.NewtonsoftJsonSerializer>();

            var config = new Configuration();
            var applicationSection = config.Config.GetSection("Application");
            services.Configure<Common.Models.ApplicationSettings>(applicationSection);

            var loggingServerSection = config.Config.GetSection("Logging:Server");
            services.Configure<Common.Models.Errors.LoggingServerSettings>(loggingServerSection);

            var ghostscriptSection = config.Config.GetSection("Conversions:GhostScript");
            services.Configure<Common.Models.Conversions.GhostScriptSettings>(ghostscriptSection);
        }
    }
}
