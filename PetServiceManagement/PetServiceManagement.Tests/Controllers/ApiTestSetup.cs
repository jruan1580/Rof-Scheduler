using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PetServiceManagement.API.Controllers;
using PetServiceManagement.Domain.BusinessLogic;
using RofShared.Services;
using RofShared.StartupInits;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PetServiceManagement.Tests.Controllers
{
    public class ApiTestSetup
    {
        private TestServer _server;
        protected HttpClient _httpClient;
        protected ITokenHandler _tokenHandler;

        protected Mock<IHolidayService> _holidayService = new Mock<IHolidayService>();
        protected Mock<IHolidayRateService> _holidayRateService = new Mock<IHolidayRateService>();
        protected Mock<IPetServiceManagementService> _petServiceManagementService = new Mock<IPetServiceManagementService>();

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
                service.AddTransient(provider => _tokenHandler);
                service.AddTransient(provider => _holidayService.Object);
                service.AddTransient(provider => _holidayRateService.Object);
                service.AddTransient(provider => _petServiceManagementService.Object);

                service.AddMvc()
                    .AddApplicationPart(typeof(HolidayRateController).Assembly)
                    .AddApplicationPart(typeof(HolidayController).Assembly)
                    .AddApplicationPart(typeof(PetServiceController).Assembly);

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

        protected async Task SendDeleteRequestAndVerifySuccess(string url)
        {
            SetAuthHeaderOnHttpClient("Administrator");

            var res = await _httpClient.DeleteAsync(url);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        }

        protected async Task SendNonGetAndDeleteRequestAndVerifyBadRequest<T>(string url, string method, T body, string expectedExceptionMsg)
        {
            SetAuthHeaderOnHttpClient("Administrator");

            HttpResponseMessage res = await SendPostOrPutRequestAndGetResp(url, method, body);          

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);

            Assert.IsNotNull(res.Content);
            var content = await res.Content.ReadAsStringAsync();
            Assert.AreEqual(expectedExceptionMsg, content);
        }

        protected async Task SendNonGetAndDeleteRequestAndVerifySuccess<T>(string url, string method, T body)
        {
            SetAuthHeaderOnHttpClient("Administrator");

            HttpResponseMessage res = await SendPostOrPutRequestAndGetResp(url, method, body);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        }

        private async Task<HttpResponseMessage> SendPostOrPutRequestAndGetResp<T>(string url, string method, T body)
        {
            HttpResponseMessage res = null;
            if (method == "POST")
            {
                res = await _httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
            }
            else
            {
                res = await _httpClient.PutAsync(url, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
            }

            return res;
        }
    }
}
