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
using ZNetCS.AspNetCore.Authentication.Basic;

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
            services.AddSingleton<BasicAuthConfigs>(i => Configuration.GetSection("BasicAuth").Get<BasicAuthConfigs>());
            services.AddControllers();
            services.AddScoped<AuthenticationEvents>();
            services.AddHttpClient();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TownSuite.ConversionServer.APISite", Version = "v1" });
            });
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
           .AddBasicAuthentication(
               options =>
               {
                   options.Realm = "TownSuite ConversionServer";
                   options.EventsType = typeof(AuthenticationEvents);
               });
            var standardServices = new StandardServices.StandardServices();
            standardServices.AddStandardServices(services);
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

            app.UseAuthentication();

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
