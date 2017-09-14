using System;
using System.Text;
using System.Collections.Generic;

namespace BaiRong.Core
{
    public class AppManager
    {
        private AppManager() { }

        public const string Version = "5.1";

        public static string GetFullVersion()
        {
            return GetFullVersion(Version);
        }

        public static string GetFullVersion(string version)
        {
            var retval = version;
            return retval;
        }

        public static double GetVersionDouble(string version)
        {
            version = version.Replace("_", ".").ToLower().Trim();
            version = version.Replace(" ", string.Empty);
            if (StringUtils.GetCount(".", version) == 2)
            {
                var theVersion = version;
                version = theVersion.Substring(0, theVersion.LastIndexOf(".", StringComparison.Ordinal));
                version += theVersion.Substring(theVersion.LastIndexOf(".", StringComparison.Ordinal) + 1);
            }
            return TranslateUtils.ToDouble(version);
        }

        public static bool IsNeedUpgrade()
        {
            return !StringUtils.EqualsIgnoreCase(Version, BaiRongDataProvider.ConfigDao.GetDatabaseVersion());
        }

        public static bool IsNeedInstall()
        {
            var isNeedInstall = !BaiRongDataProvider.ConfigDao.IsInitialized();
            if (isNeedInstall)
            {
                isNeedInstall = !BaiRongDataProvider.ConfigDao.IsInitialized();
            }
            return isNeedInstall;
        }

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
                public const string SiteAdd = "settings_site_add";
                public const string SiteManagement = "settings_site_management";
                public const string AdminManagement = "settings_admin_management";
                public const string UserManagement = "settings_user_management";
                public const string Integration = "settings_integration";
                public const string Service = "settings_service";
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
                public const string CommentCheck = "cms_commentCheck";
                public const string CommentDelete = "cms_commentDelete";
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

        public class WeiXin
        {
            public const string AppId = "weixin";

            public class TopMenu
            {
                public const string IdSiteManagement = "SiteManagement";
                public const string IdSiteConfiguration = "SiteConfiguration";

                public static string GetText(string menuId)
                {
                    var retval = string.Empty;
                    if (menuId == IdSiteManagement)
                    {
                        retval = "微信管理";
                    }
                    else if (menuId == IdSiteConfiguration)
                    {
                        retval = "微信设置";
                    }
                    return retval;
                }
            }

            public class LeftMenu
            {
                public const string IdAccounts = "Accounts";

                public const string IdConfiguration = "Configuration";

                public class Function
                {
                    //Function
                    public const string IdCoupon = "Coupon";
                    public const string IdScratch = "Scratch";
                    public const string IdBigWheel = "BigWheel";
                    public const string IdGoldEgg = "GoldEgg";
                    public const string IdFlap = "Flap";
                    public const string IdYaoYao = "YaoYao";
                    public const string IdVote = "Vote";
                    public const string IdMessage = "Message";
                    public const string IdAppointment = "Appointment";
                    public const string IdConference = "Conference";
                    public const string IdMap = "Map";
                    public const string IdView360 = "View360";
                    public const string IdAlbum = "Album";
                    public const string IdSearch = "Search";
                    public const string IdStore = "Store";
                    public const string IdWifi = "Wifi";
                    public const string IdCard = "Card";
                    public const string IdCollect = "Collect";
                }

                public static string GetText(string menuId)
                {
                    string retval = string.Empty;
                    if (menuId == IdAccounts)
                    {
                        retval = "公共账号";
                    }
                    return retval;
                }

