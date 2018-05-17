using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
