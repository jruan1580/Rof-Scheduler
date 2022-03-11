using EmployeeManagementService.Domain.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementService.Test.Service
{
    [TestFixture]
    public class PasswordServiceTest
    {
        private Mock<IPasswordService> _passwordService;
        private Mock<IConfiguration> _config;

        [SetUp]
        public void Setup()
        {
            _passwordService = new Mock<IPasswordService>();
            _config = new Mock<IConfiguration>();
            _config.Setup(c => c.GetSection(It.Is<string>(r => r.Equals("PasswordSalt"))).Value)
                .Returns("arandomstringforlocaldev");
        }

        [Test]
        public void EncrptyPassword_Success()
        {
            _passwordService.Setup(p => p.EncryptPassword(It.IsAny<string>()))
                .Returns(new byte[32]);

            var passwordService = new PasswordService(_config.Object);

            passwordService.EncryptPassword("tE$t1234");

            _passwordService.Verify(p => p.EncryptPassword(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordHash_Fail()
        {
            _passwordService.Setup(p => p.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(false);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordHash("tE$t1234", new byte[32]);

            _passwordService.Verify(p => p.EncryptPassword(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordHash_Success()
        {
            _passwordService.Setup(p => p.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(true);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordHash("tE$t1234", new byte[32]);

            _passwordService.Verify(p => p.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordReq_LengthIncorrect()
        {
            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.IsAny<string>()))
                .Returns(false);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordRequirements("tE$t123");

            _passwordService.Verify(p => p.VerifyPasswordRequirements(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordReq_HasSpace()
        {
            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.IsAny<string>()))
                .Returns(false);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordRequirements("tE$t1 234");

            _passwordService.Verify(p => p.VerifyPasswordRequirements(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordReq_MissingUpper()
        {
            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.IsAny<string>()))
                .Returns(false);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordRequirements("te$t1234");

            _passwordService.Verify(p => p.VerifyPasswordRequirements(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordReq_MissingLower()
        {
            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.IsAny<string>()))
                .Returns(false);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordRequirements("TE$T1234");

            _passwordService.Verify(p => p.VerifyPasswordRequirements(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordReq_MissingNum()
        {
            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.IsAny<string>()))
                .Returns(false);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordRequirements("tE$tabcd");

            _passwordService.Verify(p => p.VerifyPasswordRequirements(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordReq_MissingSpecial()
        {
            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.IsAny<string>()))
                .Returns(false);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordRequirements("tESt1234");

            _passwordService.Verify(p => p.VerifyPasswordRequirements(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void VerifyPasswordReq_Success()
        {
            _passwordService.Setup(p => p.VerifyPasswordRequirements(It.IsAny<string>()))
                .Returns(true);

            var passwordService = new PasswordService(_config.Object);

            passwordService.VerifyPasswordRequirements("tE$t1234");

            _passwordService.Verify(p => p.VerifyPasswordRequirements(It.IsAny<string>()), Times.Once);
        }
    }
}
