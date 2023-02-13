using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RofShared.Services;
using System;

namespace RofShared.Tests.Services
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
        public void ValidatePasswordForLoginWithInvalidPassword()
        {
            var invalidPassword = "tE$t1234";
            var validPassword = "tE$t1234123";
            var storedPasswordHash = _passwordService.EncryptPassword(validPassword);

            Assert.Throws<ArgumentException>(() => _passwordService.ValidatePasswordForLogin(invalidPassword, storedPasswordHash));
        }
        
        [Test]
        public void ValidatePasswordForLoginWithValidPassword()
        {
            var validPassword = "tE$t1234123";
            var storedPasswordHash = _passwordService.EncryptPassword(validPassword);

            try
            {
                _passwordService.ValidatePasswordForLogin(validPassword, storedPasswordHash);

                Assert.Pass();
            }
            catch(ArgumentException)
            {
                Assert.Fail("argument exception was thrown when it should not have been thrown. password is valid!");
            }
        }

        [Test]
        public void PasswordDoesNotRequirementForCreateAndUpdate()
        {
            var shortPassword = "t3$T";
            Assert.Throws<ArgumentException>(() => _passwordService.ValidatePasswordForCreate(shortPassword));
            Assert.Throws<ArgumentException>(() => _passwordService.ValidateNewPasswordForUpdate(shortPassword, null));

            var longPassword = "astasf123aasdsad123fasasfasf1312afhagfh999asdadhsahdd!@#!@$!@ASFASF";
            Assert.Throws<ArgumentException>(() => _passwordService.ValidatePasswordForCreate(longPassword));
            Assert.Throws<ArgumentException>(() => _passwordService.ValidateNewPasswordForUpdate(longPassword, null));

            var noLowerCasePassword = "ASDASD!@3123124";
            Assert.Throws<ArgumentException>(() => _passwordService.ValidatePasswordForCreate(noLowerCasePassword));
            Assert.Throws<ArgumentException>(() => _passwordService.ValidateNewPasswordForUpdate(noLowerCasePassword, null));

            var noUpperCasePassword = "asdasdasd!@#12451244";
            Assert.Throws<ArgumentException>(() => _passwordService.ValidatePasswordForCreate(noUpperCasePassword));
            Assert.Throws<ArgumentException>(() => _passwordService.ValidateNewPasswordForUpdate(noUpperCasePassword, null));

            var noDigitsPassword = "ASDASasdasda!@#!@#";
            Assert.Throws<ArgumentException>(() => _passwordService.ValidatePasswordForCreate(noDigitsPassword));
            Assert.Throws<ArgumentException>(() => _passwordService.ValidateNewPasswordForUpdate(noDigitsPassword, null));

            var noSpecialCharPassword = "ASDASDdasdasdsad123";
            Assert.Throws<ArgumentException>(() => _passwordService.ValidatePasswordForCreate(noSpecialCharPassword));
            Assert.Throws<ArgumentException>(() => _passwordService.ValidateNewPasswordForUpdate(noSpecialCharPassword, null));
        }

        [Test]
        public void ValidatePasswordForCreateWithGoodPassword()
        {
            var goodPassword = "tE$t1234123";
            try
            {
                _passwordService.ValidatePasswordForCreate(goodPassword);

                Assert.Pass();
            }
            catch (ArgumentException)
            {
                Assert.Fail("argument exception was thrown when it should not have been thrown. password is valid!");
            }
        }
     
        [Test]        
        public void NewPasswordIsSameAsExistingForUpdate()
        {
            var newPassword = "tE$t1234123";
            var storedPasswordHash = _passwordService.EncryptPassword(newPassword);

            Assert.Throws<ArgumentException>(() => _passwordService.ValidateNewPasswordForUpdate(newPassword, storedPasswordHash));
        }

        [Test]
        public void ValidatePasswordForUpdateWithNewPassword()
        {
            var newPassword = "tE$t1234123";
            var existingPassword = "tE$t123111234";
            var storedPasswordHash = _passwordService.EncryptPassword(existingPassword);

            try
            {
                _passwordService.ValidateNewPasswordForUpdate(newPassword, storedPasswordHash);

                Assert.Pass();
            }
            catch (ArgumentException)
            {
                Assert.Fail("argument exception was thrown when it should not have been thrown. new password is valid!");
            }
        }
    }
}
