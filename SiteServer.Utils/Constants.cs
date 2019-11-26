using System;
using System.Collections.Generic;
using System.Text;

namespace SiteServer.Utils
{
    public static class Constants
    {
        public const string AuthKeyUserHeader = "X-SS-USER-TOKEN";
        public const string AuthKeyUserCookie = "SS-USER-TOKEN";
        public const string AuthKeyUserQuery = "userToken";
        public const string AuthKeyAdminHeader = "X-SS-ADMIN-TOKEN";
        public const string AuthKeyAdminCookie = "SS-ADMIN-TOKEN";
        public const string AuthKeyAdminQuery = "adminToken";
        public const string AuthKeyApiHeader = "X-SS-API-KEY";
        public const string AuthKeyApiCookie = "SS-API-KEY";
        public const string AuthKeyApiQuery = "apiKey";
        public const int AccessTokenExpireDays = 7;

        public const string ScopeChannels = "Channels";
        public const string ScopeContents = "Contents";
        public const string ScopeAdministrators = "Administrators";
        public const string ScopeUsers = "Users";
        public const string ScopeStl = "STL";

        public static readonly List<string> ScopeList = new List<string>
        {
            ScopeChannels,
            ScopeContents,
            ScopeAdministrators,
            ScopeUsers,
            ScopeStl
        };

        public static class TopMenu
        {
            public const string IdSite = "Site";
        }

        public static class PluginsPermissions
        {
            public const string Add = "plugins_add";
        }

        public static class SettingsPermissions
        {
            public const string SiteAdd = "settings_siteAdd";
            public const string Site = "settings_site";
            public const string Admin = "settings_admin";
            public const string User = "settings_user";
            public const string Chart = "settings_chart";
            public const string Log = "settings_log";
            public const string Config = "settings_config";
            public const string Utility = "settings_utility";
        }

        public static class WebSitePermissions
        {
            public const string Content = "cms_content";                            //信息管理
            public const string Template = "cms_template";                          //显示管理
            public const string Configuration = "cms_configuration";                  //设置管理
            public const string Create = "cms_create";                              //生成管理
        }

        public static class ChannelPermissions
        {
            public const string ContentView = "cms_contentView";
            public const string ContentAdd = "cms_contentAdd";
            public const string ContentEdit = "cms_contentEdit";
            public const string ContentDelete = "cms_contentDelete";
            public const string ContentTranslate = "cms_contentTranslate";
            public const string ChannelAdd = "cms_channelAdd";
            public const string ChannelEdit = "cms_channelEdit";
            public const string ChannelDelete = "cms_channelDelete";
            public const string ChannelTranslate = "cms_channelTranslate";
            public const string CreatePage = "cms_createPage";
            public const string ContentCheck = "cms_contentCheck";
            public const string ContentCheckLevel1 = "cms_contentCheckLevel1";
            public const string ContentCheckLevel2 = "cms_contentCheckLevel2";
            public const string ContentCheckLevel3 = "cms_contentCheckLevel3";
            public const string ContentCheckLevel4 = "cms_contentCheckLevel4";
            public const string ContentCheckLevel5 = "cms_contentCheckLevel5";
        }

        public static DateTime SqlMinValue { get; } = new DateTime(1754, 1, 1, 0, 0, 0, 0);

        public static Encoding Gb2312 { get; } = Encoding.GetEncoding("gb2312");

        public const char PageSeparatorChar = '/';

        public const string AdminLogin = "后台管理员登录";

        public const string Unauthorized = "权限不足，访问被禁止";
        public const string NotFound = "请求的资源不存在";

        public static string GetSessionIdCacheKey(int userId)
        {
            return $"SESSION-ID-{userId}";
        }

        public const string ReturnAndNewline = "\r\n";//回车换行
        public const string Html5Empty = @"<html><head><meta charset=""utf-8""></head><body></body></html>";

        public const string Ellipsis = "...";

        public const int PageSize = 25;//后台分页数
        public const string HideElementStyle = "display:none";
        public const string ShowElementStyle = "display:";

        public const string TitleImageAppendix = "t_";
        public const string SmallImageAppendix = "s_";
    }
}
