using System;
using System.Collections.Specialized;
using System.Collections;
using BaiRong.Core.Data;

namespace BaiRong.Core.Model
{
    /// <summary>
    /// Provides standard implementation for simple extendent data storage
    /// </summary>
    [Serializable]
    public class CaseExtendedAttributes : Copyable
    {
        private readonly object _dataItem;
        private Hashtable _extendedAttributes = new Hashtable();

        public CaseExtendedAttributes()
        {
        }

        public CaseExtendedAttributes(object dataItem)
        {
            _dataItem = dataItem;
        }

        public virtual object GetExtendedAttribute(string name)
        {
            var returnValue = _extendedAttributes[name];

            if (returnValue == null && _dataItem != null)
            {
                returnValue = SqlUtils.Eval(_dataItem, name);
            }

            return returnValue ?? string.Empty;
        }

        public bool ContainsKey(string name)
        {
            var returnValue = _extendedAttributes[name];

            if (returnValue == null && _dataItem != null)
            {
                returnValue = SqlUtils.Eval(_dataItem, name);
            }

            return (returnValue == null) ? false : true;
        }

        public virtual void SetExtendedAttribute(string name, object value)
        {
            if (value == null)
                _extendedAttributes.Remove(name);
            else
                _extendedAttributes[name] = value;
        }

        public static void SetExtendedAttribute(Hashtable extendedAttributes, string name, object value)
        {
            if (value == null)
                extendedAttributes.Remove(name);
            else
                extendedAttributes[name] = value;
        }

        public void SetExtendedAttribute(NameValueCollection attributes)
        {
            if (attributes != null)
            {
                foreach (string key in attributes)
                {
                    SetExtendedAttribute(key, attributes[key]);
                }
            }
        }

        public virtual Hashtable Attributes => _extendedAttributes;

        public int CaseExtendedAttributesCount => _extendedAttributes.Count;

        protected bool GetBool(string name, bool defaultValue)
        {
            var b = GetExtendedAttribute(name);
            if (b == null)
                return defaultValue;
            try
            {
                return bool.Parse(b.ToString());
            }
            catch
            {
                // ignored
            }
            return defaultValue;
        }

        protected int GetInt(string name, int defaultValue)
        {
            var i = GetExtendedAttribute(name);
            if (i == null)
                return defaultValue;

            var retval = defaultValue;
            try
            {
                retval = int.Parse(i.ToString());
            }
            catch
            {
                // ignored
            }
            return retval;
        }

        protected decimal GetDecimal(string name, decimal defaultValue)
        {
            var i = GetExtendedAttribute(name);
            if (i == null)
                return defaultValue;

            var retval = defaultValue;
            try
            {
                retval = decimal.Parse(i.ToString());
            }
            catch
            {
                // ignored
            }
            return retval;
        }

        protected DateTime GetDateTime(string name, DateTime defaultValue)
        {
            var d = GetExtendedAttribute(name);
            if (d == null)
                return defaultValue;

            var retval = defaultValue;
            try
            {
                retval = DateTime.Parse(d.ToString());
            }
            catch
            {
                // ignored
            }
            return retval;
        }

        protected string GetString(string name, string defaultValue)
        {
            var v = GetExtendedAttribute(name);
            return v?.ToString() ?? defaultValue;
        }

        public override object Copy()
        {
            var ea = (CaseExtendedAttributes)CreateNewInstance();
            ea._extendedAttributes = new Hashtable(_extendedAttributes);
            return ea;
        }

        protected virtual ArrayList GetDefaultAttributesNames()
        {
            return new ArrayList();
        }
    }
}
