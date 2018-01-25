using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public class ConfigManager
    {
        private const string CacheKey = "SiteServer.CMS.Core.ConfigManager";
        private static readonly object LockObject = new object();

        public const string IdSite = "Site";
        public const string IdPlugins = "Plugins";
        public const string IdSettings = "Settings";

        public class Permissions
        {
            public class Plugins
            {
                public const string Add = "plugins_add";
                public const string Management = "plugins_management";
            }

            public class Settings
            {
                public const string SiteAdd = "settings_siteAdd";
                public const string Site = "settings_site";
                public const string Admin = "settings_admin";
                public const string User = "settings_user";
                public const string Chart = "settings_chart";
                public const string Log = "settings_log";
                public const string Utility = "settings_utility";
            }

            public class WebSite
            {
                private WebSite() { }

                public const string ContentTrash = "cms_contentTrash";                  //内容回收站
                public const string Template = "cms_template";                          //显示管理
                public const string Configration = "cms_configration";                  //设置管理
                public const string Create = "cms_create";                              //生成管理
            }

            public class Channel
            {
                private Channel() { }
                public const string ContentView = "cms_contentView";
                public const string ContentAdd = "cms_contentAdd";
                public const string ContentEdit = "cms_contentEdit";
                public const string ContentDelete = "cms_contentDelete";
                public const string ContentTranslate = "cms_contentTranslate";
                public const string ContentArchive = "cms_contentArchive";
                public const string ContentOrder = "cms_contentOrder";
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
        }

        public static string GetTopMenuName(string menuId)
        {
            var retval = string.Empty;
            if (menuId == IdSite)
            {
                retval = "站点管理";
            }
            else if (menuId == IdPlugins)
            {
                retval = "插件管理";
            }
            else if (menuId == IdSettings)
            {
                retval = "系统管理";
            }
            return retval;
        }

        public static string GetLeftMenuName(string menuId)
        {
            var retval = string.Empty;
            if (menuId == Cms.LeftMenu.IdContent)
            {
                retval = "信息管理";
            }
            else if (menuId == Cms.LeftMenu.IdTemplate)
            {
                retval = "显示管理";
            }
            else if (menuId == Cms.LeftMenu.IdConfigration)
            {
                retval = "设置管理";
            }
            else if (menuId == Cms.LeftMenu.IdCreate)
            {
                retval = "生成管理";
            }
            return retval;
        }

        public class Cms
        {
            public const string AppId = "cms";

            public class LeftMenu
            {
                public const string IdContent = "Content";
                public const string IdTemplate = "Template";
                public const string IdConfigration = "Configration";
                public const string IdCreate = "Create";
            }
        }

        public static ConfigInfo Instance
        {
            get
            {
                var retval = CacheUtils.Get<ConfigInfo>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = CacheUtils.Get<ConfigInfo>(CacheKey);
                    if (retval == null)
                    {
                        retval = DataProvider.ConfigDao.GetConfigInfo();
                        CacheUtils.Insert(CacheKey, retval);
                    }
                }

                return retval;
            }
        }

        public static bool IsChanged
        {
            set
            {
                if (value)
                {
                    CacheUtils.Remove(CacheKey);
                }
            }
        }

        private ConfigManager() { }

        public static SystemConfigInfo SystemConfigInfo => Instance.SystemConfigInfo;

        //public static string Cipherkey
        //{
        //    get
        //    {
        //        var cipherkey = Instance.SystemConfigInfo.Cipherkey;
        //        if (string.IsNullOrEmpty(cipherkey))
        //        {
        //            var s = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'I', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };//枚举数组
        //            var r = new Random();
        //            for (var i = 0; i < 8; i++)
        //            {
        //                cipherkey += s[r.Next(0, s.Length)].ToString();
        //            }

        //            Instance.SystemConfigInfo.Cipherkey = cipherkey;

        //            DataProvider.ConfigDao.Update(Instance);
        //        }
        //        return cipherkey;
        //    }
        //}
    }
}
