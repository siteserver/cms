using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SiteServer.Utils.Auth
{
    public static class AuthUtils
    {
        public static string Md5ByFilePath(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static string Md5ByString(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(str);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
