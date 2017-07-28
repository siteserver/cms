using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class KeywordResourceAttribute
    {
        protected KeywordResourceAttribute()
        {
        }

        public const string ResourceID = "ResourceID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordID = "KeywordID";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";

        public const string ResourceType = "ResourceType";
        public const string IsShowCoverPic = "IsShowCoverPic";
        public const string Content = "Content";
        public const string NavigationUrl = "NavigationUrl";
        public const string ChannelID = "ChannelID";
        public const string ContentID = "ContentID";
        public const string Taxis = "Taxis";


        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(ResourceID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(KeywordID);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    allAttributes.Add(ResourceType);
                    allAttributes.Add(IsShowCoverPic);
                    allAttributes.Add(Content);
                    allAttributes.Add(NavigationUrl);
                    allAttributes.Add(ChannelID);
                    allAttributes.Add(ContentID);
                    allAttributes.Add(Taxis);
                }

                return allAttributes;
            }
        }
    }

    public class KeywordResourceInfo
    {
        private int resourceID;
        private int publishmentSystemID;
        private int keywordID;
        private string title;
        private string imageUrl;
        private string summary;
        private EResourceType resourceType;
        private bool isShowCoverPic;
        private string content;
        private string navigationUrl;
        private int channelID;
        private int contentID;
        private int taxis;

        public KeywordResourceInfo()
        {
            resourceID = 0;
            publishmentSystemID = 0;
            keywordID = 0;
            title = string.Empty;
            imageUrl = string.Empty;
            summary = string.Empty;
            resourceType = EResourceType.Content;
            isShowCoverPic = true;
            content = string.Empty;
            navigationUrl = string.Empty;
            channelID = 0;
            contentID = 0;
            taxis = 0;
        }

        public KeywordResourceInfo(int resourceID, int publishmentSystemID, int keywordID, string title, string imageUrl, string summary, EResourceType resourceType, bool isShowCoverPic, string content, string navigationUrl, int channelID, int contentID, int taxis)
        {
            this.resourceID = resourceID;
            this.publishmentSystemID = publishmentSystemID;
            this.keywordID = keywordID;
            this.title = title;
            this.imageUrl = imageUrl;
            this.summary = summary;
            this.resourceType = resourceType;
            this.isShowCoverPic = isShowCoverPic;
            this.content = content;
            this.navigationUrl = navigationUrl;
            this.channelID = channelID;
            this.contentID = contentID;
            this.taxis = taxis;
        }
        public KeywordResourceInfo(object dataItem)
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
        public KeywordResourceInfo(NameValueCollection form, bool isFilterSqlAndXss)
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

                    if (attributeName == "ResourceType")
                    {
                        return EResourceTypeUtils.GetEnumType(nameVlaue.ToString());
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
                return KeywordResourceAttribute.AllAttributes;
            }
        }
        public int ResourceID
        {
            get { return resourceID; }
            set { resourceID = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public int KeywordID
        {
            get { return keywordID; }
            set { keywordID = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        public EResourceType ResourceType
        {
            get { return resourceType; }
            set { resourceType = value; }
        }

        public bool IsShowCoverPic
        {
            get { return isShowCoverPic; }
            set { isShowCoverPic = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public string NavigationUrl
        {
            get { return navigationUrl; }
            set { navigationUrl = value; }
        }

        public int ChannelID
        {
            get { return channelID; }
            set { channelID = value; }
        }

        public int ContentID
        {
            get { return contentID; }
            set { contentID = value; }
        }

        public int Taxis
        {
            get { return taxis; }
            set { taxis = value; }
        }
    }
}
