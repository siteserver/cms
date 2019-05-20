using Microsoft.Extensions.Primitives;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request
    {
        public bool TryGetHeader(string key, out StringValues value)
        {
            value = StringValues.Empty;
            if (_owinContext.Request.Headers.TryGetValue(key, out var values))
            {
                value = values;
                return true;
            }

            return false;
        }

        public void SetHeader(string key, StringValues value)
        {
            _owinContext.Response.Headers.Remove(key);
            _owinContext.Response.Headers.Append(key, value);
        }

        public bool RemoveHeader(string key)
        {
            return _owinContext.Response.Headers.Remove(key);
        }
    }
}