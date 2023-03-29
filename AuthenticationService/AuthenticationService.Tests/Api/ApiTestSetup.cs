using AuthenticationService.API.Controllers;
using AuthenticationService.Domain.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RofShared.Services;
using RofShared.StartupInits;
using System;
using System.Net;
using System.Net.Http;

namespace AuthenticationService.Tests.Api
{
    public class ApiTestSetup
    {
        protected ITokenHandler _tokenHandler;
        protected readonly Mock<IAuthService> _authService = new Mock<IAuthService>();
        private TestServer _server;
        protected HttpClient _httpClient;        

        [OneTimeSetUp]
        public void Setup()
        {
            var dependentServices = RegisterServices();

            var webHostBuilder = UnitTestSetupHelper.GetWebHostBuilder(dependentServices);

            _server = new TestServer(webHostBuilder);
            _httpClient = _server.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _server.Dispose();

            _httpClient.Dispose();
        }

        private Action<IServiceCollection> RegisterServices()
        {
            var tokenConfig = UnitTestSetupHelper.GetConfiguration();

            _tokenHandler = new TokenHandler(tokenConfig);
            
            Action<IServiceCollection> services = service =>
            {               
                service.AddTransient(provider => _authService.Object);
                service.AddTransient(provider => _tokenHandler);

                service.AddMvc().AddApplicationPart(typeof(AuthenticationController).Assembly);

                service.AddControllers();

                service.AddJwtAuthentication(tokenConfig);
            };

            return services;
        }

        protected void AssertStatusCodeEqualsExpected(HttpResponseMessage res, HttpStatusCode expected)
        {
            Assert.IsNotNull(res);
            Assert.AreEqual(expected, res.StatusCode);
        }
    }
}
