using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ClientManagementService.Domain.Services
{
    public interface IPasswordService
    {
        byte[] EncryptPassword(string password);
        bool VerifyPasswordHash(string password, byte[] passwordHash);
        bool VerifyPasswordRequirements(string password);
    }

    public class PasswordService : IPasswordService
    {
        private readonly string _saltString;
        private readonly byte[] _salt;

        public PasswordService(IConfiguration config)
        {
            _saltString = config.GetSection("PasswordSalt").Value;

            _salt = Encoding.UTF8.GetBytes(_saltString);
        }

        public byte[] EncryptPassword(string password)
        {
            using (var hmac = new HMACSHA512(_salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

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
            if (password.Length < 8 || password.Length > 32)
            {
                return false;
            }

            if (password.Contains(' '))
            {
                return false;
            }

            if (!password.Any(ch => char.IsUpper(ch)) || !password.Any(ch => char.IsLower(ch)))
            {
                return false;
            }

            if (!password.Any(ch => char.IsDigit(ch)))
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
