using System;
using System.Web;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request
    {
        public bool TryGetCookie(string key, out string value)
        {
            value = null;
            if (_httpContext.Request.Cookies[key] == null) return false;

            value = _httpContext.Request.Cookies[key].Value;
            value = TranslateUtils.DecryptStringBySecretKey(value);
            return true;
        }

        public void SetCookie(Cookie cookie)
        {
            var value = TranslateUtils.EncryptStringBySecretKey(cookie.Value);
            _httpContext.Response.Cookies.Add(new HttpCookie(cookie.Name)
            {
                Path = cookie.Path,
                Secure = cookie.Secure,
                HttpOnly = cookie.HttpOnly,
                Domain = cookie.Domain,
                Expires = TranslateUtils.ToDateTime(cookie.Expires),
                Value = value
            });
        }

        public bool RemoveCookie(string key)
        {
            if (_httpContext.Request.Cookies[key] != null)
            {
                _httpContext.Request.Cookies.Remove(key);

                SetCookie(new Cookie
                {
                    Name = key,
                    Value = string.Empty,
                    Expires = DateTime.Now.AddDays(-1d),
                    Domain = Host
                });
                return true;
            }
            return false;
        }

        //public string GetCookie(string name)
        //{

        //}

        //public void DeleteCookie(string name)
        //{
        //    if (HttpContext.Current.Request.Cookies[name] != null)
        //    {
        //        SetCookie(name, string.Empty, DateTime.Now.AddDays(-1d));
        //    }
        //}

        //public void SetCookie(string name, string value)
        //{
        //    SetCookie(name, value);
        //}

        //public void SetCookie(string name, string value, TimeSpan expiresAt)
        //{
        //    SetCookie(name, value, expiresAt);
        //}

        public void SetCookie(string name, string value, TimeSpan expiresAt)
        {
            SetCookie(new Cookie
            {
                Name = name,
                Value = value,
                Expires = TranslateUtils.ToDateTimeOffset(DateTime.Now.Add(expiresAt)),
                Domain = Host
            });
        }

        public void SetCookie(string name, string value, DateTime expires)
        {
            SetCookie(new Cookie
            {
                Name = name,
                Value = value,
                Expires = TranslateUtils.ToDateTimeOffset(expires),
                Domain = Host
            });
        }

        public void SetCookie(string name, string value)
        {
            SetCookie(new Cookie
            {
                Name = name,
                Value = value,
                Domain = Host
            });
        }

        //private void SetCookie(HttpCookie cookie, bool isEncrypt)
        //{
        //    cookie.Value = isEncrypt ? TranslateUtils.EncryptStringBySecretKey(cookie.Value) : cookie.Value;
        //    cookie.HttpOnly = false;

        //    if (HttpContext.Current.Request.Url.Scheme.Equals("https"))
        //    {
        //        cookie.Secure = true;//通过https传递cookie
        //    }
        //    HttpContext.Current.Response.Cookies.Add(cookie);
        //}

        //public bool IsCookieExists(string name)
        //{
        //    return HttpContext.Current.Request.Cookies[name] != null;
        //}
    }
}