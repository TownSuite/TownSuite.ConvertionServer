using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.Tests.Common.Setup
{
    public class DependencyInjection
    {
        private ServiceProvider _serviceProvider;

        public DependencyInjection()
        {
            var collection = new ServiceCollection();

            var configuration = new Configuration();

            var loggingServerSection = configuration.Config.GetSection("Logging:Server");
            collection.Configure<ConversionServer.Common.Models.Conversions.GhostScriptSettings>(loggingServerSection);

            _serviceProvider = collection.BuildServiceProvider();
        }

        public T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
