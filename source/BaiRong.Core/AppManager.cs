using System;
using System.Text;
using System.Collections.Generic;

namespace BaiRong.Core
{
    public class AppManager
    {
        private AppManager() { }

        public const string Version = "5.0";

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

        public const string IdManagement = "Management";
        public const string IdSys = "Sys";
        public const string IdAdmin = "Admin";
        public const string IdUser = "User";
        public const string IdAnalysis = "Analysis";
        public const string IdSettings = "Settings";
        public const string IdService = "Service";

        public class Sys
        {
            public class LeftMenu
            {
                public const string Site = "Site";
                public const string Auxiliary = "Auxiliary";
            }

            public class Permission
            {
                public const string SysSite = "sys_site";
                public const string SysAuxiliary = "sys_auxiliary";
            }
        }

        public class Admin
        {
            public class LeftMenu
            {
                public const string AdminManagement = "AdminManagement";
                public const string AdminConfiguration = "AdminConfiguration";
            }

            public class Permission
            {
                public const string AdminManagement = "admin_management";
                public const string AdminConfiguration = "admin_configuration";
            }
        }

        public class User
        {
            public class LeftMenu
            {
                public const string UserManagement = "UserManagement";
                public const string UserConfiguration = "UserConfiguration";
            }

            public class Permission
            {
                public const string UserManagement = "user_management";
                public const string UserConfiguration = "user_configuration";
            }
        }

        public class Analysis
        {
            public class LeftMenu
            {
                public const string Chart = "Chart";
                public const string Log = "Log";
            }

            public class Permission
            {
                public const string AnalysisChart = "analysis_chart";
                public const string AnalysisLog = "analysis_log";
            }
        }

        public class Settings
        {
            public class LeftMenu
            {
                public const string Config = "Config";
                public const string Restriction = "Restriction";
                public const string Utility = "Utility";
            }

            public class Permission
            {
                public const string SettingsConfig = "settings_config";
                public const string SettingsRestriction = "settings_restriction";
                public const string SettingsUtility = "settings_Utility";
            }
        }

        public class Service
        {
            public class LeftMenu
            {
                public const string Status = "Status";
                public const string Task = "Task";
                public const string ServiceCreate = "ServiceCreate";
            }

            public class Permission
            {
                public const string ServiceStatus = "service_status";
                public const string ServiceTask = "service_task";
                public const string ServiceCreate = "service_create";
            }
        }

