using System;
using Microsoft.AspNetCore.Http;
using SiteServer.Abstractions;

namespace SiteServer.Web.Core
{
    public partial class AuthManager
    {
        public string SessionId
        {
            get
            {
                if (_context.Request.Cookies.TryGetValue(Constants.AuthKeySessionId, out var sessionId))
                {
                    return sessionId;
                }

                long i = 1;
                foreach (var b in Guid.NewGuid().ToByteArray())
                {
                    i *= b + 1;
                }
                sessionId = $"{i - DateTime.Now.Ticks:x}";

                _context.Response.Cookies.Delete(Constants.AuthKeySessionId);
                _context.Response.Cookies.Append(Constants.AuthKeySessionId, sessionId, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(100)
                });

                return sessionId;
            }
        }
    }
}