using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;

namespace SiteServer.Plugin.Models
{
    /// <summary>
    /// Provides standard implementation for simple extendent data storage
    /// </summary>
    [Serializable]
    public class ExtendedAttributes : Copyable
    {
        private readonly object _dataItem;
        private NameValueCollection _extendedAttributes = new NameValueCollection();
        public NameValueCollection GetExtendedAttributes()
        {
            return _extendedAttributes;
        }

        public ExtendedAttributes()
        {
        }

        public ExtendedAttributes(object dataItem)
        {
            _dataItem = dataItem;
            AfterExecuteReader();
        }

        public ExtendedAttributes(string str)
        {
            _extendedAttributes = ToNameValueCollection(str);
        }

        public virtual string GetExtendedAttribute(string name)
        {
            name = name.ToLower();
            var returnValue = _extendedAttributes[name];

            if (returnValue == null && _dataItem != null)
            {
                var obj = Eval(_dataItem, name);
                if (obj != null)
                {
                    if (obj is string)
                    {
                        returnValue = _extendedAttributes[name] = obj as string;
                    }
                    else
                    {
                        returnValue = _extendedAttributes[name] = obj.ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(returnValue))
            {
                returnValue = UnFilterSql(returnValue);
            }

            return returnValue ?? string.Empty;
        }

        public virtual void RemoveExtendedAttribute(string name)
        {
            name = name.ToLower();
            _extendedAttributes.Remove(name);
        }

        public bool ContainsKey(string name)
        {
            name = name.ToLower();
            var returnValue = _extendedAttributes[name];

            if (returnValue == null && _dataItem != null)
            {
                var obj = Eval(_dataItem, name);
                if (obj != null)
                {
                    if (obj is string)
                    {
                        returnValue = _extendedAttributes[name] = obj as string;
                    }
                    else
                    {
                        returnValue = _extendedAttributes[name] = obj.ToString();
                    }
                }
            }

            return returnValue != null;
        }

        public virtual void SetExtendedAttribute(string name, string value)
        {
            name = name.ToLower();

            if (value == null)
            {
                _extendedAttributes.Remove(name);
            }
            else
            {
                _extendedAttributes[name] = value;
            }
        }

        public static void SetExtendedAttribute(NameValueCollection extendedAttributes, string name, string value)
        {
            name = name.ToLower();

            if (value == null)
            {
                extendedAttributes.Remove(name);
            }
            else
            {
                extendedAttributes[name] = value;
            }
        }

        public void SetExtendedAttribute(NameValueCollection attributes)
        {
            if (attributes == null) return;

            foreach (string key in attributes)
            {
                SetExtendedAttribute(key.ToLower(), attributes[key]);
            }
        }

        public int ExtendedAttributesCount => _extendedAttributes.Count;

        public bool GetBool(string name, bool defaultValue = false)
        {
            name = name.ToLower();
            var b = GetExtendedAttribute(name);
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
            var i = GetExtendedAttribute(name);
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
            var i = GetExtendedAttribute(name);
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
            var d = GetExtendedAttribute(name);
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

        public string GetString(string name, string defaultValue = "")
        {
            name = name.ToLower();
            var v = GetExtendedAttribute(name);
            return string.IsNullOrEmpty(v) ? defaultValue : v;
        }

        public override object Copy()
        {
            var ea = (ExtendedAttributes)CreateNewInstance();
            ea._extendedAttributes = new NameValueCollection(_extendedAttributes);
            return ea;
        }

        private string SettingsXml
        {
            get { return GetExtendedAttribute("SettingsXml"); }
            set { SetExtendedAttribute("SettingsXml", value); }
        }

        public virtual List<string> GetDefaultAttributesNames()
        {
            return new List<string>();
        }

        //将数据保存至数据库前执行
        public void BeforeExecuteNonQuery()
        {
            var attributes = new NameValueCollection(GetExtendedAttributes());

            foreach (var attributeName in GetDefaultAttributesNames())
            {
                attributes.Remove(attributeName.ToLower());
            }

            SettingsXml = NameValueCollectionToString(attributes);
        }


        public string GetSettingsXml()
        {
            BeforeExecuteNonQuery();
            return SettingsXml;
        }

        //从数据库获取数据后执行
        public void AfterExecuteReader()
        {
            foreach (var attributeName in GetDefaultAttributesNames())
            {
                var value = GetExtendedAttribute(attributeName);
                SetExtendedAttribute(attributeName.ToLower(), value);
            }

            var attributes = ToNameValueCollection(SettingsXml);
            if (attributes == null) return;

            foreach (string key in attributes)
            {
                if (string.IsNullOrEmpty(GetExtendedAttribute(key)))
                {
                    SetExtendedAttribute(key.ToLower(), attributes[key]);
                }
            }
        }

        public SerializerData GetSerializerData()
        {
            var data = new SerializerData();

            string keys = null;
            string values = null;

            Serializer.ConvertFromNameValueCollection(_extendedAttributes, ref keys, ref values);
            data.Keys = keys;
            data.Values = values;

            return data;
        }

        public void SetSerializerData(SerializerData data)
        {
            if (_extendedAttributes == null || _extendedAttributes.Count == 0)
            {
                _extendedAttributes = Serializer.ConvertToNameValueCollection(data.Keys, data.Values);
            }

            if (_extendedAttributes == null)
                _extendedAttributes = new NameValueCollection();
        }

        public override string ToString()
        {
            if (_extendedAttributes != null && _extendedAttributes.Count > 0)
            {
                return NameValueCollectionToString(_extendedAttributes);
            }
            return string.Empty;
        }

        public static ExtendedAttributes Parse(string str)
        {
            return new ExtendedAttributes
            {
                _extendedAttributes = ToNameValueCollection(str)
            };
        }

        public void Load(string str)
        {
            _extendedAttributes = ToNameValueCollection(str);
        }

        #region Utils

        private static object Eval(object dataItem, string name)
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

        private static string UnFilterSql(string objStr)
        {
            if (string.IsNullOrEmpty(objStr)) return string.Empty;

            return objStr.Replace("_sqlquote_", "'").Replace("_sqldoulbeline_", "--").Replace("_sqlleftparenthesis_", "\\(").Replace("_sqlrightparenthesis_", "\\)");
        }

        private static string NameValueCollectionToString(NameValueCollection attributes, char seperator = '&')
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

        private static NameValueCollection ToNameValueCollection(string separateString)
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
                        attributes.Add(name, value);
                    }
                }
            }
            return attributes;
        }

        #endregion
    }
}
