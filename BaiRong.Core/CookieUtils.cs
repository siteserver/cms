using System;
using System.Web;

namespace BaiRong.Core
{
    public class CookieUtils
    {
        private CookieUtils()
        {
        }

        public static void SetCookie(string name, string value, DateTime expires)
        {
            var myCookie = new HttpCookie(name)
            {
                Value = TranslateUtils.EncryptStringBySecretKey(value),
                Expires = expires,
                HttpOnly = true
            };

            //防止通过js获取到cookie
            if (HttpContext.Current.Request.Url.Scheme.Equals("https"))
            {
                myCookie.Secure = true;//通过https传递cookie
            }

            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        public static void SetCookie(HttpCookie cookie)
        {
            cookie.Value = TranslateUtils.EncryptStringBySecretKey(cookie.Value);
            cookie.HttpOnly = true;//防止通过js获取到cookie
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
            var cookie = HttpContext.Current.Response.Cookies[name];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.Values.Clear();
            }
        }
    }
}
