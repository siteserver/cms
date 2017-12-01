using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CountAttribute
    {
        protected CountAttribute()
        {
        }

        public const string CountId = nameof(CountInfo.CountId);
        public const string PublishmentSystemId = nameof(CountInfo.PublishmentSystemId);
        public const string CountYear = nameof(CountInfo.CountYear);
        public const string CountMonth = nameof(CountInfo.CountMonth);
        public const string CountDay = nameof(CountInfo.CountDay);
        public const string CountType = nameof(CountInfo.CountType);
        public const string Count = nameof(CountInfo.Count);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            CountId,
            PublishmentSystemId,
            CountYear,
            CountMonth,
            CountDay,
            CountType,
            Count
        });
    }

    public class CountInfo
    {
        public CountInfo()
        {
            CountId = 0;
            PublishmentSystemId = 0;
            CountYear = 0;
            CountMonth = 0;
            CountDay = 0;
            CountType = ECountType.UserSubscribe;
            Count = 0;
        }

        public CountInfo(int countId, int publishmentSystemId, int countYear, int countMonth, int countDay, ECountType countType, int count)
        {
            CountId = countId;
            PublishmentSystemId = publishmentSystemId;
            CountYear = countYear;
            CountMonth = countMonth;
            CountDay = countDay;
            CountType = countType;
            Count = count;
        }

        public CountInfo(NameValueCollection form, bool isFilterSqlAndXss)
        {
            if (form == null) return;

            foreach (var name in AllAttributes)
            {
                var value = form[name];
                if (value == null) continue;

                if (isFilterSqlAndXss)
                {
                    value = PageUtils.FilterSqlAndXss(value);
                }
                SetValueInternal(name, value);
            }
        }

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

        public CountInfo(object dataItem)
        {
            if (dataItem == null) return;

            foreach (var name in AllAttributes)
            {
                var value = SqlUtils.Eval(dataItem, name);
                if (value != null)
                {
                    SetValueInternal(name, value);
                }
            }
        }

        public object GetValue(string attributeName)
        {
            foreach (var name in AllAttributes)
            {
                if (!StringUtils.EqualsIgnoreCase(name, attributeName)) continue;

                var nameVlaue = GetType().GetProperty(name).GetValue(this, null);

                return attributeName == "CountType" ? ECountTypeUtils.GetEnumType(nameVlaue.ToString()) : nameVlaue;
            }
            return null;
        }

        public void SetValue(string attributeName, object value)
        {
            foreach (var name in AllAttributes)
            {
                if (!StringUtils.EqualsIgnoreCase(name, attributeName)) continue;
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

        private void SetValueInternal(string name, object value)
        {
            try
            {
                GetType().GetProperty(name).SetValue(this, value, null);
            }
            catch
            {
                if (StringUtils.ContainsIgnoreCase(name, "Is"))
                {
                    GetType().GetProperty(name).SetValue(this, TranslateUtils.ToBool(value.ToString()), null);
                }
                else if (StringUtils.EndsWithIgnoreCase(name, "Date"))
                {
                    GetType().GetProperty(name).SetValue(this, TranslateUtils.ToDateTime(value.ToString()), null);
                }
                else if (StringUtils.EndsWithIgnoreCase(name, "Id") || StringUtils.EndsWithIgnoreCase(name, "Num") || StringUtils.EndsWithIgnoreCase(name, "Count"))
                {
                    GetType().GetProperty(name).SetValue(this, TranslateUtils.ToInt(value.ToString()), null);
                }
                else if (StringUtils.EndsWithIgnoreCase(name, "Amount"))
                {
                    GetType().GetProperty(name).SetValue(this, TranslateUtils.ToDecimal(value.ToString()), null);
                }
            }
        }

        protected List<string> AllAttributes => CountAttribute.AllAttributes;

        public int CountId { get; set; }

        public int PublishmentSystemId { get; set; }

        public int CountYear { get; set; }

        public int CountMonth { get; set; }

        public int CountDay { get; set; }

        public ECountType CountType { get; set; }

        public int Count { get; set; }
    }
}
