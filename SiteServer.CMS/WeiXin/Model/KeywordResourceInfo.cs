using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class KeywordResourceAttribute
    {
        protected KeywordResourceAttribute()
        {
        }

        public const string ResourceId = nameof(KeywordResourceInfo.ResourceId);
        public const string PublishmentSystemId = nameof(KeywordResourceInfo.PublishmentSystemId);
        public const string KeywordId = nameof(KeywordResourceInfo.KeywordId);
        public const string Title = nameof(KeywordResourceInfo.Title);
        public const string ImageUrl = nameof(KeywordResourceInfo.ImageUrl);
        public const string Summary = nameof(KeywordResourceInfo.Summary);
        public const string ResourceType = nameof(KeywordResourceInfo.ResourceType);
        public const string IsShowCoverPic = nameof(KeywordResourceInfo.IsShowCoverPic);
        public const string Content = nameof(KeywordResourceInfo.Content);
        public const string NavigationUrl = nameof(KeywordResourceInfo.NavigationUrl);
        public const string ChannelId = nameof(KeywordResourceInfo.ChannelId);
        public const string ContentId = nameof(KeywordResourceInfo.ContentId);
        public const string Taxis = nameof(KeywordResourceInfo.Taxis);


        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            ResourceId,
            PublishmentSystemId,
            KeywordId,
            Title,
            ImageUrl,
            Summary,
            ResourceType,
            IsShowCoverPic,
            Content,
            NavigationUrl,
            ChannelId,
            ContentId,
            Taxis
        });
    }

    public class KeywordResourceInfo
    {
        public KeywordResourceInfo()
        {
            ResourceId = 0;
            PublishmentSystemId = 0;
            KeywordId = 0;
            Title = string.Empty;
            ImageUrl = string.Empty;
            Summary = string.Empty;
            ResourceType = EResourceType.Content;
            IsShowCoverPic = true;
            Content = string.Empty;
            NavigationUrl = string.Empty;
            ChannelId = 0;
            ContentId = 0;
            Taxis = 0;
        }

        public KeywordResourceInfo(int resourceId, int publishmentSystemId, int keywordId, string title, string imageUrl, string summary, EResourceType resourceType, bool isShowCoverPic, string content, string navigationUrl, int channelId, int contentId, int taxis)
        {
            ResourceId = resourceId;
            PublishmentSystemId = publishmentSystemId;
            KeywordId = keywordId;
            Title = title;
            ImageUrl = imageUrl;
            Summary = summary;
            ResourceType = resourceType;
            IsShowCoverPic = isShowCoverPic;
            Content = content;
            NavigationUrl = navigationUrl;
            ChannelId = channelId;
            ContentId = contentId;
            Taxis = taxis;
        }

        public KeywordResourceInfo(object dataItem)
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

        public KeywordResourceInfo(NameValueCollection form, bool isFilterSqlAndXss)
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

                if (attributeName == "ResourceType")
                {
                    return EResourceTypeUtils.GetEnumType(nameVlaue.ToString());
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

        protected List<string> AllAttributes => KeywordResourceAttribute.AllAttributes;

        public int ResourceId { get; set; }

        public int PublishmentSystemId { get; set; }

        public int KeywordId { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string Summary { get; set; }

        public EResourceType ResourceType { get; set; }

        public bool IsShowCoverPic { get; set; }

        public string Content { get; set; }

        public string NavigationUrl { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public int Taxis { get; set; }
    }
}
