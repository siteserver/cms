using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Model
{
    public class MenuAttribute
    {
        protected MenuAttribute()
        {
        }

        public const string MenuId = nameof(MenuInfo.MenuId);
        public const string PublishmentSystemId = nameof(MenuInfo.PublishmentSystemId);
        public const string MenuName = nameof(MenuInfo.MenuName);
        public const string MenuType = nameof(MenuInfo.MenuType);
        public const string Keyword = nameof(MenuInfo.Keyword);
        public const string Url = nameof(MenuInfo.Url);
        public const string ChannelId = nameof(MenuInfo.ChannelId);
        public const string ContentId = nameof(MenuInfo.ContentId);
        public const string ParentId = nameof(MenuInfo.ParentId);
        public const string Taxis = nameof(MenuInfo.Taxis);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            MenuId,
            PublishmentSystemId,
            MenuName,
            MenuType,
            Keyword,
            Url,
            ChannelId,
            ContentId,
            ParentId,
            Taxis
        });
    }

    public class MenuInfo
    {
        public MenuInfo()
        {
            MenuId = 0;
            PublishmentSystemId = 0;
            MenuName = string.Empty;
            MenuType = EMenuType.Keyword;
            Keyword = string.Empty;
            Url = string.Empty;
            ChannelId = 0;
            ContentId = 0;
            ParentId = 0;
            Taxis = 0;
        }

        public MenuInfo(int menuId, int publishmentSystemId, string menuName, EMenuType menuType, string keyword, string url, int channelId, int contentId, int parentId, int taxis)
        {
            MenuId = menuId;
            PublishmentSystemId = publishmentSystemId;
            MenuName = menuName;
            MenuType = menuType;
            Keyword = keyword;
            Url = url;
            ChannelId = channelId;
            ContentId = contentId;
            ParentId = parentId;
            Taxis = taxis;
        }

        public MenuInfo(object dataItem)
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

        public MenuInfo(NameValueCollection form, bool isFilterSqlAndXss)
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

                if (attributeName == "MenuType")
                {
                    return EMenuTypeUtils.GetEnumType(nameVlaue.ToString());
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

        protected List<string> AllAttributes => MenuAttribute.AllAttributes;

        public int MenuId { get; set; }

        public int PublishmentSystemId { get; set; }

        public string MenuName { get; set; }

        public EMenuType MenuType { get; set; }

        public string Keyword { get; set; }

        public string Url { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public int ParentId { get; set; }

        public int Taxis { get; set; }
    }
}
