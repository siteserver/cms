using Microsoft.Extensions.Primitives;
using SiteServer.Plugin;
using SiteServer.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SiteServer.API.Common
{
    public partial class Request
    {
        public List<string> QueryKeys => _context.Request.Query.Keys.ToList();

        public bool IsQueryExists(string name)
        {
            return _context.Request.Query.ContainsKey(name);
        }

        public string GetQueryString(string name)
        {
            return _context.Request.Query[name];
        }

        public int GetQueryInt(string name, int defaultValue = 0)
        {
            return IsQueryExists(name)
                ? TranslateUtils.ToIntWithNagetive(GetQueryString(name))
                : defaultValue;
        }

        public decimal GetQueryDecimal(string name, decimal defaultValue = 0)
        {
            return IsQueryExists(name)
                ? TranslateUtils.ToDecimalWithNagetive(GetQueryString(name))
                : defaultValue;
        }

        public bool GetQueryBool(string name, bool defaultValue = false)
        {
            return IsQueryExists(name)
                ? TranslateUtils.ToBool(GetQueryString(name))
                : defaultValue;
        }

        public bool TryGetQuery(string name, out StringValues value)
        {
            return _context.Request.Query.TryGetValue(name, out value);
        }
    }
}
