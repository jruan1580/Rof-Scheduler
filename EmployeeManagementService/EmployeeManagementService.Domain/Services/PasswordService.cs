using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagementService.Domain.Services
{
    public interface IPasswordService
    {
        byte[] EncryptPassword(string password);
        bool VerifyPasswordHash(string password, byte[] passwordHash);        
    }

    public class PasswordService : IPasswordService
    {
        private readonly string _saltString;
        private readonly byte[] _salt;

        public PasswordService(IConfiguration config)
        {
            _salt = Encoding.UTF8.GetBytes(_saltString);

            _saltString = config.GetSection("PasswordSalt").Value;
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

        /// <summary>
        /// Method to ensure that the password provided matches the password stored in the DB
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <returns>True if password stored matches password provided</returns>
        public bool VerifyPasswordHash(string password, byte[] passwordHash)
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
        
        public bool VerifyPasswordRequirements(string password)
        {
            if(password.Length < 8 || password.Length > 32)
            {
                return false;
            }

            if (password.Contains(' '))
            {
                return false;
            }

            if(!password.Any(char.IsUpper) || !password.Any(char.IsLower))
            {
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return false;
            }

            return true;
        }
    }
}
