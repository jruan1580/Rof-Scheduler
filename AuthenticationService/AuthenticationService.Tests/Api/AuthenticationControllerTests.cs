using AuthenticationService.Domain.Model;
using AuthenticationService.DTO.Controllers.Authentication;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Tests.Api
{
    [TestFixture]
    public class AuthenticationControllerTests : ApiTestSetup
    {
        private readonly string _baseUrl = "/api/Authentication";

        [Test]
        public async Task TestLoginSuccess()
        {
            _authService.Setup(a => a.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new BasicUserInfo() 
                { 
                    FirstName = "testFirstName",                     
                    Id = 1, 
                    Role = "Employee" 
                });

            var stringContent = GetStringContentForLogin();
            var url = $"{_baseUrl}/login";

            var res = await _httpClient.PatchAsync(url, stringContent);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);

            var content = res.Content;
            Assert.IsNotNull(content);

            var contentStr = await content.ReadAsStringAsync();

            Assert.True(contentStr.Contains("testFirstName"));
        }

        [Test]
        public async Task TestLoginInternalError()
        {
            _authService.Setup(a => a.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("force failed"));

            var stringContent = GetStringContentForLogin();
            var url = $"{_baseUrl}/login";

            var res = await _httpClient.PatchAsync(url, stringContent);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.InternalServerError, res.StatusCode);
        }

        [Test]
        public async Task TestLoginNotFound()
        {
            _authService.Setup(a => a.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new EntityNotFoundException("User"));

            var stringContent = GetStringContentForLogin();
            var url = $"{_baseUrl}/login";

            var res = await _httpClient.PatchAsync(url, stringContent);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Test]
        public async Task TestLoginBadRequest()
        {
            _authService.Setup(a => a.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("bad dto"));

            var stringContent = GetStringContentForLogin();
            var url = $"{_baseUrl}/login";

            var res = await _httpClient.PatchAsync(url, stringContent);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Test]
        public async Task TestLogoutSuccess()
        {
            _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var token = _tokenHandler.GenerateTokenForRole("Employee");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"{_baseUrl}/1/logout";

            var res = await _httpClient.PatchAsync(url, null);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        }

        [Test]
        public async Task TestLogoutUnauthorized()
        {
            //set to null
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var url = $"{_baseUrl}/1/logout";

            var res = await _httpClient.PatchAsync(url, null);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Test]
        public async Task TestLogoutInternalError()
        {
            _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>()))
                .Throws(new Exception("forced exception"));

            var token = _tokenHandler.GenerateTokenForRole("Employee");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"{_baseUrl}/1/logout";

            var res = await _httpClient.PatchAsync(url, null);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.InternalServerError, res.StatusCode);
        }

        [Test]
        public async Task TestLogoutNotFound()
        {
            _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>()))
                .Throws(new EntityNotFoundException("User"));

            var token = _tokenHandler.GenerateTokenForRole("Employee");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"{_baseUrl}/1/logout";

            var res = await _httpClient.PatchAsync(url, null);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Test]
        public async Task TestLogoutBadRequest()
        {
            _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>()))
                .Throws(new ArgumentException("bad id"));

            var token = _tokenHandler.GenerateTokenForRole("Employee");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"{_baseUrl}/1/logout";

            var res = await _httpClient.PatchAsync(url, null);

            Assert.IsNotNull(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, res.StatusCode);
        }

        private StringContent GetStringContentForLogin()
        {
            var loginDto = new LoginDTO() { Username = "test", Password = "testpw" };
            var stringContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            return stringContent;
        }
    }
}
