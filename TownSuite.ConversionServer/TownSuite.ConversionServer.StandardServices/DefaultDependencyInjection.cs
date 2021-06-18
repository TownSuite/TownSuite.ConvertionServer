using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.StandardServices
{
    public class DefaultDependencyInjection
    {
        public IServiceProvider ServiceProvider { get; set; }

        public DefaultDependencyInjection()
        {
            var serviceCollection = new ServiceCollection();
            var standardService = new StandardServices();
            standardService.AddStandardServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
