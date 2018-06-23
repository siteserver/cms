using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SiteServer.CMS.Api.V1
{
    public static class OUtils
    {
        public static Dictionary<string, string> DemoOptions = new Dictionary<string, string>
        {
            {"$select", "ID,Name"},
            {"$expand", "ProductDetail"},
            {"$filter", "Categories/any(d:d/ID gt 1)"},
            {"$orderby", "ID desc"},
            {"$top", "10"},
            {"$skip", "20"},
            {"$count", "true"},
            {"$search", "tom"}
        };

        private const string StrRegex = @"(?<Filter>" +
                                        "\n" + @"     (?<Resource>.+?)\s+" +
                                        "\n" + @"     (?<Operator>eq|ne|gt|ge|lt|le|add|sub|mul|div|mod|in)\s+" +
                                        "\n" + @"     '?(?<Value>.+?)'?" +
                                        "\n" + @")" +
                                        "\n" + @"(?:" +
                                        "\n" + @"    \s*$" +
                                        "\n" + @"   |\s+(?:or|and|not)\s+" +
                                        "\n" + @")" +
                                        "\n";

        public static OFilter ParseFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) return null;

            var oFilter = new OFilter();

            try
            {
                var regex = new Regex(StrRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                var mc = regex.Matches(filter);
                foreach (Match m in mc)
                {
                    oFilter.Resource = m.Groups["Resource"].Value;
                    oFilter.Operator = m.Groups["Operator"].Value;
                    oFilter.Value = m.Groups["Value"].Value;
                }
            }
            catch
            {
                // ignored
            }

            return !string.IsNullOrEmpty(oFilter.Resource) && !string.IsNullOrEmpty(oFilter.Operator) && !string.IsNullOrEmpty(oFilter.Value) ? oFilter : null;
        }
    }
}