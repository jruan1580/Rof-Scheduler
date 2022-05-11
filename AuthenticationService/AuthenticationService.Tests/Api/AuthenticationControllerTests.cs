using AuthenticationService.API.Controllers;
using AuthenticationService.API.Models;
using AuthenticationService.Domain.Exceptions;
using AuthenticationService.Domain.Model;
using AuthenticationService.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationService.Tests.Api
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IAuthService> _authService;
        private Mock<ITokenHandler> _tokenHandler;

        private AuthenticationController _ctr;

        [SetUp]
        public void Setup()
        {
            _authService = new Mock<IAuthService>();
            _tokenHandler = new Mock<ITokenHandler>();

            _tokenHandler.Setup(t => t.GenerateTokenForRole(It.IsAny<string>(), It.IsAny<int>()))
                .Returns("tokentousefortesting");

            _tokenHandler.Setup(t => t.ExtractTokenFromRequest(It.IsAny<HttpRequest>(), It.IsAny<string>()))
                .Returns("tokentobeusedfortesting");

            _ctr = new AuthenticationController(_tokenHandler.Object, _authService.Object);
        }

        [Test]
        public async Task TestLoginSuccess()
        {
            _authService.Setup(a => a.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new BasicUserInfo() 
                { 
                    FirstName = "test", 
                    Id = 1, 
                    Role = "Employee" 
                });

            _ctr.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var res = await _ctr.Login(new LoginDTO() { Username = "test", Password = "testpw" });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkObjectResult), res.GetType());

            var okObj = (OkObjectResult)res;

            Assert.AreEqual(200, okObj.StatusCode);
            Assert.IsNotNull(okObj.Value);            
        }

        [Test]
        public async Task TestLoginInternalError()
        {
            _authService.Setup(a => a.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("force failed"));

            _ctr.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var res = await _ctr.Login(new LoginDTO() { Username = "test", Password = "testpw" });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(ObjectResult), res.GetType());

            var objRes = (ObjectResult)res;

            Assert.AreEqual(500, objRes.StatusCode);
            Assert.NotNull(objRes.Value);
            Assert.AreEqual(typeof(string), objRes.Value.GetType());
            Assert.AreEqual("force failed", objRes.Value.ToString());
        }

        [Test]
        public async Task TestLoginNotFound()
        {
            _authService.Setup(a => a.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new NotFoundException());

            _ctr.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var res = await _ctr.Login(new LoginDTO() { Username = "test", Password = "testpw" });

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(NotFoundObjectResult), res.GetType());

            var notFound = (NotFoundObjectResult)res;

            Assert.AreEqual(404, notFound.StatusCode);
            Assert.NotNull(notFound.Value);
            Assert.AreEqual(typeof(string), notFound.Value.GetType());
            Assert.AreEqual("Username not found", notFound.Value.ToString());
        }

        [Test]
        public async Task TestLogoutSuccess()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "Employee")
            };
            
            var identity = new ClaimsIdentity(claims, "Bearer");
            var user = new ClaimsPrincipal(identity);
            
            _ctr.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user
                }
            };

            _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var res = await _ctr.Logout(1);

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(OkResult), res.GetType());

            var ok = (OkResult)res;

            Assert.AreEqual(200, ok.StatusCode);
        }

        [Test]
        public async Task TestLogoutInternalError()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "Employee")
            };

            var identity = new ClaimsIdentity(claims, "Bearer");
            var user = new ClaimsPrincipal(identity);

            _ctr.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user
                }
            };

            _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("forced exception"));

            var res = await _ctr.Logout(1);

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(ObjectResult), res.GetType());

            var objRes = (ObjectResult)res;

            Assert.AreEqual(500, objRes.StatusCode);
            Assert.NotNull(objRes.Value);
            Assert.AreEqual(typeof(string), objRes.Value.GetType());
            Assert.AreEqual("forced exception", objRes.Value.ToString());
        }

        [Test]
        public async Task TestLogoutNotFound()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "Employee")
            };

            var identity = new ClaimsIdentity(claims, "Bearer");
            var user = new ClaimsPrincipal(identity);

            _ctr.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user
                }
            };

            _authService.Setup(a => a.Logout(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new NotFoundException());

            var res = await _ctr.Logout(1);

            Assert.IsNotNull(res);
            Assert.AreEqual(typeof(NotFoundObjectResult), res.GetType());

            var notFound = (NotFoundObjectResult)res;

            Assert.AreEqual(404, notFound.StatusCode);
            Assert.NotNull(notFound.Value);
            Assert.AreEqual(typeof(string), notFound.Value.GetType());
            Assert.AreEqual("User not found", notFound.Value.ToString());
        }
    }
}
