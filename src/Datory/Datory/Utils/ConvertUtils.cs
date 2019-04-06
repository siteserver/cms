using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

[assembly: InternalsVisibleTo("Datory.Tests")]

namespace Datory.Utils
{
    internal static class ConvertUtils
    {
        

        // ------------------- transateutis ----------------------------

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

        private static T JsonDeserialize<T>(string json, T defaultValue = default(T))
        {
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

        //public static T Get<T>(IDictionary<string, object> dict, string name, T defaultValue)
        //{
        //    return Get(Get(dict, name), defaultValue);
        //}

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

        //public static int ToInt(string intStr, int defaultValue = 0)
        //{
        //    if (!int.TryParse(intStr?.Trim().TrimStart('0'), out var i))
        //    {
        //        i = defaultValue;
        //    }
        //    if (i < 0)
        //    {
        //        i = defaultValue;
        //    }
        //    return i;
        //}

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

        // ------------------- transateutis ----------------------------

        // ------------------- stringutis ----------------------------

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

        // ------------------- stringutis ----------------------------

        /// <summary>
        /// 获取文件的文件夹路径，如果path为文件夹，返回自身。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        private static string GetDirectoryPath(string path)
        {
            var ext = Path.GetExtension(path);
            var directoryPath = !string.IsNullOrEmpty(ext) ? Path.GetDirectoryName(path) : path;
            return directoryPath;
        }

        private static bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        private static void CreateDirectoryIfNotExists(string path)
        {
            var directoryPath = GetDirectoryPath(path);

            if (!IsDirectoryExists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch
                {
                    //Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                    //string[] directoryNames = directoryPath.Split('\\');
                    //string thePath = directoryNames[0];
                    //for (int i = 1; i < directoryNames.Length; i++)
                    //{
                    //    thePath = thePath + "\\" + directoryNames[i];
                    //    if (StringUtils.Contains(thePath.ToLower(), ConfigUtils.Instance.PhysicalApplicationPath.ToLower()) && !IsDirectoryExists(thePath))
                    //    {
                    //        fso.CreateFolder(thePath);
                    //    }
                    //}                    
                }
            }
        }

        public static async Task AppendTextAsync(string filePath, string content)
        {
            CreateDirectoryIfNotExists(filePath);

            var file = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                await writer.WriteAsync(content);
                writer.Flush();
                writer.Close();

                file.Close();
            }
        }
    }
}
