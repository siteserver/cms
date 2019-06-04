using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace SiteServer.BackgroundPages.Common
{
    public class RequestCookieCollection : IRequestCookieCollection
    {
        private readonly HttpCookieCollection _cookies;

        public RequestCookieCollection(HttpCookieCollection cookies)
        {
            _cookies = cookies;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _cookies.AllKeys.Select(cookiesKey => new KeyValuePair<string, string>(cookiesKey, _cookies[cookiesKey]?.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cookies.GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return _cookies.AllKeys.Contains(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            value = null;
            if (_cookies[key] == null) return false;

            value = _cookies[key].Value;
            return true;
        }

        public int Count => _cookies.Count;

        public ICollection<string> Keys => _cookies.AllKeys;

        public string this[string key] => _cookies[key]?.Value;
    }
}