        public static string GetTopMenuName(string menuId)
        {
            var retval = string.Empty;
            if (menuId == IdManagement)
            {
                retval = "站点管理";
            }
            else if (menuId == IdSys)
            {
                retval = "系统管理";
            }
            else if (menuId == IdAdmin)
            {
                retval = "管理员管理";
            }
            else if (menuId == IdUser)
            {
                retval = "用户管理";
            }
            else if (menuId == IdAnalysis)
            {
                retval = "统计分析";
            }
            else if (menuId == IdSettings)
            {
                retval = "平台设置";
            }
            else if (menuId == IdService)
            {
                retval = "服务组件";
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
            else if (menuId == Cms.LeftMenu.IdFunction)
            {
                retval = "功能管理";
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
            else if (menuId == Wcm.LeftMenu.IdGovPublic)
            {
                retval = "信息公开";
            }
            else if (menuId == Wcm.LeftMenu.IdGovInteract)
            {
                retval = "互动交流";
            }
            else if (menuId == Service.LeftMenu.Status)
            {
                retval = "服务组件状态";
            }
            else if (menuId == Service.LeftMenu.Task)
            {
                retval = "定时任务管理";
            }
            else if (menuId == Service.LeftMenu.ServiceCreate)
            {
                retval = "服务生成管理";
            }
            else if (menuId == Settings.LeftMenu.Config)
            {
                retval = "平台设置";
            }
            else if (menuId == Settings.LeftMenu.Restriction)
            {
                retval = "后台访问限制";
            }
            else if (menuId == Settings.LeftMenu.Utility)
            {
                retval = "实用工具";
            }
            else if (menuId == Analysis.LeftMenu.Chart)
            {
                retval = "统计图表";
            }
            else if (menuId == Analysis.LeftMenu.Log)
            {
                retval = "运行日志";
            }
            else if (menuId == User.LeftMenu.UserManagement)
            {
                retval = "用户管理";
            }
            else if (menuId == User.LeftMenu.UserConfiguration)
            {
                retval = "设置管理";
            }
            else if (menuId == Admin.LeftMenu.AdminManagement)
            {
                retval = "管理员管理";
            }
            else if (menuId == Admin.LeftMenu.AdminConfiguration)
            {
                retval = "设置管理";
            }
            else if (menuId == Sys.LeftMenu.Site)
            {
                retval = "系统站点管理";
            }
            else if (menuId == Sys.LeftMenu.Auxiliary)
            {
                retval = "辅助表管理";
            }
            return retval;
        }

        public static string GetLeftSubMenuName(string menuId)
        {
            var retval = string.Empty;
            //Function
            if (menuId == Cms.LeftMenu.Function.IdSiteAnalysis)
            {
                retval = "站点数据统计";
            }
            else if (menuId == Cms.LeftMenu.Function.IdInput)
            {
                retval = "提交表单管理";
            }
            else if (menuId == Cms.LeftMenu.Function.IdGather)
            {
                retval = "信息采集管理";
            }
            else if (menuId == Cms.LeftMenu.Function.IdAdvertisement)
            {
                retval = "广告管理";
            }
            else if (menuId == Cms.LeftMenu.Function.IdResume)
            {
                retval = "简历管理";
            }
            else if (menuId == Cms.LeftMenu.Function.IdSeo)
            {
                retval = "搜索引擎优化";
            }
            else if (menuId == Cms.LeftMenu.Function.IdTracking)
            {
                retval = "流量统计管理";
            }
            else if (menuId == Cms.LeftMenu.Function.IdInnerLink)
            {
                retval = "站内链接管理";
            }
            else if (menuId == Cms.LeftMenu.Function.IdBackup)
            {
                retval = "数据备份恢复";
            }
            //Template
            else if (menuId == Cms.LeftMenu.Template.IdTagStyle)
            {
                retval = "模板标签样式";
            }
            //Configuration
            else if (menuId == Cms.LeftMenu.Configuration.IdConfigurationContentModel)
            {
                retval = "内容模型设置";
            }
            else if (menuId == Cms.LeftMenu.Configuration.IdConfigurationGroupAndTags)
            {
                retval = "组别及标签设置";
            }
            else if (menuId == Cms.LeftMenu.Configuration.IdConfigurationUpload)
            {
                retval = "上传设置";
            }
            else if (menuId == Cms.LeftMenu.Configuration.IdConfigurationTask)
            {
                retval = "定时任务管理";
            }
            //GovPublic
            else if (menuId == Wcm.LeftMenu.GovPublic.IdGovPublicContent)
            {
                retval = "主动信息公开";
            }
            else if (menuId == Wcm.LeftMenu.GovPublic.IdGovPublicApply)
            {
                retval = "依申请公开";
            }
            else if (menuId == Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration)
            {
                retval = "主动信息公开设置";
            }
            else if (menuId == Wcm.LeftMenu.GovPublic.IdGovPublicApplyConfiguration)
            {
                retval = "依申请公开设置";
            }
            else if (menuId == Wcm.LeftMenu.GovPublic.IdGovPublicAnalysis)
            {
                retval = "数据统计分析";
            }
            //GovInteract
            else if (menuId == Wcm.LeftMenu.GovInteract.IdGovInteractConfiguration)
            {
                retval = "互动交流设置";
            }
            else if (menuId == Wcm.LeftMenu.GovInteract.IdGovInteractAnalysis)
            {
                retval = "数据统计分析";
            }
            else if (menuId == Cms.LeftMenu.Create.IdConfigurationCreate)
            {
                retval = "页面生成设置";
            }
            return retval;
        }

        public class Cms
        {
            public const string AppId = "cms";

            public class LeftMenu
            {
                public const string IdContent = "Content";
                public const string IdFunction = "Function";
                public const string IdTemplate = "Template";
                public const string IdConfigration = "Configration";
                public const string IdCreate = "Create";

                public class Function
                {
                    public const string IdSiteAnalysis = "SiteAnalysis";
                    public const string IdInput = "Input";
                    public const string IdGather = "Gather";
                    public const string IdAdvertisement = "Advertisement";
                    public const string IdResume = "Resume";
                    public const string IdSeo = "SEO";
                    public const string IdTracking = "Tracking";
                    public const string IdInnerLink = "InnerLink";
                    public const string IdBackup = "Backup";
                }

                public class Template
                {
                    public const string IdTagStyle = "TagStyle";     //模板标签样式
                }

                public class Configuration
                {
                    public const string IdConfigurationContentModel = "ConfigurationContentModel";          //内容模型设置
                    public const string IdConfigurationGroupAndTags = "ConfigurationGroupAndTags";          //组别及标签设置
                    public const string IdConfigurationUpload = "ConfigurationUpload";                      //上传设置
                    public const string IdConfigurationTask = "ConfigurationTask";                          //定时任务管理
                }

                public class Create
                {
                    public const string IdConfigurationCreate = "ConfigurationCreate";                      //页面生成设置
                }
            }

            public class Permission
            {
                public class WebSite
                {
                    private WebSite() { }

                    public const string ContentTrash = "cms_contentTrash";                  //内容回收站

                    public const string SiteAnalysis = "cms_siteAnalysis";                  //站点数据统计
                    public const string Input = "cms_input";                                //提交表单管理
                    public const string Gather = "cms_gather";                              //信息采集管理
                    public const string Advertisement = "cms_advertisement";                //广告管理
                    public const string Resume = "cms_resume";                              //在线招聘管理
                    public const string Seo = "cms_seo";                                    //搜索引擎优化
                    public const string Tracking = "cms_tracking";                          //流量统计管理
                    public const string InnerLink = "cms_innerLink";                        //站内链接管理
                    public const string Backup = "cms_backup";                              //数据备份恢复
                    public const string Archive = "cms_archive";                            //归档内容管理

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
        }

        public class Wcm
        {
            public const string AppId = "wcm";

            public class LeftMenu
            {
                public const string IdGovPublic = "GovPublic";
                public const string IdGovInteract = "GovInteract";

                public class GovPublic
                {
                    public const string IdGovPublicContent = "GovPublicContent";     //主动信息公开
                    public const string IdGovPublicApply = "GovPublicApply";         //依申请公开
                    public const string IdGovPublicContentConfiguration = "GovPublicContentConfiguration";         //主动信息公开设置
                    public const string IdGovPublicApplyConfiguration = "GovPublicApplyConfiguration";         //依申请公开设置
                    public const string IdGovPublicAnalysis = "GovPublicAnalysis";         //数据统计分析
                }

                public class GovInteract
                {
                    public const string IdGovInteractConfiguration = "GovInteractConfiguration";     //互动交流设置
                    public const string IdGovInteractAnalysis = "GovInteractAnalysis";             //数据统计分析
                }
            }

            public class Permission
            {
                public class WebSite
                {
                    private WebSite() { }

                    public const string GovPublicContent = "wcm_govPublicContent";                                      //主动信息公开
                    public const string GovPublicApply = "wcm_govPublicApply";                                          //依申请公开
                    public const string GovPublicContentConfiguration = "wcm_govPublicContentConfiguration";            //主动信息公开设置
                    public const string GovPublicApplyConfiguration = "wcm_govPublicApplyConfiguration";                //依申请公开设置
                    public const string GovPublicAnalysis = "wcm_govPublicAnalysis";                                    //信息公开统计

                    public const string GovInteract = "wcm_govInteract";                                                //互动交流管理
                    public const string GovInteractConfiguration = "wcm_govInteractConfiguration";                      //互动交流设置
                    public const string GovInteractAnalysis = "wcm_govInteractAnalysis";                                //互动交流统计
                }

                public class GovInteract
                {
                    private GovInteract() { }
                    public const string GovInteractView = "wcm_govInteractView";
                    public const string GovInteractAdd = "wcm_govInteractAdd";
                    public const string GovInteractEdit = "wcm_govInteractEdit";
                    public const string GovInteractDelete = "wcm_govInteractDelete";
                    public const string GovInteractSwitchToTranslate = "wcm_govInteractSwitchToTranslate";
                    public const string GovInteractComment = "wcm_govInteractComment";
                    public const string GovInteractAccept = "wcm_govInteractAccept";
                    public const string GovInteractReply = "wcm_govInteractReply";
                    public const string GovInteractCheck = "wcm_govInteractCheck";

                    public static string GetPermissionName(string permission)
                    {
                        var retval = string.Empty;
                        if (permission == GovInteractView)
                        {
                            retval = "浏览办件";
                        }
                        else if (permission == GovInteractAdd)
                        {
                            retval = "新增办件";
                        }
                        else if (permission == GovInteractEdit)
                        {
                            retval = "编辑办件";
                        }
                        else if (permission == GovInteractDelete)
                        {
                            retval = "删除办件";
                        }
                        else if (permission == GovInteractSwitchToTranslate)
                        {
                            retval = "转办转移";
                        }
                        else if (permission == GovInteractComment)
                        {
                            retval = "批示办件";
                        }
                        else if (permission == GovInteractAccept)
                        {
                            retval = "受理办件";
                        }
                        else if (permission == GovInteractReply)
                        {
                            retval = "办理办件";
                        }
                        else if (permission == GovInteractCheck)
                        {
                            retval = "审核办件";
                        }
                        return retval;
                    }
                }
            }

            public class AuxiliaryTableName
            {
                public const string BackgroundContent = AppId + "_Content";
                public const string GovPublicContent = AppId + "_ContentGovPublic";
                public const string GovInteractContent = AppId + "_ContentGovInteract";
                public const string JobContent = AppId + "_ContentJob";
                public const string VoteContent = AppId + "_ContentVote";
                public const string UserDefined = AppId + "_ContentCustom";
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

                public const string IdFunction = "Function";

                public class Function
                {
                    //Accounts
                    public const string IdInfo = "Info";
                    public const string IdChart = "Chart";
                    public const string IdMenu = "Menu";
                    public const string IdTextReply = "TextReply";
                    public const string IdImageReply = "ImageReply";
                    public const string IdSetReply = "SetReply";

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
                    else if (menuId == IdFunction)
                    {
                        retval = "微功能";
                    }
                    return retval;
                }

                public static string GetSubText(string menuId)
                {
                    string retval = string.Empty;
                    //Accounts
                    if (menuId == Function.IdInfo)
                    {
                        retval = "账户信息";
                    }
                    else if (menuId == Function.IdChart)
                    {
                        retval = "运营图表";
                    }
                    else if (menuId == Function.IdMenu)
                    {
                        retval = "自定义菜单";
                    }
                    else if (menuId == Function.IdTextReply)
                    {
                        retval = "关键词文本回复";
                    }
                    else if (menuId == Function.IdImageReply)
                    {
                        retval = "关键词图文回复";
                    }
                    else if (menuId == Function.IdSetReply)
                    {
                        retval = "关键词回复设置";
                    }
                    //Function
                    else if (menuId == Function.IdCoupon)
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
            return new List<string> { Cms.AppId, Wcm.AppId };
        }

        public static string GetAppName(string appId, bool isFullName)
        {
            var retval = string.Empty;
            if (StringUtils.EqualsIgnoreCase(appId, Cms.AppId))
            {
                retval = "SiteServer CMS";
                if (isFullName) retval += " 内容管理系统";
            }
            else if (StringUtils.EqualsIgnoreCase(appId, Wcm.AppId))
            {
                retval = "SiteServer WCM";
                if (isFullName) retval += " 内容协作平台";
            }

            return retval;
        }

        public static bool IsWcm()
        {
            return FileUtils.IsFileExists(PathUtils.GetMenusPath(Wcm.AppId, "Management.config"));
        }

        public static void Upgrade(string version, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(version) && BaiRongDataProvider.ConfigDao.GetDatabaseVersion() != version)
            {
                var isMySql = WebConfigUtils.IsMySql;
                var errorBuilder = new StringBuilder();
                BaiRongDataProvider.DatabaseDao.Upgrade(isMySql, errorBuilder);

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
