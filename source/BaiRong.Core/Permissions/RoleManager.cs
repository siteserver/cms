using System;
using System.Web;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Permissions
{
    public class RoleManager
    {
        private RoleManager() { }

        public static void CreatePredefinedRoles()
        {
            var allPredefinedRoles = EPredefinedRoleUtils.GetAllPredefinedRole();
            foreach (EPredefinedRole enumRole in allPredefinedRoles)
            {
                BaiRongDataProvider.RoleDao.InsertRole(EPredefinedRoleUtils.GetValue(enumRole), string.Empty, EPredefinedRoleUtils.GetText(enumRole));
            }
        }

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
                var cookie = new HttpCookie(CookieName, text);
                cookie.Path = "/";
                cookie.Domain = string.Empty;
                cookie.Expires = new DateTime(0x7cf, 10, 12);
                cookie.Secure = false;
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
