using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.StandardServices
{
    public class Configuration
    {
        public IConfiguration Config { get; private set; }

        public Configuration()
        {
            var builder = new ConfigurationBuilder();

            string devFile = "appsettings.Development.json";
            bool useDevFile = false;
#if DEBUG
            useDevFile = true;
#endif
            if (useDevFile && System.IO.File.Exists(devFile))
            {
                builder.AddJsonFile(devFile);
            }
            else
            {
                builder.AddJsonFile("appsettings.json");
            }

            Config = builder.Build();
        }
    }
}
