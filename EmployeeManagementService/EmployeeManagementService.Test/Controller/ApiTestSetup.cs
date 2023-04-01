using EmployeeManagementService.API.Controllers;
using EmployeeManagementService.Domain.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RofShared.Services;
using RofShared.StartupInits;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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

        protected readonly string _employeeNotFoundMessage = "Employee not found!";

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

        protected async Task<HttpResponseMessage> SendRequest(string role, HttpMethod method, string url, StringContent content = null)
        {
            SetAuthHeaderOnHttpClient(role);

            var httpRequestMessage = new HttpRequestMessage(method, url);

            if (content != null)
            {
                httpRequestMessage.Content = content;
            }

            return await _httpClient.SendAsync(httpRequestMessage);
        }

        protected void AssertExpectedStatusCode(HttpResponseMessage res, HttpStatusCode expected)
        {
            Assert.IsNotNull(res);

            Assert.AreEqual(expected, res.StatusCode);
        }

        protected void AssertContentIsAsExpected(HttpResponseMessage res, string expectedContentAsString)
        {
            Assert.IsNotNull(res);

            Assert.IsNotNull(res.Content);

            var content = Task.Run(() => res.Content.ReadAsStringAsync()).Result;

            Assert.AreEqual(expectedContentAsString, content);
        }

        protected StringContent ConvertObjectToStringContent<T>(T obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }

        private void SetAuthHeaderOnHttpClient(string role)
        {
            var token = _tokenHandler.GenerateTokenForRole(role);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
