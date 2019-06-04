using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using SiteServer.Plugin;
using HttpContext = System.Web.HttpContext;

namespace SiteServer.BackgroundPages.Common
{
    public class ResponseCookies : IResponseCookies
    {
        private readonly HttpCookieCollection _cookies;
        public ResponseCookies(HttpCookieCollection cookies)
        {
            _cookies = cookies;
        }

        public void Append(string key, string value)
        {
            _cookies.Add(new HttpCookie(key, value));
        }

        public void Append(string key, string value, CookieOptions options)
        {
            var cookie = new HttpCookie(key, value);
            if (options != null)
            {
                if (options.Domain != null) cookie.Domain = options.Domain;
                if (options.Expires != null) cookie.Expires = options.Expires.Value.DateTime;
                if (options.Path != null) cookie.Path = options.Path;
                cookie.HttpOnly = options.HttpOnly;
                cookie.Secure = options.Secure;
            }
            _cookies.Add(new HttpCookie(key, value));
        }

        public void Delete(string key)
        {
            _cookies.Remove(key);
        }

        public void Delete(string key, CookieOptions options)
        {
            _cookies.Remove(key);
        }
    }
}
