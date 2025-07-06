using Sw.ChinaEncryptSM;
using System.Security.Cryptography;
using System;
using System.Text;

namespace SSCMS.Utils
{
    public class EncryptUtils
    {
        public static string GenerateSecurityKey()
        {
            var key = new byte[16];
            var iv = new byte[16];
            RandomNumberGenerator.Fill(key);
            RandomNumberGenerator.Fill(iv);

            string keyStr = Convert.ToHexString(key);
            string ivStr = Convert.ToHexString(iv);
            return $"{keyStr}.{ivStr}".ToLower();
        }

        public static (bool isValid, string key, string iv) IsValidSecurityKey(string securityKey)
        {
            if (string.IsNullOrEmpty(securityKey)) return (false, string.Empty, string.Empty);
            var list = securityKey.Split('.');
            if (list.Length != 2) return (false, string.Empty, string.Empty);
            var key = list[0];
            var iv = list[1];
            if (key.Length != 32 || iv.Length != 32) return (false, string.Empty, string.Empty);
            return (true, key, iv);
        }

        public static byte[] GetSecurityKeyBytes(string securityKey)
        {
            var (isValid, _, _) = IsValidSecurityKey(securityKey);
            if (!isValid)
            {
                securityKey = GenerateSecurityKey();
            }
            return Encoding.UTF8.GetBytes(securityKey);
        }

        public static string Encrypt(string plaintext, string securityKey)
        {
            var (isValid, key, iv) = IsValidSecurityKey(securityKey);
            if (!isValid)
            {
                return DesEncryptor.EncryptStringBySecretKey(plaintext, securityKey);
            }

            var sM4Utils = new SM4Utils()
            {
                secretKey = key,
                iv = iv
            };
            return sM4Utils.Encrypt_CBC_Hex(plaintext);
        }

        public static string Decrypt(string ciphertext, string securityKey)
        {
            var (isValid, key, iv) = IsValidSecurityKey(securityKey);
            if (!isValid)
            {
                return DesEncryptor.DecryptStringBySecretKey(ciphertext, securityKey);
            }

            var sM4Utils = new SM4Utils()
            {
                secretKey = key,
                iv = iv
            };
            return sM4Utils.Decrypt_CBC_Hex(ciphertext);
        }
    }
}
