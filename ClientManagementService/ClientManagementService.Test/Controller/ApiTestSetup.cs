﻿using ClientManagementService.API.Controllers;
using ClientManagementService.Domain.Services;
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

namespace ClientManagementService.Test.Controller
{
    public class ApiTestSetup
    {
        private TestServer _server;
        protected HttpClient _httpClient;
        protected ITokenHandler _tokenHandler;

        protected readonly Mock<IClientAuthService> _clientAuthService = new Mock<IClientAuthService>();
        protected readonly Mock<IClientRetrievalService> _clientRetrievalService = new Mock<IClientRetrievalService>();
        protected readonly Mock<IClientUpsertService> _clientUpsertService = new Mock<IClientUpsertService>();
        protected readonly Mock<IPetRetrievalService> _petRetrievalService = new Mock<IPetRetrievalService>();
        protected readonly Mock<IPetService> _petService = new Mock<IPetService>();
        protected readonly Mock<IDropdownService> _dropdownService = new Mock<IDropdownService>();

        protected readonly string _clientNotFoundMessage = "Client not found!";
        protected readonly string _petNotFoundMessage = "Pet not found!";

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
                service.AddTransient(provider => _clientAuthService.Object);
                service.AddTransient(provider => _clientRetrievalService.Object);
                service.AddTransient(provider => _clientUpsertService.Object);
                service.AddTransient(provider => _petRetrievalService.Object);
                service.AddTransient(provider => _petService.Object);
                service.AddTransient(provider => _dropdownService.Object);

                service.AddMvc()
                    .AddApplicationPart(typeof(ClientController).Assembly)
                    .AddApplicationPart(typeof(PetController).Assembly)
                    .AddApplicationPart(typeof(DropdownController).Assembly);

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

