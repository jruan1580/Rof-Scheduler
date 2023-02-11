using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace RofShared.Tests
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
            _config.Setup(c => c.GetSection(It.Is<string>(r => r.Equals("PasswordSalt"))).Value)
                .Returns("arandomstringforlocaldev");

            _passwordService = new PasswordService(_config.Object);
        }

        [Test]
        public void EncrptyPassword_Success()
        {
            var encryptedPass =  _passwordService.EncryptPassword("tE$t1234");

            Assert.IsNotNull(encryptedPass);
            Assert.Greater(encryptedPass.Length, 0);
        }

        [Test]
        public void VerifyPasswordHash_Fail()
        {
            var encryptedPass = _passwordService.EncryptPassword("tE$t1234123");

            var result = _passwordService.VerifyPasswordHash("tE$t1234", encryptedPass);

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordHash_Success()
        {
            var encryptedPass = _passwordService.EncryptPassword("tE$t1234");

            var result = _passwordService.VerifyPasswordHash("tE$t1234", encryptedPass);

            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyPasswordReq_LengthIncorrect()
        {
            var result = _passwordService.VerifyPasswordRequirements("t3$T");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_HasSpace()
        {
            var result = _passwordService.VerifyPasswordRequirements("t3 $T1234");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_MissingUpper()
        {
            var result = _passwordService.VerifyPasswordRequirements("te$t1234");
            
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_MissingLower()
        {
            var result = _passwordService.VerifyPasswordRequirements("TE$T1234");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_MissingNum()
        {
            var result = _passwordService.VerifyPasswordRequirements("tE$tabcd");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_MissingSpecial()
        {
            var result = _passwordService.VerifyPasswordRequirements("tESt1234");

            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPasswordReq_Success()
        {
            var result = _passwordService.VerifyPasswordRequirements("tE$t1234");

            Assert.IsTrue(result);
        }
    }
}
