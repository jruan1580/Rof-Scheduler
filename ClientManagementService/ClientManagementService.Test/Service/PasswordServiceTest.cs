using ClientManagementService.Domain.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientManagementService.Test.Service
{
    [TestFixture]
    public class PasswordServiceTest
    {
        private Mock<IConfiguration> _config;
        private IPasswordService _passwordService;

        [SetUp]
        public void Setup()
        {
            _config = new Mock<IConfiguration>();
            _config.Setup(c => c.GetSection(It.Is<string>(p => p.Equals("PasswordSalt"))).Value)
                .Returns("arandomstringforlocaldev");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public void EncrptyPassword_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            Assert.IsNotNull(encryptedPass);
            Assert.Greater(encryptedPass.Length, 0);
        }

        [Test]
        public void VerifyPasswordHash_Fail()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            var result = _passwordService.VerifyPasswordHash("TESTpassword123!", encryptedPass);

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordHash_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("TestPassword123!");

            var result = _passwordService.VerifyPasswordHash("TestPassword123!", encryptedPass);

            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyPasswordReq_LengthIncorrect()
        {
            var result = _passwordService.VerifyPasswordRequirements("Test0!");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_HasSpace()
        {
            var result = _passwordService.VerifyPasswordRequirements("Test Password 123!");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_MissingUpper()
        {
            var result = _passwordService.VerifyPasswordRequirements("testpassword123!");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_MissingLower()
        {
            var result = _passwordService.VerifyPasswordRequirements("TESTPASSWORD123!");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_MissingNum()
        {
            var result = _passwordService.VerifyPasswordRequirements("TestPassword!");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_MissingSpecial()
        {
            var result = _passwordService.VerifyPasswordRequirements("TestPassword123");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_Success()
        {
            var result = _passwordService.VerifyPasswordRequirements("TestPassword123!");

            Assert.IsTrue(result);
        }
    }
}
