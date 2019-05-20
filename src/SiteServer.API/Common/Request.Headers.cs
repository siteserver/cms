using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Common
{
    public partial class Request
    {
        public bool TryGetHeader(string key, out StringValues value)
        {
            return _context.Request.Headers.TryGetValue(key, out value);
        }

        public void SetHeader(string key, StringValues value)
        {
            _context.Response.Headers.Remove(key);
            _context.Response.Headers.Append(key, value);
        }

        public bool RemoveHeader(string key)
        {
            return _context.Response.Headers.Remove(key);
        }
    }
}
