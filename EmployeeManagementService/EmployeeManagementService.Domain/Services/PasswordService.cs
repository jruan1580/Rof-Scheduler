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
        private const string _saltString = "cmaqdcuzyidmmlfobtwduukqexizuflvwvzgwhdozhsuuadovmkgtogqjphrcbrwvkdhucpokstwgoff";
        private readonly byte[] _salt;

        public PasswordService()
        {
            _salt = Encoding.UTF8.GetBytes(_saltString);
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
    }
}
