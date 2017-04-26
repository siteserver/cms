using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class MenuAttribute
    {
        protected MenuAttribute()
        {
        }

        public const string MenuID = "MenuID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string MenuName = "MenuName";
        public const string MenuType = "MenuType";
        public const string Keyword = "Keyword";
        public const string Url = "Url";
        public const string ChannelID = "ChannelID";
        public const string ContentID = "ContentID";
        public const string ParentID = "ParentID";
        public const string Taxis = "Taxis";

        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(MenuID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(MenuName);
                    allAttributes.Add(MenuType);
                    allAttributes.Add(Keyword);
                    allAttributes.Add(Url);
                    allAttributes.Add(ChannelID);
                    allAttributes.Add(ContentID);
                    allAttributes.Add(ParentID);
                    allAttributes.Add(Taxis);
                }

                return allAttributes;
            }
        }
    }


    public class MenuInfo
    {
        private int menuID;
        private int publishmentSystemID;
        private string menuName;
        private EMenuType menuType;
        private string keyword;
        private string url;
        private int channelID;
        private int contentID;
        private int parentID;
        private int taxis;

        public MenuInfo()
        {
            menuID = 0;
            publishmentSystemID = 0;
            menuName = string.Empty;
            menuType = EMenuType.Keyword;
            keyword = string.Empty;
            url = string.Empty;
            channelID = 0;
            contentID = 0;
            parentID = 0;
            taxis = 0;
        }

        public MenuInfo(int menuID, int publishmentSystemID, string menuName, EMenuType menuType, string keyword, string url, int channelID, int contentID, int parentID, int taxis)
        {
            this.menuID = menuID;
            this.publishmentSystemID = publishmentSystemID;
            this.menuName = menuName;
            this.menuType = menuType;
            this.keyword = keyword;
            this.url = url;
            this.channelID = channelID;
            this.contentID = contentID;
            this.parentID = parentID;
            this.taxis = taxis;
        }

        public MenuInfo(object dataItem)
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
        public MenuInfo(NameValueCollection form, bool isFilterSqlAndXss)
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

                    if (attributeName == "MenuType")
                    {
                        return EMenuTypeUtils.GetEnumType(nameVlaue.ToString());
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
                return MenuAttribute.AllAttributes;
            }
        }
        public int MenuID
        {
            get { return menuID; }
            set { menuID = value; }
        }

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public string MenuName
        {
            get { return menuName; }
            set { menuName = value; }
        }

        public EMenuType MenuType
        {
            get { return menuType; }
            set { menuType = value; }
        }

        public string Keyword
        {
            get { return keyword; }
            set { keyword = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
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

        public int ParentID
        {
            get { return parentID; }
            set { parentID = value; }
        }

        public int Taxis
        {
            get { return taxis; }
            set { taxis = value; }
        }
    }
}
