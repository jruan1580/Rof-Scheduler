using EmployeeManagementService.API.Controllers;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RofShared.Services;
using RofShared.StartupInits;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EmployeeManagementService.Test.Controller
{
    public class ApiTestSetup
    {
        private TestServer _server;
        protected HttpClient _httpClient;
        protected ITokenHandler _tokenHandler;

        protected readonly Mock<IEmployeeAuthService> _employeeAuthService = new Mock<IEmployeeAuthService>();
        protected readonly Mock<IEmployeeRetrievalService> _employeeRetrievalService = new Mock<IEmployeeRetrievalService>();
        protected readonly Mock<IEmployeeUpsertService> _employeeUpsertService = new Mock<IEmployeeUpsertService>();

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
                service.AddTransient(provider => _employeeAuthService.Object);
                service.AddTransient(provider => _employeeRetrievalService.Object);
                service.AddTransient(provider => _employeeUpsertService.Object);

                service.AddMvc()
                    .AddApplicationPart(typeof(AdminController).Assembly)
                    .AddApplicationPart(typeof(EmployeeController).Assembly);                    

                service.AddControllers();

                service.AddJwtAuthentication(tokenConfig);
            };

            return services;
        }
        protected void SetAuthHeaderOnHttpClient(string role)
        {
            var token = _tokenHandler.GenerateTokenForRole(role);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected void AssertExpectedStatusCode(HttpResponseMessage res, HttpStatusCode expected)
        {
            Assert.IsNotNull(res);

            Assert.AreEqual(expected, res.StatusCode);
        }
    }
}
