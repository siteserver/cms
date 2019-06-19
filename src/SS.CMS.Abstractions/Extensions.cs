using Microsoft.AspNetCore.Http;

namespace SS.CMS
{
    public static class Extensions
    {
        public static string GetQueryString(this HttpRequest request, string name, string defaultValue = "")
        {
            return request.Query.TryGetValue(name, out var value) ? value.ToString() : defaultValue;
        }

        public static int GetQueryInt(this HttpRequest request, string name, int defaultValue = 0)
        {
            if (!request.Query.TryGetValue(name, out var value)) return defaultValue;
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public static decimal GetQueryDecimal(this HttpRequest request, string name, decimal defaultValue = 0)
        {
            if (!request.Query.TryGetValue(name, out var value)) return defaultValue;
            return decimal.TryParse(value, out var result) ? result : defaultValue;
        }

        public static bool GetQueryBool(this HttpRequest request, string name, bool defaultValue = false)
        {
            if (!request.Query.TryGetValue(name, out var value)) return defaultValue;
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}
