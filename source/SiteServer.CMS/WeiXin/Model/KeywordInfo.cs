using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class KeyWordAttribute
    {
        protected KeyWordAttribute()
        {
        }

        public const string KeywordID = "KeywordID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string Keywords = "Keywords";
        public const string IsDisabled = "IsDisabled";
        public const string KeywordType = "KeywordType";
        public const string MatchType = "MatchType";
        public const string Reply = "Reply";
        public const string AddDate = "AddDate";
        public const string Taxis = "Taxis";

        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(KeywordID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(Keywords);
                    allAttributes.Add(IsDisabled);
                    allAttributes.Add(KeywordType);
                    allAttributes.Add(MatchType);
                    allAttributes.Add(Reply);
                    allAttributes.Add(AddDate);
                    allAttributes.Add(Taxis);
                }

                return allAttributes;
            }
        }
    }

    public class KeywordInfo
    {
        private int keywordID;
        private int publishmentSystemID;
        private string keywords;
        private bool isDisabled;
        private EKeywordType keywordType;
        private EMatchType matchType;
        private string reply;
        private DateTime addDate;
        private int taxis;

        public KeywordInfo()
        {
            keywordID = 0;
            publishmentSystemID = 0;
            keywords = string.Empty;
            isDisabled = false;
            keywordType = EKeywordType.Text;
            matchType = EMatchType.Exact;
            reply = string.Empty;
            addDate = DateTime.Now;
            taxis = 0;
        }

        public KeywordInfo(int keywordID, int publishmentSystemID, string keywords, bool isDisabled, EKeywordType keywordType, EMatchType matchType, string reply, DateTime addDate, int taxis)
        {
            this.keywordID = keywordID;
            this.publishmentSystemID = publishmentSystemID;
            this.keywords = keywords;
            this.isDisabled = isDisabled;
            this.keywordType = keywordType;
            this.matchType = matchType;
            this.reply = reply;
            this.addDate = addDate;
            this.taxis = taxis;
        }

        public KeywordInfo(object dataItem)
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

        public KeywordInfo(NameValueCollection form, bool isFilterSqlAndXss)
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

        public object GetValue(string attributeName)
        {
            foreach (var name in AllAttributes)
            {
                if (StringUtils.EqualsIgnoreCase(name, attributeName))
                {
                    var nameVlaue = GetType().GetProperty(name).GetValue(this, null);

                    if (attributeName == "KeywordType")
                    {
                        return EKeywordTypeUtils.GetEnumType(nameVlaue.ToString());
                    }
                    if (attributeName == "MatchType")
                    {
                        return EMatchTypeUtils.GetEnumType(nameVlaue.ToString());
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
                return KeyWordAttribute.AllAttributes;
            }
        }

        public int KeywordID
        {
            get { return keywordID; }
            set { keywordID = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public string Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }

        public bool IsDisabled
        {
            get { return isDisabled; }
            set { isDisabled = value; }
        }

        public EKeywordType KeywordType
        {
            get { return keywordType; }
            set { keywordType = value; }
        }

        public EMatchType MatchType
        {
            get { return matchType; }
            set { matchType = value; }
        }

        public string Reply
        {
            get { return reply; }
            set { reply = value; }
        }

        public DateTime AddDate
        {
            get { return addDate; }
            set { addDate = value; }
        }

        public int Taxis
        {
            get { return taxis; }
            set { taxis = value; }
        }
    }
}
