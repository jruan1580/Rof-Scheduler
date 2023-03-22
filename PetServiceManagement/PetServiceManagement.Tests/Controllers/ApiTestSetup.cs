using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using PetServiceManagement.API.Controllers;
using PetServiceManagement.Domain.BusinessLogic;
using RofShared.Services;
using RofShared.StartupInits;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PetServiceManagement.Tests.Controllers
{
    public class ApiTestSetup
    {
        private TestServer _server;
        protected HttpClient _httpClient;
        protected ITokenHandler _tokenHandler;

        protected Mock<IHolidayAndRateService> _holidayAndRateService = new Mock<IHolidayAndRateService>();

        [OneTimeSetUp]
        public void Setup()
        {
            var requestPipeline = GetRequestPipeline();

            var dependentServices = RegisterServices();

            var webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(dependentServices)
                .Configure(requestPipeline)
                .UseUrls("http://localhost");

            _server = new TestServer(webHostBuilder);
            _httpClient = _server.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _server.Dispose();

            _httpClient.Dispose();
        }

        private Action<IApplicationBuilder> GetRequestPipeline()
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

        private Action<IServiceCollection> RegisterServices()
        {
            var tokenConfig = GetConfiguration();

            _tokenHandler = new TokenHandler(tokenConfig);

            Action<IServiceCollection> services = service =>
            {
                service.AddTransient(provider => _tokenHandler);
                service.AddTransient(provider => _holidayAndRateService.Object);

                service.AddMvc().AddApplicationPart(typeof(HolidayRateController).Assembly);

                service.AddControllers();

                service.AddJwtAuthentication(tokenConfig);
            };

            return services;
        }

        private IConfiguration GetConfiguration()
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

        protected void SetAuthHeaderOnHttpClient(string role)
        {
            var token = _tokenHandler.GenerateTokenForRole(role);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
