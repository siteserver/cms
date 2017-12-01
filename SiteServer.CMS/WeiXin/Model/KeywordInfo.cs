using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class KeyWordAttribute
    {
        protected KeyWordAttribute()
        {
        }

        public const string KeywordId = nameof(KeywordInfo.KeywordId);
        public const string PublishmentSystemId = nameof(KeywordInfo.PublishmentSystemId);
        public const string Keywords = nameof(KeywordInfo.Keywords);
        public const string IsDisabled = nameof(KeywordInfo.IsDisabled);
        public const string KeywordType = nameof(KeywordInfo.KeywordType);
        public const string MatchType = nameof(KeywordInfo.MatchType);
        public const string Reply = nameof(KeywordInfo.Reply);
        public const string AddDate = nameof(KeywordInfo.AddDate);
        public const string Taxis = nameof(KeywordInfo.Taxis);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            KeywordId,
            PublishmentSystemId,
            Keywords,
            IsDisabled,
            KeywordType,
            MatchType,
            Reply,
            AddDate,
            Taxis
        });
    }

    public class KeywordInfo
    {
        public KeywordInfo()
        {
            KeywordId = 0;
            PublishmentSystemId = 0;
            Keywords = string.Empty;
            IsDisabled = false;
            KeywordType = EKeywordType.Text;
            MatchType = EMatchType.Exact;
            Reply = string.Empty;
            AddDate = DateTime.Now;
            Taxis = 0;
        }

        public KeywordInfo(int keywordId, int publishmentSystemId, string keywords, bool isDisabled, EKeywordType keywordType, EMatchType matchType, string reply, DateTime addDate, int taxis)
        {
            KeywordId = keywordId;
            PublishmentSystemId = publishmentSystemId;
            Keywords = keywords;
            IsDisabled = isDisabled;
            KeywordType = keywordType;
            MatchType = matchType;
            Reply = reply;
            AddDate = addDate;
            Taxis = taxis;
        }

        public KeywordInfo(object dataItem)
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

        public KeywordInfo(NameValueCollection form, bool isFilterSqlAndXss)
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

        public object GetValue(string attributeName)
        {
            foreach (var name in AllAttributes)
            {
                if (!StringUtils.EqualsIgnoreCase(name, attributeName)) continue;

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

        protected List<string> AllAttributes => KeyWordAttribute.AllAttributes;

        public int KeywordId { get; set; }

        public int PublishmentSystemId { get; set; }

        public string Keywords { get; set; }

        public bool IsDisabled { get; set; }

        public EKeywordType KeywordType { get; set; }

        public EMatchType MatchType { get; set; }

        public string Reply { get; set; }

        public DateTime AddDate { get; set; }

        public int Taxis { get; set; }
    }
}
