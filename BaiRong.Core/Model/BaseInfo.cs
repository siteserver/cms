using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Data;

namespace BaiRong.Core.Model
{
    public abstract class BaseInfo
    {
        protected BaseInfo()
        {
        }

        protected BaseInfo(object dataItem)
        {
            if (dataItem != null)
            {
                if (AllAttributes != null)
                {
                    foreach (var name in AllAttributes)
                    {
                        var value = SqlUtils.Eval(dataItem, name);
                        if (value != null)
                        {
                            SetValueInternal(name, value);
                        }
                    }
                }
            }
        }

        protected BaseInfo(NameValueCollection form) : this(form, false) 
        {

        }

        protected BaseInfo(NameValueCollection form, bool isFilterSqlAndXss)
        {
            if (form != null)
            {
                if (AllAttributes != null)
                {
                    foreach (var name in AllAttributes)
                    {
                        var value = form[name];
                        if (value != null)
                        {
                            if (isFilterSqlAndXss)
                            {
                                value = PageUtils.FilterSqlAndXss(value);
                            }
                            SetValueInternal(name, value);
                        }
                    }
                }
            }
        }

        protected BaseInfo(IDataReader rdr)
        {
            for (var i = 0; i < rdr.FieldCount; i++)
            {
                var columnName = rdr.GetName(i);
                SetValue(columnName, rdr.GetValue(i));
            }
        }

        public int Id { get; set; }

        protected abstract List<string> AllAttributes { get; }

        public NameValueCollection ToNameValueCollection()
        {
            var attributes = new NameValueCollection();
            foreach (var attributeName in AllAttributes)
            {
                var value = GetType().GetProperty(attributeName).GetValue(this, null);
                if (value != null)
                {
                    attributes.Add(attributeName, value.ToString());
                }
            }
            return attributes;
        }

        public Dictionary<string, object> ToDictionary()
        {
            var attributes = new Dictionary<string, object>();
            foreach (var attributeName in AllAttributes)
            {
                var value = GetType().GetProperty(attributeName).GetValue(this, null);
                if (value != null)
                {
                    attributes.Add(attributeName, value);
                }
            }
            return attributes;
        }

        public object GetValue(string attributeName)
        {
            foreach (var name in AllAttributes)
            {
                if (StringUtils.EqualsIgnoreCase(name, attributeName))
                {
                    return GetType().GetProperty(name).GetValue(this, null);
                }
            }
            return null;
        }

        public void SetValue(string attributeName, object value)
        {
            foreach (var name in AllAttributes)
            {
                if (StringUtils.EqualsIgnoreCase(name, attributeName))
                {
                    try
                    {
                        SetValueInternal(name, value);
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
                }
            }
        }

        private void SetValueInternal(string name, object value)
        {
            try
            {
                GetType().GetProperty(name).SetValue(this, value, null);
            }
            catch
            {
                if (StringUtils.StartsWithIgnoreCase(name, "Is"))
                {
                    GetType().GetProperty(name).SetValue(this, TranslateUtils.ToBool(value.ToString()), null);
                }
                else if (StringUtils.EndsWithIgnoreCase(name, "Date"))
                {
                    GetType().GetProperty(name).SetValue(this, TranslateUtils.ToDateTime(value.ToString()), null);
                }
                else if (StringUtils.EndsWithIgnoreCase(name, "ID") || StringUtils.EndsWithIgnoreCase(name, "Num") || StringUtils.EndsWithIgnoreCase(name, "Count"))
                {
                    GetType().GetProperty(name).SetValue(this, TranslateUtils.ToInt(value.ToString()), null);
                }
                else if (StringUtils.EndsWithIgnoreCase(name, "Amount"))
                {
                    GetType().GetProperty(name).SetValue(this, TranslateUtils.ToDecimal(value.ToString()), null);
                }
            }
        }
    }
}
