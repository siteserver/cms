using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SiteServer.Utils.Auth
{
    public static class AuthUtils
    {
        public static string GetAdminTokenByAdminName(string administratorName, DateTime addDate)
        {
            if (string.IsNullOrEmpty(administratorName)) return null;

            var administratorToken = new AdministratorToken()
            {
                AdministratorName = administratorName,
                AddDate = addDate
            };

            return JsonWebToken.Encode(administratorToken, WebConfigUtils.SecretKey, JwtHashAlgorithm.HS256);
        }

        public static string GetUserTokenByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userToken = new UserToken
            {
                UserName = userName,
                AddDate = DateTime.Now
            };

            return JsonWebToken.Encode(userToken, WebConfigUtils.SecretKey, JwtHashAlgorithm.HS256);
        }

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
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
