using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RofShared.StartupInits;
using System;
using System.Collections.Generic;

namespace RofShared.Services
{
    public static class UnitTestSetupHelper
    {
        public static IWebHostBuilder GetWebHostBuilder(Action<IServiceCollection> registeredServices)
        {
            var requestPipeline = GetRequestPipeline();

            return new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(registeredServices)
                .Configure(requestPipeline)
                .UseUrls("http://localhost");
        }

        public static Action<IApplicationBuilder> GetRequestPipeline()
        {
            Action<IApplicationBuilder> requestPipeline = app =>
            {
                app.AddExceptionHandlerForApi();

                app.UseHttpsRedirection();

                app.UseRouting();

                app.UseAuthentication();

                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            };

            return requestPipeline;
        }

        public static IConfiguration GetConfiguration()
        {
            var tokenConfig = new Dictionary<string, string>();
            tokenConfig.Add("Jwt:Key", "thisisjustsomerandomlocalkey");
            tokenConfig.Add("Jwt:Issuer", "localhost.com");
            tokenConfig.Add("Jwt:Audience", "rof_services");

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(tokenConfig)
                .Build();

            return configuration;
        }
    }
}
