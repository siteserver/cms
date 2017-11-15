using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class KeywordMatchAttribute
    {
        protected KeywordMatchAttribute()
        {
        }

        public const string MatchId = nameof(KeywordMatchInfo.MatchId);
        public const string PublishmentSystemId = nameof(KeywordMatchInfo.PublishmentSystemId);
        public const string Keyword = nameof(KeywordMatchInfo.Keyword);
        public const string KeywordId = nameof(KeywordMatchInfo.KeywordId);
        public const string IsDisabled = nameof(KeywordMatchInfo.IsDisabled);
        public const string KeywordType = nameof(KeywordMatchInfo.KeywordType);
        public const string MatchType = nameof(KeywordMatchInfo.MatchType);


        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            MatchId,
            PublishmentSystemId,
            Keyword,
            IsDisabled,
            KeywordId,
            MatchType,
            KeywordType
        });
    }

    public class KeywordMatchInfo
    {
        public KeywordMatchInfo()
        {
            MatchId = 0;
            PublishmentSystemId = 0;
            Keyword = string.Empty;
            KeywordId = 0;
            IsDisabled = false;
            KeywordType = EKeywordType.Text;
            MatchType = EMatchType.Exact;
        }

        public KeywordMatchInfo(int matchId, int publishmentSystemId, string keyword, int keywordId, bool isDisabled, EKeywordType keywordType, EMatchType matchType)
        {
            MatchId = matchId;
            PublishmentSystemId = publishmentSystemId;
            Keyword = keyword;
            KeywordId = keywordId;
            IsDisabled = isDisabled;
            KeywordType = keywordType;
            MatchType = matchType;
        }

        public KeywordMatchInfo(object dataItem)
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

        public KeywordMatchInfo(NameValueCollection form, bool isFilterSqlAndXss)
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

        protected List<string> AllAttributes => KeywordMatchAttribute.AllAttributes;

        public int MatchId { get; set; }

        public int PublishmentSystemId { get; set; }

        public string Keyword { get; set; }

        public int KeywordId { get; set; }

        public bool IsDisabled { get; set; }

        public EKeywordType KeywordType { get; set; }

        public EMatchType MatchType { get; set; }
    }
}
