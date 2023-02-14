using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RofShared.Services
{
    public interface IPasswordService
    {
        byte[] EncryptPassword(string password);
        void ValidatePasswordForLogin(string password, byte[] storedPasswordHash);
        void ValidatePasswordForCreate(string password);
        void ValidateNewPasswordForUpdate(string newPassword, byte[] storedPasswordHash);
    }

    public class PasswordService : IPasswordService
    {
        private readonly byte[] _salt;

        public PasswordService(IConfiguration config)
        {
             var saltString = config.GetSection("PasswordSalt").Value;

            _salt = Encoding.UTF8.GetBytes(saltString);
        }

        /// <summary>
        /// Method to create password hash, allowing password encryptions
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Password in hash code</returns>
        public byte[] EncryptPassword(string password)
        {
            using (var hmac = new HMACSHA512(_salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public void ValidatePasswordForLogin(string password, byte[] storedPasswordHash)
        {
            if (!EnsurePasswordMatchesStoredPassword(password, storedPasswordHash))
            {
                throw new ArgumentException("Passowrd is incorrect");
            }
        }

        public void ValidatePasswordForCreate(string password)
        {
            if (!PasswordMeetsRequirements(password))
            {
                throw new ArgumentException("Password does not meet all requirements");
            }
        }

        public void ValidateNewPasswordForUpdate(string newPassword, byte[] storedPasswordHash)
        {        
            if (!PasswordMeetsRequirements(newPassword))
            {
                throw new ArgumentException("New password does not meet all requirements");
            }

            var newPasswordSameAsExisting = EnsurePasswordMatchesStoredPassword(newPassword, storedPasswordHash);
            if (newPasswordSameAsExisting)
            {
                throw new ArgumentException("New password is same as existing password");
            }
        }
        
        /// <summary>
        /// Method to ensure that the password provided matches the password stored in the DB
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <returns>True if password stored matches password provided</returns>
        private bool EnsurePasswordMatchesStoredPassword(string password, byte[] passwordHash)
        {
            var encryptedPassword = EncryptPassword(password);

            for (var i = 0; i < encryptedPassword.Length; i++)
            {
                if (encryptedPassword[i] != passwordHash[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Password length must be between 8 and 32 characters.
        /// Password cannot contain empty spaces.
        /// Password must contain both upper and lower case characters.
        /// Password must contain atleast one number.
        /// Passowrd contains at least one special character (non alpha numeric)
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool PasswordMeetsRequirements(string password)
        {
            var passwordMeetsLengthRequirement = password.Length >= 8 && password.Length <= 32;
            var passwordDoesNotContainEmptySpaces = !password.Contains(' ');
            var passwordHasBothUpperAndLowercase = password.Any(ch => char.IsUpper(ch)) && password.Any(ch => char.IsLower(ch));
            var passwordContainsDigits = password.Any(ch => char.IsDigit(ch));
            var passwordContainSpecialChars = password.Any(ch => !char.IsLetterOrDigit(ch));

            return passwordMeetsLengthRequirement && passwordDoesNotContainEmptySpaces
                && passwordHasBothUpperAndLowercase && passwordContainsDigits && passwordContainSpecialChars;
        }        
    }
}