                public static string GetSubText(string menuId)
                {
                    string retval = string.Empty;
                    //Accounts
                    //Function
                    if (menuId == Function.IdCoupon)
                    {
                        retval = "优惠券";
                    }
                    else if (menuId == Function.IdScratch)
                    {
                        retval = "刮刮卡";
                    }
                    else if (menuId == Function.IdBigWheel)
                    {
                        retval = "大转盘";
                    }
                    else if (menuId == Function.IdGoldEgg)
                    {
                        retval = "砸金蛋";
                    }
                    else if (menuId == Function.IdFlap)
                    {
                        retval = "大翻牌";
                    }
                    else if (menuId == Function.IdYaoYao)
                    {
                        retval = "摇摇乐";
                    }
                    else if (menuId == Function.IdVote)
                    {
                        retval = "微投票";
                    }
                    else if (menuId == Function.IdMessage)
                    {
                        retval = "微留言";
                    }
                    else if (menuId == Function.IdAppointment)
                    {
                        retval = "微预约";
                    }
                    else if (menuId == Function.IdConference)
                    {
                        retval = "微会议";
                    }
                    else if (menuId == Function.IdMap)
                    {
                        retval = "微导航";
                    }
                    else if (menuId == Function.IdView360)
                    {
                        retval = "360全景";
                    }
                    else if (menuId == Function.IdAlbum)
                    {
                        retval = "微相册";
                    }
                    else if (menuId == Function.IdSearch)
                    {
                        retval = "微搜索";
                    }
                    else if (menuId == Function.IdStore)
                    {
                        retval = "微门店";
                    }
                    else if (menuId == Function.IdWifi)
                    {
                        retval = "微Wifi";
                    }
                    else if (menuId == Function.IdCard)
                    {
                        retval = "会员卡";
                    }
                    else if (menuId == Function.IdCollect)
                    {
                        retval = "微收集";
                    }
                    return retval;
                }
            }

            public class Permission
            {
                public class WebSite
                {
                    private WebSite() { }


                    public const string Info = "weixin_info";                       //账户信息
                    public const string Chart = "weixin_chart";                     //运营图表
                    public const string Menu = "weixin_menu";                       //菜单
                    public const string TextReply = "weixin_textReply";             //文本回复
                    public const string ImageReply = "weixin_imageReply";           //图文回复
                    public const string SetReply = "weixin_setReply";               //回复设置


                    public const string Coupon = "weixin_coupon";               //优惠券管理
                    public const string Scratch = "weixin_scratch";             //刮刮卡管理
                    public const string BigWheel = "weixin_bigWheel";           //大转盘管理
                    public const string GoldEgg = "weixin_goldEgg";             //砸金蛋管理
                    public const string Flap = "weixin_flap";                   //大翻牌管理
                    public const string YaoYao = "weixin_yaoYao";               //摇摇乐管理
                    public const string Vote = "weixin_vote";                   //微投票管理
                    public const string Message = "weixin_message";             //微留言管理
                    public const string Appointment = "weixin_appointment";            //微预约管理

                    public const string Conference = "weixin_conference";       //微会议管理
                    public const string Map = "weixin_map";                     //微导航管理
                    public const string View360 = "weixin_view360";             //全景管理
                    public const string Album = "weixin_album";                 //微相册管理
                    public const string Search = "weixin_search";               //微搜索管理
                    public const string Store = "weixin_store";                 //微门店管理
                    public const string Wifi = "weixin_wifi";                   //微wifi管理
                    public const string Card = "weixin_card";                   //微会员管理
                    public const string Collect = "weixin_collect";             //微征集管理
                }
            }
        }

        public static List<string> GetAppIdList()
        {
            return new List<string> { Cms.AppId, WeiXin.AppId };
        }

        public static bool IsWeiXin()
        {
            return FileUtils.IsFileExists(PathUtils.GetMenusPath(WeiXin.AppId, "Management.config"));
        }

        public static void Upgrade(string version, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(version) && BaiRongDataProvider.ConfigDao.GetDatabaseVersion() != version)
            {
                var errorBuilder = new StringBuilder();
                BaiRongDataProvider.DatabaseDao.Upgrade(WebConfigUtils.DatabaseType, errorBuilder);

                //升级数据库

                errorMessage = $"<!--{errorBuilder}-->";
            }

            var configInfo = BaiRongDataProvider.ConfigDao.GetConfigInfo();
            configInfo.DatabaseVersion = version;
            configInfo.IsInitialized = true;
            configInfo.UpdateDate = DateTime.Now;
            BaiRongDataProvider.ConfigDao.Update(configInfo);
        }
    }
}
