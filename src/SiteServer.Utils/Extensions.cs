using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace SiteServer.Utils
{
    public static class Extensions
    {
        public static bool IsDefault<T>(this T value) where T : struct
        {
            return value.Equals(default(T));
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null || items == null) return;

            if (list is List<T>)
            {
                ((List<T>)list).AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Returns an individual HTTP Header value
        /// </summary>
        public static string GetHeader(this HttpRequestMessage request, string key)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            return !request.Headers.TryGetValues(key, out var keys) ? null : keys.First();
        }

        /// <summary>
        /// Retrieves an individual cookie from the cookies collection
        /// </summary>
        public static string GetCookie(this HttpRequestMessage request, string cookieName)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            var value = cookie?[cookieName].Value;
            return TranslateUtils.DecryptStringBySecretKey(value);
        }
        
        public static IDictionary<string, string> GetQueryDirectory(this HttpRequestMessage request)
        {
            IDictionary<string, string> dict = null;
            if (request.Properties.ContainsKey("QueryDictionary"))
            {
                dict = TranslateUtils.Get<IDictionary<string, string>>(request.Properties, "QueryDictionary");
            }

            if (dict != null) return dict;

            dict = request.GetQueryNameValuePairs()
                .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

            request.Properties["QueryDictionary"] = dict;

            return dict;
        }

        public static bool IsQueryExists(this HttpRequestMessage request, string name)
        {
            var dict = request.GetQueryDirectory();
            return dict.ContainsKey(name);
        }

        public static string GetQueryString(this HttpRequestMessage request, string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var dict = request.GetQueryDirectory();
            return dict.TryGetValue(name, out var value) ? value : null;
        }

        public static int GetQueryInt(this HttpRequestMessage request, string name, int defaultValue = 0)
        {
            return TranslateUtils.ToIntWithNegative(request.GetQueryString(name), defaultValue);
        }

        public static decimal GetQueryDecimal(this HttpRequestMessage request, string name, decimal defaultValue = 0)
        {
            return TranslateUtils.ToDecimalWithNegative(request.GetQueryString(name), defaultValue);
        }

        public static bool GetQueryBool(this HttpRequestMessage request, string name, bool defaultValue = false)
        {
            return TranslateUtils.ToBool(request.GetQueryString(name), false);
        }
        
        public static IDictionary<string, object> GetPostDictionary(this HttpRequestMessage request)
        {
            IDictionary<string, object> dict = null;
            if (request.Properties.ContainsKey("PostDictionary"))
            {
                dict = TranslateUtils.Get<IDictionary<string, object>>(request.Properties, "PostDictionary");
            }

            if (dict != null) return dict;

            dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            var json = request.Content.ReadAsStringAsync().Result;
            if (string.IsNullOrEmpty(json)) return dict;

            var originalDict = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(json);
            if (originalDict != null)
            {
                foreach (var key in originalDict.Keys)
                {
                    dict[key] = originalDict[key];
                }
            }

            request.Properties["PostDictionary"] = dict;

            return dict;
        }

        public static T GetPostObject<T>(this HttpRequestMessage request)
        {
            var json = request.Content.ReadAsStringAsync().Result;
            return TranslateUtils.JsonDeserialize<T>(json);
        }

        public static T GetPostObject<T>(this HttpRequestMessage request, string name)
        {
            var json = request.GetPostString(name);
            return TranslateUtils.JsonDeserialize<T>(json);
        }

        public static bool IsPostExists(this HttpRequestMessage request, string name)
        {
            var dict = request.GetPostDictionary();
            return dict.ContainsKey(name);
        }

        public static object GetPostObject(this HttpRequestMessage request, string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var dict = request.GetPostDictionary();

            return dict.TryGetValue(name, out var value) ? value : null;
        }

        public static string GetPostString(this HttpRequestMessage request, string name)
        {
            var value = request.GetPostObject(name);
            switch (value)
            {
                case null:
                    return null;
                case string s:
                    return s;
                default:
                    return value.ToString();
            }
        }

        public static int GetPostInt(this HttpRequestMessage request, string name, int defaultValue = 0)
        {
            var value = request.GetPostObject(name);
            switch (value)
            {
                case null:
                    return 0;
                case int _:
                    return (int)value;
                default:
                    return TranslateUtils.ToIntWithNegative(value.ToString(), defaultValue);
            }
        }

        public static decimal GetPostDecimal(this HttpRequestMessage request, string name, decimal defaultValue = 0)
        {
            var value = request.GetPostObject(name);
            switch (value)
            {
                case null:
                    return 0;
                case decimal _:
                    return (decimal)value;
                default:
                    return TranslateUtils.ToDecimalWithNegative(value.ToString(), defaultValue);
            }
        }

        public static bool GetPostBool(this HttpRequestMessage request, string name, bool defaultValue = false)
        {
            var value = request.GetPostObject(name);
            switch (value)
            {
                case null:
                    return false;
                case bool _:
                    return (bool)value;
                default:
                    return TranslateUtils.ToBool(value.ToString(), defaultValue);
            }
        }

        public static DateTime GetPostDateTime(this HttpRequestMessage request, string name, DateTime defaultValue)
        {
            var value = request.GetPostObject(name);
            switch (value)
            {
                case null:
                    return DateTime.Now;
                case DateTime _:
                    return (DateTime)value;
                default:
                    return TranslateUtils.ToDateTime(value.ToString(), defaultValue);
            }
        }

        public static string GetApiToken(this HttpRequestMessage request)
        {
            var accessTokenStr = string.Empty;
            if (!string.IsNullOrEmpty(request.GetHeader(Constants.AuthKeyApiHeader)))
            {
                accessTokenStr = request.GetHeader(Constants.AuthKeyApiHeader);
            }
            else if (request.IsQueryExists(Constants.AuthKeyApiQuery))
            {
                accessTokenStr = request.GetQueryString(Constants.AuthKeyApiQuery);
            }
            else if (!string.IsNullOrEmpty(request.GetCookie(Constants.AuthKeyApiCookie)))
            {
                accessTokenStr = request.GetCookie(Constants.AuthKeyApiCookie);
            }

            if (StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
            {
                accessTokenStr = TranslateUtils.DecryptStringBySecretKey(accessTokenStr);
            }

            return accessTokenStr;
        }

        public static string GetUserToken(this HttpRequestMessage request)
        {
            var accessTokenStr = string.Empty;
            if (!string.IsNullOrEmpty(request.GetCookie(Constants.AuthKeyUserCookie)))
            {
                accessTokenStr = request.GetCookie(Constants.AuthKeyUserCookie);
            }
            else if (!string.IsNullOrEmpty(request.GetHeader(Constants.AuthKeyUserHeader)))
            {
                accessTokenStr = request.GetHeader(Constants.AuthKeyUserHeader);
            }
            else if (request.IsQueryExists(Constants.AuthKeyUserQuery))
            {
                accessTokenStr = request.GetQueryString(Constants.AuthKeyUserQuery);
            }

            if (StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
            {
                accessTokenStr = TranslateUtils.DecryptStringBySecretKey(accessTokenStr);
            }

            return accessTokenStr;
        }

        public static string GetAdminToken(this HttpRequestMessage request)
        {
            var accessTokenStr = string.Empty;
            if (!string.IsNullOrEmpty(request.GetCookie(Constants.AuthKeyAdminCookie)))
            {
                accessTokenStr = request.GetCookie(Constants.AuthKeyAdminCookie);
            }
            else if (!string.IsNullOrEmpty(request.GetHeader(Constants.AuthKeyAdminHeader)))
            {
                accessTokenStr = request.GetHeader(Constants.AuthKeyAdminHeader);
            }
            else if (request.IsQueryExists(Constants.AuthKeyAdminQuery))
            {
                accessTokenStr = request.GetQueryString(Constants.AuthKeyAdminQuery);
            }

            if (StringUtils.EndsWith(accessTokenStr, TranslateUtils.EncryptStingIndicator))
            {
                accessTokenStr = TranslateUtils.DecryptStringBySecretKey(accessTokenStr);
            }

            return accessTokenStr;
        }
    }
}
