using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CountAttribute
    {
        protected CountAttribute()
        {
        }

        public const string CountID = "CountID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string CountYear = "CountYear";
        public const string CountMonth = "CountMonth";
        public const string CountDay = "CountDay";
        public const string CountType = "CountType";
        public const string Count = "Count";

        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(CountID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(CountYear);
                    allAttributes.Add(CountMonth);
                    allAttributes.Add(CountDay);
                    allAttributes.Add(CountType);
                    allAttributes.Add(Count);

                }

                return allAttributes;
            }
        }
    }

    public class CountInfo
    {
        private int countID;
        private int publishmentSystemID;
        private int countYear;
        private int countMonth;
        private int countDay;
        private ECountType countType;
        private int count;

        public CountInfo()
        {
            countID = 0;
            publishmentSystemID = 0;
            countYear = 0;
            countMonth = 0;
            countDay = 0;
            countType = ECountType.UserSubscribe;
            count = 0;
        }

        public CountInfo(int countID, int publishmentSystemID, int countYear, int countMonth, int countDay, ECountType countType, int count)
        {
            this.countID = countID;
            this.publishmentSystemID = publishmentSystemID;
            this.countYear = countYear;
            this.countMonth = countMonth;
            this.countDay = countDay;
            this.countType = countType;
            this.count = count;
        }


        public CountInfo(NameValueCollection form, bool isFilterSqlAndXss)
        {
            if (form != null)
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
            if (dataItem != null)
            {
                foreach (var name in AllAttributes)
                {
                    var value = TranslateUtils.Eval(dataItem, name);
                    if (value != null)
                    {
                        SetValueInternal(name, value);
                    }
                }
            }
        }

        public object GetValue(string attributeName)
        {
            foreach (var name in AllAttributes)
            {
                if (StringUtils.EqualsIgnoreCase(name, attributeName))
                {
                    var nameVlaue = GetType().GetProperty(name).GetValue(this, null);

                    if (attributeName == "CountType")
                    {
                        return ECountTypeUtils.GetEnumType(nameVlaue.ToString());
                    }

                    return nameVlaue;
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
                    catch { }

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
                if (StringUtils.ContainsIgnoreCase(name, "Is"))
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

        protected List<string> AllAttributes
        {
            get
            {
                return CountAttribute.AllAttributes;
            }
        }

        public int CountID
        {
            get { return countID; }
            set { countID = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public int CountYear
        {
            get { return countYear; }
            set { countYear = value; }
        }

        public int CountMonth
        {
            get { return countMonth; }
            set { countMonth = value; }
        }

        public int CountDay
        {
            get { return countDay; }
            set { countDay = value; }
        }

        public ECountType CountType
        {
            get { return countType; }
            set { countType = value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }
    }
}
