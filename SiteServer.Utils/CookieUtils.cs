using System;
using System.Web;

namespace SiteServer.Utils
{
    public static class CookieUtils
    {
        public static void SetCookie(string name, string value, DateTime expires, bool isEncrypt = true)
        {
            SetCookie(new HttpCookie(name)
            {
                Value = value,
                Expires = expires,
                Domain = PageUtils.HttpContextRootDomain
            }, isEncrypt);
        }

        public static void SetCookie(string name, string value, bool isEncrypt = true)
        {
            SetCookie(new HttpCookie(name)
            {
                Value = value,
                Domain = PageUtils.HttpContextRootDomain
            }, isEncrypt);
        }

        private static void SetCookie(HttpCookie cookie, bool isEncrypt)
        {
            cookie.Value = isEncrypt ? TranslateUtils.EncryptStringBySecretKey(cookie.Value) : cookie.Value;
            cookie.HttpOnly = false;

            if (HttpContext.Current.Request.Url.Scheme.Equals("https"))
            {
                cookie.Secure = true;//通过https传递cookie
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static string GetCookie(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] == null) return string.Empty;

            var value = HttpContext.Current.Request.Cookies[name].Value;
            return TranslateUtils.DecryptStringBySecretKey(value);
        }

        public static bool IsExists(string name)
        {
            return HttpContext.Current.Request.Cookies[name] != null;
        }

        public static void Erase(string name)
        {
            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                SetCookie(name, string.Empty, DateTime.Now.AddDays(-1d));
            }
        }
    }
}
