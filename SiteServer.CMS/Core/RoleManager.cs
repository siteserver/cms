using System;
using System.Web;

namespace SiteServer.CMS.Core
{
    public static class RoleManager
    {
        public static void DeleteCookie()
        {
            var current = HttpContext.Current;
            if ((current != null) && current.Request.Browser.Cookies)
            {
                var text = string.Empty;
                if (current.Request.Browser["supportsEmptyStringInCookieValue"] == "false")
                {
                    text = "NoCookie";
                }
                var cookie = new HttpCookie(CookieName, text)
                {
                    Path = "/",
                    Domain = string.Empty,
                    Expires = new DateTime(0x7cf, 10, 12),
                    Secure = false
                };
                current.Response.Cookies.Remove(CookieName);
                current.Response.Cookies.Add(cookie);
            }
        }

        public const string CookieName = "BAIRONG.ROLES";
        public const int CookieTimeout = 90;
        public const string CookiePath = "/";
        public const bool CookieSlidingExpiration = true;
        public const int MaxCachedResults = 1000;
        public const string Domain = "";
        public const bool CreatePersistentCookie = true;
        public const bool CookieRequireSsl = false;
        public const bool CacheRolesInCookie = true;
    }
}
