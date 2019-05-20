using SiteServer.Plugin;
using SiteServer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request
    {
        private Dictionary<string, object> _postData;

        public Dictionary<string, object> PostData
        {
            get
            {
                if (_postData != null) return _postData;

                string json;
                using (var bodyStream = new StreamReader(_httpContext.Request.InputStream))
                {
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    json = bodyStream.ReadToEnd();
                }

                _postData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                if (string.IsNullOrEmpty(json)) return _postData;

                var dict = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(json);
                foreach (var key in dict.Keys)
                {
                    _postData[key] = dict[key];
                }

                return _postData;
            }
        }

        public List<string> PostKeys => PostData.Keys.ToList();

        public bool IsPostExists(string name)
        {
            return PostData.ContainsKey(name);
        }

        public object GetPostObject(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return PostData.TryGetValue(name, out var value) ? value : null;
        }

        public string GetPostString(string name)
        {
            var value = GetPostObject(name);
            if (value == null) return null;
            if (value is string) return (string)value;
            return value.ToString();
        }

        public int GetPostInt(string name, int defaultValue = 0)
        {
            var value = GetPostObject(name);
            if (value == null) return defaultValue;
            if (value is int) return (int)value;
            return TranslateUtils.ToIntWithNagetive(value.ToString(), defaultValue);
        }

        public decimal GetPostDecimal(string name, decimal defaultValue = 0)
        {
            var value = GetPostObject(name);
            if (value == null) return defaultValue;
            if (value is decimal) return (decimal)value;
            return TranslateUtils.ToDecimalWithNagetive(value.ToString(), defaultValue);
        }

        public bool GetPostBool(string name, bool defaultValue = false)
        {
            var value = GetPostObject(name);
            if (value == null) return defaultValue;
            if (value is bool) return (bool)value;
            return TranslateUtils.ToBool(value.ToString(), defaultValue);
        }

        public DateTime GetPostDateTime(string name, DateTime defaultValue)
        {
            var value = GetPostObject(name);
            if (value == null) return defaultValue;
            if (value is DateTime) return (DateTime)value;
            return TranslateUtils.ToDateTime(value.ToString(), defaultValue);
        }

        public bool TryGetPost<T>(string name, out T value)
        {
            try
            {
                value = GetPostObject<T>(name);
                return value != null;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public bool TryGetPost<T>(out T value)
        {
            return TryGetPost(string.Empty, out value);
        }

        public T GetPostObject<T>(string name = "")
        {
            string json;
            if (string.IsNullOrEmpty(name))
            {
                using (var bodyStream = new StreamReader(_httpContext.Request.InputStream))
                {
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    json = bodyStream.ReadToEnd();
                }
            }
            else
            {
                json = GetPostString(name);
            }

            return TranslateUtils.JsonDeserialize<T>(json);
        }
    }
}
