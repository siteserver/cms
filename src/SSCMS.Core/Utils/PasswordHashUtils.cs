using System;
using System.Security.Cryptography;
using System.Text;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// 不可逆口令存储（GJ20260924）：PBKDF2-HMAC-SHA256。
    /// 输入统一为 MD5(明文) 的小写十六进制，这样前端登录仍提交 MD5，服务端无需明文即可校验。
    /// PasswordSalt 字段格式：pbkdf2-sha256$迭代次数$Base64(盐)，便于今后调整迭代次数。
    /// </summary>
    public static class PasswordHashUtils
    {
        public const int Iterations = 210000;
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const string SaltPrefix = "pbkdf2-sha256";

        public static string ToMd5(string plainPassword)
        {
            return AuthUtils.Md5ByString(plainPassword ?? string.Empty);
        }

        public static string Hash(string passwordMd5, out string passwordSalt)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            passwordSalt = $"{SaltPrefix}${Iterations}${Convert.ToBase64String(salt)}";
            return Convert.ToBase64String(Derive(passwordMd5, salt, Iterations));
        }

        public static bool Verify(string passwordMd5, string storedHash, string passwordSalt)
        {
            if (string.IsNullOrEmpty(passwordMd5) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(passwordSalt)) return false;

            var parts = passwordSalt.Split('$');
            if (parts.Length != 3 || parts[0] != SaltPrefix || !int.TryParse(parts[1], out var iterations) || iterations < 1) return false;

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(storedHash);
            }
            catch (FormatException)
            {
                return false;
            }

            var actual = Derive(passwordMd5, salt, iterations);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        private static readonly byte[] DummySalt = RandomNumberGenerator.GetBytes(SaltSize);

        /// <summary>
        /// 账号不存在或不可登录时执行一次同等开销的 PBKDF2，使响应时间与真实校验一致，防止按耗时枚举账号。
        /// </summary>
        public static void DummyVerify(string passwordMd5)
        {
            Derive(passwordMd5 ?? string.Empty, DummySalt, Iterations);
        }

        private static byte[] Derive(string passwordMd5, byte[] salt, int iterations)
        {
            var input = Encoding.UTF8.GetBytes(passwordMd5.Trim().ToLowerInvariant());
            return Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, HashAlgorithmName.SHA256, HashSize);
        }
    }
}
