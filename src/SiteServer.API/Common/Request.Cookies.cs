using Microsoft.AspNetCore.Http;
using SiteServer.Plugin;
using SiteServer.Utils;
using System;

namespace SiteServer.API.Common
{
    public partial class Request
    {
        public bool TryGetCookie(string key, out string value)
        {
            if (_context.Request.Cookies.TryGetValue(key, out value))
            {
                value = TranslateUtils.DecryptStringBySecretKey(value);
                return true;
            }
            return false;
        }

        public void SetCookie(Cookie cookie)
        {
            var option = new CookieOptions
            {
                Domain = cookie.Domain,
                Path = cookie.Path,
                Expires = cookie.Expires,
                Secure = cookie.Secure,
                HttpOnly = cookie.HttpOnly
            };

            var value = TranslateUtils.EncryptStringBySecretKey(cookie.Value);

            _context.Response.Cookies.Delete(cookie.Name);
            _context.Response.Cookies.Append(cookie.Name, value, option);
        }

        public bool RemoveCookie(string key)
        {
            _context.Response.Cookies.Delete(key);
            return true;
        }

        public void SetCookie(string name, string value, TimeSpan expiresAt)
        {
            SetCookie(new Cookie
            {
                Name = name,
                Value = value,
                Expires = DateTimeOffset.Now.Add(expiresAt),
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
    }
}
