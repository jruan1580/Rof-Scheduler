using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PetServiceManagement.API.Controllers;
using PetServiceManagement.Domain.BusinessLogic;
using RofShared.Services;
using RofShared.StartupInits;
using System;
using System.Collections.Generic;
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

        protected Mock<IHolidayAndRateService> _holidayAndRateService = new Mock<IHolidayAndRateService>();
        protected Mock<IPetServiceManagementService> _petServiceManagementService = new Mock<IPetServiceManagementService>();

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
                service.AddTransient(provider => _petServiceManagementService.Object);

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
