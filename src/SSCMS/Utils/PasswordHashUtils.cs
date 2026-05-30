using System;
using System.Security.Cryptography;

namespace SSCMS.Utils
{
    public static class PasswordHashUtils
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100000;

        public static string HashPassword(string password, out string salt)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
            salt = Convert.ToBase64String(saltBytes);
            return HashNormalizedPassword(AuthUtils.Md5ByString(password), saltBytes);
        }

        public static bool VerifyPassword(string password, bool isPasswordMd5, string hash, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(salt))
            {
                return false;
            }

            var normalizedPassword = isPasswordMd5 ? password : AuthUtils.Md5ByString(password);
            var saltBytes = Convert.FromBase64String(salt);
            var computedHash = HashNormalizedPassword(normalizedPassword, saltBytes);
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(hash),
                Convert.FromBase64String(computedHash));
        }

        private static string HashNormalizedPassword(string normalizedPassword, byte[] saltBytes)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                normalizedPassword,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256);
            return Convert.ToBase64String(pbkdf2.GetBytes(KeySize));
        }
    }
}
