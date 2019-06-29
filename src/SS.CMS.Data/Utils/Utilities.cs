using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

[assembly: InternalsVisibleTo("SS.CMS.Data.Tests")]

namespace SS.CMS.Data.Utils
{
    internal static class Utilities
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"}
            },
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public static string JsonSerialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, JsonSettings);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static T JsonDeserialize<T>(string json, T defaultValue = default(T))
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, JsonSettings);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static object Get(IDictionary<string, object> dict, string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return dict.TryGetValue(name, out var extendValue) ? extendValue : null;
        }

        public static T Get<T>(object value, T defaultValue = default(T))
        {
            switch (value)
            {
                case null:
                    return defaultValue;
                case T variable:
                    return variable;
                default:
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        return defaultValue;
                    }
            }
        }

        public static Dictionary<string, object> ToDictionary(string json)
        {
            var dict = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(json)) return dict;

            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                dict = JsonDeserialize<Dictionary<string, object>>(json);
                return dict;
            }

            json = json.Replace("/u0026", "&");

            var pairs = json.Split('&');
            foreach (var pair in pairs)
            {
                if (pair.IndexOf("=", StringComparison.Ordinal) == -1) continue;
                var name = pair.Split('=')[0];
                if (string.IsNullOrEmpty(name)) continue;

                name = name.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+").Replace("_return_", "\r").Replace("_newline_", "\n");
                var value = pair.Split('=')[1];
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+").Replace("_return_", "\r").Replace("_newline_", "\n");
                }

                dict[name] = value;
            }

            return dict;
        }

        public static IList<string> StringCollectionToStringList(string collection, char split = ',')
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(collection)) return list;

            var array = collection.Split(split);
            list.AddRange(array);
            return list;
        }

        public static bool ToBool(string boolStr)
        {
            if (!bool.TryParse(boolStr?.Trim(), out var boolean))
            {
                boolean = false;
            }
            return boolean;
        }

        public static decimal ToDecimal(string intStr, decimal defaultValue = 0)
        {
            if (!decimal.TryParse(intStr?.Trim(), out var i))
            {
                i = defaultValue;
            }
            if (i < 0)
            {
                i = defaultValue;
            }
            return i;
        }

        public static bool IsGuid(string val)
        {
            return !string.IsNullOrWhiteSpace(val) && Guid.TryParse(val, out _);
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;

            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(string text, string inner)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(inner)) return false;
            return text.ToLower().IndexOf(inner.ToLower(), StringComparison.Ordinal) >= 0;
        }

        public static string TrimAndToUpper(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.ToUpper().Trim();
        }

        public static string TrimAndToLower(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.ToLower().Trim();
        }

        public static bool ContainsIgnoreCase(IList<string> list, string target)
        {
            if (list == null || list.Count == 0) return false;

            return list.Any(element => EqualsIgnoreCase(element, target));
        }

        public static string GetConnectionStringDatabase(string connectionString)
        {
            foreach (var pair in StringCollectionToStringList(connectionString, ';'))
            {
                if (string.IsNullOrEmpty(pair) || pair.IndexOf("=", StringComparison.Ordinal) == -1) continue;
                var key = pair.Substring(0, pair.IndexOf("=", StringComparison.Ordinal));
                var value = pair.Substring(pair.IndexOf("=", StringComparison.Ordinal) + 1);
                if (EqualsIgnoreCase(key, "Database") ||
                    EqualsIgnoreCase(key, "Data Source") ||
                    EqualsIgnoreCase(key, "Initial Catalog"))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        public static string GetConnectionStringUserName(string connectionString)
        {
            foreach (var pair in StringCollectionToStringList(connectionString, ';'))
            {
                if (string.IsNullOrEmpty(pair) || pair.IndexOf("=", StringComparison.Ordinal) == -1) continue;
                var key = pair.Substring(0, pair.IndexOf("=", StringComparison.Ordinal));
                var value = pair.Substring(pair.IndexOf("=", StringComparison.Ordinal) + 1);
                if (EqualsIgnoreCase(key, "Uid") ||
                    EqualsIgnoreCase(key, "Username") ||
                    EqualsIgnoreCase(key, "User ID"))
                {
                    return value;
                }
            }

            return string.Empty;
        }
    }
}
