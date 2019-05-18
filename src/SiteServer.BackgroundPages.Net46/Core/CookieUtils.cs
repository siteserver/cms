using System;
using System.Text.RegularExpressions;
using System.Web;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Core
{
    public static class CookieUtils
    {
        private static string HttpContextRootDomain
        {
            get
            {
                var url = HttpContext.Current.Request.Url;

                if (url.HostNameType != UriHostNameType.Dns) return url.Host;

                var match = Regex.Match(url.Host, "([^.]+\\.[^.]{1,3}(\\.[^.]{1,3})?)$");
                return match.Groups[1].Success ? match.Groups[1].Value : null;
            }
        }

        public static void SetCookie(string name, string value, TimeSpan expiresAt, bool isEncrypt = true)
        {
            SetCookie(new System.Web.HttpCookie(name)
            {
                Value = value,
                Expires = DateUtils.GetExpiresAt(expiresAt),
                Domain = HttpContextRootDomain
            }, isEncrypt);
        }

        public static void SetCookie(string name, string value, DateTime expires, bool isEncrypt = true)
        {
            SetCookie(new System.Web.HttpCookie(name)
            {
                Value = value,
                Expires = expires,
                Domain = HttpContextRootDomain
            }, isEncrypt);
        }

        public static void SetCookie(string name, string value, bool isEncrypt = true)
        {
            SetCookie(new System.Web.HttpCookie(name)
            {
                Value = value,
                Domain = HttpContextRootDomain
            }, isEncrypt);
        }

        private static void SetCookie(System.Web.HttpCookie cookie, bool isEncrypt)
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
