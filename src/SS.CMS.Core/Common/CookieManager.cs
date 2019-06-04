using System;
using Microsoft.AspNetCore.Http;
using SS.CMS.Plugin;

namespace SS.CMS.Core.Common
{
    public static class CookieManager
    {
        public const string AdminToken = "SS-ADMIN-TOKEN";
        public const string UserToken = "SS-USER-TOKEN";
        public const string ApiKey = "SS-API-KEY";

        public static void Set(string key, IResponse response, string accessToken, DateTimeOffset? expires)
        {
            response.Cookies.Delete(key);
            response.Cookies.Append(key, accessToken, new CookieOptions
            {
                Expires = expires
            });
        }

        public static void Set(string key, IResponse response, string accessToken)
        {
            response.Cookies.Delete(key);
            response.Cookies.Append(key, accessToken);
        }

        public static void Delete(string key, IResponse response)
        {
            response.Cookies.Delete(key);
        }

        public static bool TryGet(string key, IRequest request, out string accessToken)
        {
            return request.Cookies.TryGetValue(key, out accessToken);
        }
    }
}
