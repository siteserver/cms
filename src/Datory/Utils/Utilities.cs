using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

[assembly: InternalsVisibleTo("Datory.Tests")]

namespace Datory.Utils
{
    public static class Utilities
    {
        public static List<int> GetIntList(string collection, char separator = ',')
        {
            var list = new List<int>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(separator);
                foreach (var s in array)
                {
                    int.TryParse(s.Trim(), out var i);
                    list.Add(i);
                }
            }
            return list;
        }

        public static List<int> GetIntList(IEnumerable<int> collection)
        {
            return collection == null ? new List<int>() : new List<int>(collection);
        }

        public static List<int> GetIntList(JArray jArray)
        {
            try
            {
                return jArray == null ? new List<int>() : jArray.ToObject<List<int>>();
            }
            catch
            {
                return new List<int>();
            }
        }

        public static string ToString(List<string> objects, string separator = ",")
        {
            return objects != null && objects.Count > 0 ? string.Join(separator, objects) : string.Empty;
        }

        public static string ToString(List<int> objects, string separator = ",")
        {
            return objects != null && objects.Count > 0 ? string.Join(separator, objects) : string.Empty;
        }

        public static string ToString(List<object> objects, string separator = ",")
        {
            return objects != null && objects.Count > 0 ? string.Join(separator, objects) : string.Empty;
        }

        public static List<string> GetStringList(string collection, char split = ',')
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(collection))
            {
                var array = collection.Split(split);
                foreach (var s in array)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        list.Add(s);
                    }
                }
            }
            return list;
        }

        public static List<string> GetStringList(IEnumerable<string> collection)
        {
            return collection == null ? new List<string>() : new List<string>(collection);
        }

        public static List<string> GetStringList(JArray jArray)
        {
            try
            {
                return jArray == null ? new List<string>() : jArray.ToObject<List<string>>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"}
            }
        };

        public static string JsonSerialize(object obj)
        {
            if (obj == null) return string.Empty;

            try
            {
                //var settings = new JsonSerializerSettings
                //{
                //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                //};
                //var timeFormat = new IsoDateTimeConverter {DateTimeFormat = "yyyy-MM-dd HH:mm:ss"};
                //settings.Converters.Add(timeFormat);

                return JsonConvert.SerializeObject(obj, JsonSettings);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static T JsonDeserialize<T>(string json, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(json)) return defaultValue;

            try
            {
                //var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                //var timeFormat = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                //settings.Converters.Add(timeFormat);

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

            try
            {
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
            }
            catch
            {
                // ignored
            }

            return dict;
        }

        public static int ToInt(string intStr, int defaultValue = 0)
        {
            if (!int.TryParse(intStr?.Trim().TrimStart('0'), out var i))
            {
                i = defaultValue;
            }
            if (i < 0)
            {
                i = defaultValue;
            }
            return i;
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

        public static bool IsExtend(string propertyName)
        {
            return EqualsIgnoreCase(propertyName, "ExtendValues");
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

        public static bool ContainsIgnoreCase(IEnumerable<string> list, string target)
        {
            if (list == null) return false;

            return list.Any(element => EqualsIgnoreCase(element, target));
        }

        public static string GetConnectionStringDatabase(string connectionString)
        {
            foreach (var pair in GetStringList(connectionString, ';'))
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
            foreach (var pair in GetStringList(connectionString, ';'))
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

        public static (string Host, int Port, string Password, int Database, bool AllowAdmin) GetRedisConnectionString(string connectionString)
        {
            var host = "localhost";
            var post = 6379;
            var password = string.Empty;
            var database = 0;
            var allowAdmin = true;

            foreach (var pair in GetStringList(connectionString))
            {
                if (string.IsNullOrEmpty(pair)) continue;

                if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                {
                    var key = pair.Substring(0, pair.IndexOf("=", StringComparison.Ordinal));
                    var value = pair.Substring(pair.IndexOf("=", StringComparison.Ordinal) + 1);

                    if (EqualsIgnoreCase(key, "password"))
                    {
                        password = value;
                    }
                    else if (EqualsIgnoreCase(key, "allowAdmin"))
                    {
                        allowAdmin = ToBool(value);
                    }
                    else if (EqualsIgnoreCase(key, "database"))
                    {
                        database = ToInt(value);
                    }
                }
                else if (pair.IndexOf(":", StringComparison.Ordinal) != -1)
                {
                    host = pair.Substring(0, pair.IndexOf(":", StringComparison.Ordinal));
                    post = ToInt(pair.Substring(pair.IndexOf(":", StringComparison.Ordinal) + 1));
                }
                else
                {
                    host = pair;
                }
            }

            return (host, post, password, database, allowAdmin);
        }

        public static T ToEnum<T>(string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
        }

        public static string ReplaceEndsWith(string input, string replace, string to)
        {
            var retVal = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.EndsWith(replace))
            {
                retVal = input.Substring(0, input.Length - replace.Length) + to;
            }
            return retVal;
        }

        public static string ReplaceEndsWithIgnoreCase(string input, string replace, string to)
        {
            var retVal = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.ToLower().EndsWith(replace.ToLower()))
            {
                retVal = input.Substring(0, input.Length - replace.Length) + to;
            }
            return retVal;
        }

        public static string GetMd5Hash(string source)
        {
            using var md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
