using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web.UI;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    /// <summary>
    /// Provides standard implementation for simple extendent data storage
    /// </summary>
    [Serializable]
    public class ExtendedAttributes : IAttributes
    {
        private object _dataObj;
        private readonly NameValueCollection _dataNvc = new NameValueCollection();

        public ExtendedAttributes()
        {
        }

        public ExtendedAttributes(object dataItem)
        {
            Load(dataItem);
        }

        public ExtendedAttributes(IDataReader rdr)
        {
            Load(rdr);
        }

        public ExtendedAttributes(NameValueCollection attributes)
        {
            Load(attributes);
        }

        public ExtendedAttributes(string str)
        {
            Load(str);
        }

        public void Load(object dataItem)
        {
            _dataObj = dataItem;
        }

        public void Load(IDataReader rdr)
        {
            if (rdr == null) return;

            for (var i = 0; i < rdr.FieldCount; i++)
            {
                var name = rdr.GetName(i);
                var value = Convert.ToString(rdr.GetValue(i));
                Set(name, value);
            }
        }

        public void Load(NameValueCollection attributes)
        {
            if (attributes == null) return;

            foreach (string name in attributes)
            {
                var value = attributes[name];
                Set(name, value);
            }
        }

        public void Load(string str)
        {
            if (string.IsNullOrEmpty(str)) return;

            var nameValues = Utils.ToNameValueCollection(str);
            foreach (string key in nameValues.Keys)
            {
                Set(key, nameValues[key]);
            }
        }

        public string GetString(string name, string defaultValue)
        {
            var v = GetString(name);
            return string.IsNullOrEmpty(v) ? defaultValue : v;
        }

        public string GetString(string name)
        {
            name = name.ToLower();
            var returnValue = _dataNvc[name];

            if (returnValue == null && _dataObj != null)
            {
                var obj = Utils.Eval(_dataObj, name);
                if (obj != null)
                {
                    if (obj is string)
                    {
                        returnValue = _dataNvc[name] = obj as string;
                    }
                    else
                    {
                        returnValue = _dataNvc[name] = obj.ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(returnValue))
            {
                returnValue = Utils.UnFilterSql(returnValue);
            }

            return returnValue ?? string.Empty;
        }

        public bool GetBool(string name, bool defaultValue = false)
        {
            name = name.ToLower();
            var b = GetString(name);
            if (b == null || b.Trim().Length == 0)
                return defaultValue;
            try
            {
                return bool.Parse(b);
            }
            catch
            {
                // ignored
            }
            return defaultValue;
        }

        public int GetInt(string name, int defaultValue = 0)
        {
            name = name.ToLower();
            var i = GetString(name);
            if (i == null || i.Trim().Length == 0)
                return defaultValue;

            var retval = defaultValue;
            try
            {
                retval = int.Parse(i);
            }
            catch
            {
                // ignored
            }
            return retval;
        }

        public decimal GetDecimal(string name, decimal defaultValue = 0)
        {
            name = name.ToLower();
            var i = GetString(name);
            if (i == null || i.Trim().Length == 0)
                return defaultValue;

            var retval = defaultValue;
            try
            {
                retval = decimal.Parse(i);
            }
            catch
            {
                // ignored
            }
            return retval;
        }

        public DateTime GetDateTime(string name, DateTime defaultValue)
        {
            name = name.ToLower();
            var d = GetString(name);
            if (d == null || d.Trim().Length == 0)
                return defaultValue;

            var retval = defaultValue;
            try
            {
                retval = DateTime.Parse(d);
            }
            catch
            {
                // ignored
            }
            return retval;
        }

        public void Remove(string name)
        {
            name = name.ToLower();
            _dataNvc.Remove(name);
        }

        public void Set(string name, string value)
        {
            name = name.ToLower();

            if (value == null)
            {
                _dataNvc.Remove(name);
            }
            else
            {
                _dataNvc[name] = value;
            }
        }

        public bool ContainsKey(string name)
        {
            name = name.ToLower();
            var returnValue = _dataNvc[name];

            if (returnValue == null && _dataObj != null)
            {
                var obj = Utils.Eval(_dataObj, name);
                if (obj != null)
                {
                    if (obj is string)
                    {
                        returnValue = _dataNvc[name] = obj as string;
                    }
                    else
                    {
                        returnValue = _dataNvc[name] = obj.ToString();
                    }
                }
            }

            return returnValue != null;
        }

        public override string ToString()
        {
            if (_dataNvc != null && _dataNvc.Count > 0)
            {
                return Utils.NameValueCollectionToString(_dataNvc);
            }
            return string.Empty;
        }

        public string ToString(List<string> lowerCaseExcludeAttributeNames)
        {
            if (_dataNvc == null || _dataNvc.Count <= 0 || lowerCaseExcludeAttributeNames == null)
                return string.Empty;

            var nvc = new NameValueCollection();
            foreach (string key in _dataNvc.Keys)
            {
                if (!lowerCaseExcludeAttributeNames.Contains(key))
                {
                    nvc[key] = _dataNvc[key];
                }
            }
            return Utils.NameValueCollectionToString(nvc);
        }

        public NameValueCollection ToNameValueCollection()
        {
            return _dataNvc;
        }

        #region private utils

        private static class Utils
        {
            public static object Eval(object dataItem, string name)
            {
                object o = null;
                try
                {
                    o = DataBinder.Eval(dataItem, name);
                }
                catch
                {
                    // ignored
                }
                if (o == DBNull.Value)
                {
                    o = null;
                }
                return o;
            }

            public static string UnFilterSql(string objStr)
            {
                if (string.IsNullOrEmpty(objStr)) return string.Empty;

                return objStr.Replace("_sqlquote_", "'").Replace("_sqldoulbeline_", "--").Replace("_sqlleftparenthesis_", "\\(").Replace("_sqlrightparenthesis_", "\\)");
            }

            public static string NameValueCollectionToString(NameValueCollection attributes, char seperator = '&')
            {
                if (attributes == null || attributes.Count <= 0) return string.Empty;

                var builder = new StringBuilder();
                foreach (string key in attributes.Keys)
                {
                    builder.Append(
                        $@"{ValueToUrl(key)}={ValueToUrl(attributes[key])}{seperator}");
                }
                builder.Length--;
                return builder.ToString();
            }

            private static string ValueToUrl(string value)
            {
                var retval = string.Empty;
                if (!string.IsNullOrEmpty(value))
                {
                    //替换url中的换行符，update by sessionliang at 20151211
                    retval = value.Replace("=", "_equals_").Replace("&", "_and_").Replace("?", "_question_").Replace("'", "_quote_").Replace("+", "_add_").Replace("\r", "").Replace("\n", "");
                }
                return retval;
            }

            public static NameValueCollection ToNameValueCollection(string separateString)
            {
                if (!string.IsNullOrEmpty(separateString))
                {
                    separateString = separateString.Replace("/u0026", "&");
                }
                return ToNameValueCollection(separateString, '&');
            }

            private static string ValueFromUrl(string value)
            {
                var retval = string.Empty;
                if (!string.IsNullOrEmpty(value))
                {
                    retval = value.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+");
                }
                return retval;
            }

            private static NameValueCollection ToNameValueCollection(string separateString, char seperator)
            {
                var attributes = new NameValueCollection();
                if (!string.IsNullOrEmpty(separateString))
                {
                    var pairs = separateString.Split(seperator);
                    foreach (var pair in pairs)
                    {
                        if (pair.IndexOf("=", StringComparison.Ordinal) != -1)
                        {
                            var name = ValueFromUrl(pair.Split('=')[0]);
                            var value = ValueFromUrl(pair.Split('=')[1]);
                            attributes.Add(name.ToLower(), value);
                        }
                    }
                }
                return attributes;
            }
        }

        #endregion private utils
    }
}
