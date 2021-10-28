using APIFrame.Web.Authentication.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace APIFrame.Web.Authentication
{
    public class SecureHashGeneratorService : ISecureHashGeneratorService
    {
        private const int SALT_SIZE = 24;
        private const int HASH_SIZE = 24;
        private const int ITERATIONS = 100000;

        public string CreateHMACSHA256(string value, string key)
        {
            var hasher = new HMACSHA256(Encoding.ASCII.GetBytes(key));
            var hash = hasher.ComputeHash(Encoding.ASCII.GetBytes(value));

            return BitConverter.ToString(hash)
                .Replace("-", "")
                .ToLower();
        }

        public string CreateHash(string value, string salt, int hashSize = HASH_SIZE)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(salt))
            {
                throw new ArgumentException();
            }

            string resultHash = null;

            try
            {
                byte[] saltArray = Convert.FromBase64String(salt.ToLower());
                var pbkdf2 = new Rfc2898DeriveBytes(value, saltArray, ITERATIONS);

                byte[] hashBytes = pbkdf2.GetBytes(hashSize);
                resultHash = Convert.ToBase64String(hashBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return resultHash;
        }

        public string CreateSalt()
        {
            string resultSalt = null;

            try
            {
                var provider = new RNGCryptoServiceProvider();
                byte[] rawSalt = new byte[SALT_SIZE];
                provider.GetBytes(rawSalt);

                resultSalt = Convert.ToBase64String(rawSalt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return resultSalt?.ToLower();
        }
    }
}
