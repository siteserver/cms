using Microsoft.Extensions.Primitives;
using SiteServer.Plugin;
using SiteServer.Utils;
using System.Collections.Generic;
using System.Linq;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request
    {
        public IList<string> QueryKeys => _httpContext.Request.QueryString.AllKeys.ToList();

        public bool IsQueryExists(string name)
        {
            return _httpContext.Request.QueryString[name] != null;
        }

        public string GetQueryString(string name)
        {
            return !string.IsNullOrEmpty(_httpContext.Request.QueryString[name])
                ? AttackUtils.FilterSql(_httpContext.Request.QueryString[name])
                : null;
        }

        public int GetQueryInt(string name, int defaultValue = 0)
        {
            return !string.IsNullOrEmpty(_httpContext.Request.QueryString[name])
                ? TranslateUtils.ToIntWithNagetive(_httpContext.Request.QueryString[name])
                : defaultValue;
        }

        public decimal GetQueryDecimal(string name, decimal defaultValue = 0)
        {
            return !string.IsNullOrEmpty(_httpContext.Request.QueryString[name])
                ? TranslateUtils.ToDecimalWithNagetive(_httpContext.Request.QueryString[name])
                : defaultValue;
        }

        public bool GetQueryBool(string name, bool defaultValue = false)
        {
            var str = _httpContext.Request.QueryString[name];
            return !string.IsNullOrEmpty(str) ? TranslateUtils.ToBool(str) : defaultValue;
        }

        public bool TryGetQuery(string name, out StringValues value)
        {
            if (IsQueryExists(name))
            {
                value = _httpContext.Request.QueryString.GetValues(name);
                return true;
            }
            return false;
        }
    }
}
