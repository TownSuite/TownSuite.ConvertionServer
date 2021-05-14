using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownSuite.ConversionServer.APISite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddHttpClient();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TownSuite.ConversionServer.APISite", Version = "v1" });
            });

            services.AddTransient<Interfaces.Common.Errors.IResponseErrorModelFactory<Common.Models.Errors.ResponseErrorModel>, Common.Errors.ResponseErrorModelFactory>();
            services.AddTransient<Interfaces.Utilities.Converters.IPdfToImageBytesConverter, Utilities.GhostScript.PdfToImageConverter>();
            services.AddTransient<Interfaces.Utilities.Logging.ISimpleLogger, Utilities.TownSuiteLogging.SimpleLogger>();
            services.AddTransient<Interfaces.Utilities.Serializers.IJsonSerializer, Utilities.Newtonsoft.NewtonsoftJsonSerializer>();
            services.AddTransient<Interfaces.Utilities.Serializers.IGeneralSerializer, Utilities.Newtonsoft.NewtonsoftJsonSerializer>();
            services.AddTransient<Interfaces.Utilities.Logging.IModelLogger<Common.Models.Errors.LoggingErrorModel>, Utilities.TownSuiteLogging.ErrorClient>();

            // If this expands far, change to services.Configure and sections.
            var applicationSection = Configuration.GetSection("Application");
            services.Configure<Common.Models.ApplicationSettings>(applicationSection);

            var loggingServerSection = Configuration.GetSection("Logging:Server");
            services.Configure<Common.Models.Errors.LoggingServerSettings>(loggingServerSection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TownSuite.ConversionServer.APISite v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
