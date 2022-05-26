using AuthenticationService.Infrastructure.ClientManagement;
using AuthenticationService.Infrastructure.ClientManagement.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuthenticationService.Tests.Infrastructure
{
    [TestFixture]
    public class ClientManagementAccessorTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<IConfiguration> _configurationMock;

        private ClientManagementAccessor _accessor;

        [SetUp]
        public void Setup()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c.GetSection(It.Is<string>(s => s.Equals("ClientManagement:URL"))).Value)
                .Returns("http://localhost:123");

            _accessor = new ClientManagementAccessor(_configurationMock.Object, _httpClientFactoryMock.Object);
        }

        [Test]
        public async Task LoginSuccess()
        {
            var loginResponse = new ClientLoginResponse()
            {
                Id = 1,
                FirstName = "testUser"
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(loginResponse), Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            var res = await _accessor.Login("testUsername", "testPassword", "internalToken");

            Assert.IsNotNull(res);
            Assert.AreEqual(1, res.Id);
            Assert.AreEqual("testUser", res.FirstName);
        }

        [Test]
        public async Task LoginNotFoundStatusCode()
        {
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.NotFound
               });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            var loginResp = await _accessor.Login("testUsername", "testPassword", "internalToken");

            Assert.IsNull(loginResp);
        }

        [Test]
        public void LoginBadRequestStatusCode()
        {
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.BadRequest,
                   Content = new StringContent("username not found", Encoding.UTF8)
               });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            Assert.ThrowsAsync<ArgumentException>(() => _accessor.Login("testUsername", "testPassword", "internalToken"));
        }

        [Test]
        public void LoginUnsuccessfulStatusCode()
        {
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.InternalServerError,
                   Content = new StringContent("failed to login", Encoding.UTF8)
               });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            Assert.ThrowsAsync<Exception>(() => _accessor.Login("testUsername", "testPassword", "internalToken"));
        }

        [Test]
        public async Task LogoutSuccess()
        {
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK
               });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            var res = await _accessor.Logout(1, "testToken");

            Assert.IsNotNull(res);
        }

        [Test]
        public async Task LogoutNotFoundStatusCode()
        {
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.NotFound
               });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            var logoutResp = await _accessor.Logout(1, "testToken");

            Assert.IsNull(logoutResp);
        }

        [Test]
        public void LogoutBadRequestStatusCode()
        {
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.BadRequest,
                   Content = new StringContent("username not found", Encoding.UTF8)
               });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            Assert.ThrowsAsync<ArgumentException>(() => _accessor.Logout(1, "testToken"));
        }

        [Test]
        public void LogoutUnsuccessfulStatusCode()
        {
            _httpMessageHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
              )
              .ReturnsAsync(new HttpResponseMessage()
              {
                  StatusCode = HttpStatusCode.InternalServerError,
                  Content = new StringContent("failed to logout", Encoding.UTF8)
              });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            Assert.ThrowsAsync<Exception>(() => _accessor.Logout(1, "testToken"));
        }
    }
}
