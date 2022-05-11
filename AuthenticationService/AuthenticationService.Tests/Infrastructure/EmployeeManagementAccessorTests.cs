using AuthenticationService.Infrastructure.EmployeeManagement;
using AuthenticationService.Infrastructure.EmployeeManagement.Models;
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
    public class EmployeeManagementAccessorTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<IConfiguration> _configurationMock;

        private EmployeeManagementAccessor _accessor;

        [SetUp]
        public void Setup()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();            
            _configurationMock = new Mock<IConfiguration>();     

            _configurationMock.Setup(c => c.GetSection(It.Is<string>(s => s.Equals("EmployeeManagement:URL"))).Value)
                .Returns("http://localhost:123");

            _accessor = new EmployeeManagementAccessor(_configurationMock.Object, _httpClientFactoryMock.Object);
        }

        [Test]
        public async Task CheckIfEmployeeExistsSuccess()
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

            var res = await _accessor.CheckIfEmployee("testUsername", "testToken");

            Assert.IsTrue(res);
        }

        [Test]
        public async Task CheckIfEmployeeExistsNotFound()
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

            var res = await _accessor.CheckIfEmployee("testUsername", "testToken");

            Assert.IsFalse(res);
        }

        [Test]
        public void CheckIfEmployeeExistsExceptionThrown()
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
                    Content = new StringContent("an internal error has occurred", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _httpClientFactoryMock.Setup(factory => factory.CreateClient(String.Empty))
             .Returns(httpClient);

            Assert.ThrowsAsync<Exception>(() => _accessor.CheckIfEmployee("testUsername", "testToken"));
        }

        [Test]
        public async Task LoginSuccess()
        {
            var loginResponse = new EmployeeLoginResponse()
            {
                Id = 1,
                FirstName = "testUser",
                Role = "Employee"
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

            var res = await _accessor.Login("testUsername", "testPassword");

            Assert.IsNotNull(res);
            Assert.AreEqual(1, res.Id);
            Assert.AreEqual("testUser", res.FirstName);
            Assert.AreEqual("Employee", res.Role);
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

            Assert.ThrowsAsync<Exception>(() => _accessor.Login("testUsername", "testPassword"));
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

            var res = await _accessor.Logout(1, "/api/Employee/logout", "testToken");

            Assert.IsNotNull(res);
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

            Assert.ThrowsAsync<Exception>(() => _accessor.Logout(1, "/api/Employee/logout", "testToken"));
        }
    }
}
