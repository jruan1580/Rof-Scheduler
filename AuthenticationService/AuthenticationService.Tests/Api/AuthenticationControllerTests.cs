using AuthenticationService.API.Controllers;
using AuthenticationService.Domain.Model;
using AuthenticationService.Domain.Services;
using AuthenticationService.DTO.Controllers.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RofShared.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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

        //[Test]
        //public async Task TestLogoutSuccess()
        //{
        //    var claims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.Role, "Employee")
        //    };

        //    var identity = new ClaimsIdentity(claims, "Bearer");
        //    var user = new ClaimsPrincipal(identity);

        //    _httpClient.

        //    _ctr.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user
        //        }
        //    };

        //    _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>()))
        //        .Returns(Task.CompletedTask);

        //    var res = await _ctr.Logout(1);

        //    Assert.IsNotNull(res);
        //    Assert.AreEqual(typeof(OkResult), res.GetType());

        //    var ok = (OkResult)res;

        //    Assert.AreEqual(200, ok.StatusCode);
        //}

        //[Test]
        //public async Task TestLogoutInternalError()
        //{
        //    var claims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.Role, "Employee")
        //    };

        //    var identity = new ClaimsIdentity(claims, "Bearer");
        //    var user = new ClaimsPrincipal(identity);

        //    _ctr.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user
        //        }
        //    };

        //    _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>()))
        //        .Throws(new Exception("forced exception"));

        //    var res = await _ctr.Logout(1);

        //    Assert.IsNotNull(res);
        //    Assert.AreEqual(typeof(ObjectResult), res.GetType());

        //    var objRes = (ObjectResult)res;

        //    Assert.AreEqual(500, objRes.StatusCode);
        //    Assert.NotNull(objRes.Value);
        //    Assert.AreEqual(typeof(string), objRes.Value.GetType());
        //    Assert.AreEqual("forced exception", objRes.Value.ToString());
        //}

        //[Test]
        //public async Task TestLogoutNotFound()
        //{
        //    var claims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.Role, "Employee")
        //    };

        //    var identity = new ClaimsIdentity(claims, "Bearer");
        //    var user = new ClaimsPrincipal(identity);

        //    _ctr.ControllerContext = new ControllerContext()
        //    {
        //        HttpContext = new DefaultHttpContext()
        //        {
        //            User = user
        //        }
        //    };

        //    _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>()))
        //        .Throws(new EntityNotFoundException("User"));

        //    var res = await _ctr.Logout(1);

        //    Assert.IsNotNull(res);
        //    Assert.AreEqual(typeof(NotFoundObjectResult), res.GetType());

        //    var notFound = (NotFoundObjectResult)res;

        //    Assert.AreEqual(404, notFound.StatusCode);
        //    Assert.NotNull(notFound.Value);
        //    Assert.AreEqual(typeof(string), notFound.Value.GetType());
        //    Assert.AreEqual("User not found", notFound.Value.ToString());
        //}

        private StringContent GetStringContentForLogin()
        {
            var loginDto = new LoginDTO() { Username = "test", Password = "testpw" };
            var stringContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            return stringContent;
        }
    }
}
