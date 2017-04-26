using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using BaiRong.Core.Data;

namespace BaiRong.Core.Model
{
    /// <summary>
    /// Provides standard implementation for simple extendent data storage
    /// </summary>
    [Serializable]
    public class ExtendedAttributes : Copyable
    {
        private readonly object _dataItem;
        private NameValueCollection _extendedAttributes = new NameValueCollection();

        public virtual NameValueCollection Attributes => _extendedAttributes;

        public ExtendedAttributes()
        {
        }

        public ExtendedAttributes(object dataItem)
        {
            _dataItem = dataItem;
            AfterExecuteReader();
        }

        public virtual string GetExtendedAttribute(string name)
        {
            name = name.ToLower();
            var returnValue = _extendedAttributes[name];

            if (returnValue == null && _dataItem != null)
            {
                var obj = SqlUtils.Eval(_dataItem, name);
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
                returnValue = PageUtils.UnFilterSql(returnValue);
            }

            return returnValue ?? string.Empty;
        }

        public bool ContainsKey(string name)
        {
            name = name.ToLower();
            var returnValue = _extendedAttributes[name];

            if (returnValue == null && _dataItem != null)
            {
                var obj = SqlUtils.Eval(_dataItem, name);
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

        public bool ShouldSerializeAttributes()
        {
            return false;
        }

        public int ExtendedAttributesCount => _extendedAttributes.Count;

        public bool ShouldSerializeExtendedAttributesCount()
        {
            return false;
        }

        protected bool GetBool(string name, bool defaultValue)
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

        protected int GetInt(string name, int defaultValue)
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

        protected decimal GetDecimal(string name, decimal defaultValue)
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

        protected DateTime GetDateTime(string name, DateTime defaultValue)
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

        protected string GetString(string name, string defaultValue)
        {
            name = name.ToLower();
            var v = GetExtendedAttribute(name);
            return (string.IsNullOrEmpty(v)) ? defaultValue : v;
        }

        public override object Copy()
        {
            var ea = (ExtendedAttributes)CreateNewInstance();
            ea._extendedAttributes = new NameValueCollection(_extendedAttributes);
            return ea;
        }

        private string SettingsXml
        {
            get { return GetExtendedAttribute("SettingsXML"); }
            set { SetExtendedAttribute("SettingsXML", value); }
        }

        public virtual List<string> GetDefaultAttributesNames()
        {
            return new List<string>();
        }

        //将数据保存至数据库前执行
        public void BeforeExecuteNonQuery()
        {
            var attributes = new NameValueCollection(Attributes);

            foreach (var attributeName in GetDefaultAttributesNames())
            {
                attributes.Remove(attributeName.ToLower());
            }

            SettingsXml = TranslateUtils.NameValueCollectionToString(attributes);
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

            var attributes = TranslateUtils.ToNameValueCollection(SettingsXml);
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
                return TranslateUtils.NameValueCollectionToString(_extendedAttributes);
            }
            return string.Empty;
        }

        public static ExtendedAttributes Parse(string str)
        {
            return new ExtendedAttributes
            {
                _extendedAttributes = TranslateUtils.ToNameValueCollection(str)
            };
        }
    }
}
