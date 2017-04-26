using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalProgressBar : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public Literal RegisterScripts;

        public static string GetOpenWindowStringWithCreateContentsOneByOne(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenLayerStringWithCheckBoxValue("生成内容页",
                PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()},
                    {"CreateContentsOneByOne", true.ToString()}
                }), "ContentIDCollection", "请选择需要生成的内容！", 500, 360);
        }

        public static string GetOpenWindowStringWithCreateByTemplate(int publishmentSystemId, int templateId)
        {
            return PageUtils.GetOpenLayerString("生成页面",
                PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"templateID", templateId.ToString()},
                    {"CreateByTemplate", true.ToString()}
                }), 500, 360);
        }

        public static string GetRedirectUrlStringWithCreateChannelsOneByOne(int publishmentSystemId,
            string channelIdCollection, string isIncludeChildren, string isCreateContents)
        {
            return PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"CreateChannelsOneByOne", true.ToString()},
                {"ChannelIDCollection", channelIdCollection},
                {"IsIncludeChildren", isIncludeChildren},
                {"IsCreateContents", isCreateContents}
            });
        }

        public static string GetRedirectUrlStringWithCreateContentsOneByOne(int publishmentSystemId, int nodeId,
            string contentIdCollection)
        {
            return PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"CreateContentsOneByOne", true.ToString()},
                {"ContentIDCollection", contentIdCollection}
            });
        }

        public static string GetRedirectUrlStringWithCreateByIDsCollection(int publishmentSystemId, string idsCollection)
        {
            return PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"CreateByIDsCollection", true.ToString()},
                {"IDsCollection", idsCollection}
            });
        }

        public static string GetRedirectUrlStringWithGather(int publishmentSystemId, string gatherRuleNameCollection)
        {
            return PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"Gather", true.ToString()},
                {"GatherRuleNameCollection", gatherRuleNameCollection}
            });
        }

        public static string GetOpenWindowStringWithGather(int publishmentSystemId)
        {
            return PageUtils.GetOpenLayerStringWithCheckBoxValue("采集内容",
                PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"Gather", "True"}
                }), "GatherRuleNameCollection", "请选择需要开始采集的采集规则名称!", 660, 360);
        }

        public static string GetRedirectUrlStringWithGatherDatabase(int publishmentSystemId,
            string gatherRuleNameCollection)
        {
            return PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GatherDatabase", true.ToString()},
                {"GatherRuleNameCollection", gatherRuleNameCollection}
            });
        }

        public static string GetOpenWindowStringWithGatherDatabase(int publishmentSystemId)
        {
            return PageUtils.GetOpenLayerStringWithCheckBoxValue("数据库采集",
                PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"GatherDatabase", "True"}
                }), "GatherRuleNameCollection", "请选择需要开始采集的采集规则名称!", 660, 360);
        }

        public static string GetRedirectUrlStringWithGatherFile(int publishmentSystemId, string gatherRuleNameCollection)
        {
            return PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GatherFile", true.ToString()},
                {"GatherRuleNameCollection", gatherRuleNameCollection}
            });
        }

        public static string GetOpenWindowStringWithGatherFile(int publishmentSystemId)
        {
            return PageUtils.GetOpenLayerStringWithCheckBoxValue("文件采集",
                PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"GatherFile", "True"}
                }), "GatherRuleNameCollection", "请选择需要开始采集的采集规则名称!", 660, 360);
        }

        public static string GetOpenWindowStringWithSiteTemplateDownload(string downloadUrl, string directoryName)
        {
            return PageUtils.GetOpenLayerString("下载在线模板",
                PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
                {
                    {"SiteTemplateDownload", "True"},
                    {"DownloadUrl", downloadUrl},
                    {"DirectoryName", directoryName}
                }), 460, 360);
        }

        public static string GetRedirectUrlStringWithSiteTemplateDownload(string downloadUrl, string directoryName)
        {
            return PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
            {
                {"SiteTemplateDownload", true.ToString()},
                {"DownloadUrl", downloadUrl},
                {"DirectoryName", directoryName}
            });
        }

        public static string GetOpenWindowStringWithSiteTemplateZip(string directoryName)
        {
            return PageUtils.GetOpenLayerString("站点模板压缩",
                PageUtils.GetCmsUrl(nameof(ModalProgressBar), new NameValueCollection
                {
                    {"SiteTemplateZip", "True"},
                    {"DirectoryName", directoryName}
                }), 460, 360);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (Body.IsQueryExists("Gather") && Body.IsQueryExists("GatherRuleNameCollection"))
            {
                var userKeyPrefix = StringUtils.Guid();
                var parameters = AjaxGatherService.GetGatherParameters(PublishmentSystemId, Body.GetQueryString("GatherRuleNameCollection"), userKeyPrefix);
                RegisterScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxGatherService.GetGatherUrl(), parameters, userKeyPrefix, AjaxGatherService.GetCountArrayUrl());
            }
            else if (Body.IsQueryExists("GatherDatabase") && Body.IsQueryExists("GatherRuleNameCollection"))
            {
                var userKeyPrefix = StringUtils.Guid();
                var parameters = AjaxGatherService.GetGatherDatabaseParameters(PublishmentSystemId,
                    Body.GetQueryString("GatherRuleNameCollection"), userKeyPrefix);
                RegisterScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxGatherService.GetGatherDatabaseUrl(), parameters, userKeyPrefix, AjaxGatherService.GetCountArrayUrl());
            }
            else if (Body.IsQueryExists("GatherFile") && Body.IsQueryExists("GatherRuleNameCollection"))
            {
                var userKeyPrefix = StringUtils.Guid();
                var parameters = AjaxGatherService.GetGatherFileParameters(PublishmentSystemId,
                    Body.GetQueryString("GatherRuleNameCollection"), userKeyPrefix);
                RegisterScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxGatherService.GetGatherFileUrl(), parameters,
                        userKeyPrefix, AjaxGatherService.GetCountArrayUrl());
            }
            //----------------------------------------------------------------------------------------//
            else if (Body.IsQueryExists("CreateChannelsOneByOne") && Body.IsQueryExists("ChannelIDCollection"))
            {
                foreach (var channelId in TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ChannelIDCollection")))
                {
                    CreateManager.CreateChannel(PublishmentSystemId, channelId);
                }

                PageUtils.Redirect(ModalTipMessage.GetRedirectUrlString("已成功将栏目放入生成队列"));
            }
            else if (Body.IsQueryExists("CreateContentsOneByOne") && Body.IsQueryExists("NodeID") &&
                     Body.IsQueryExists("ContentIDCollection"))
            {
                foreach (var contentId in TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection")))
                {
                    CreateManager.CreateContent(PublishmentSystemId, Body.GetQueryInt("NodeID"),
                        contentId);
                }

                PageUtils.Redirect(ModalTipMessage.GetRedirectUrlString("已成功将内容放入生成队列"));
            }
            else if (Body.IsQueryExists("CreateByTemplate") && Body.IsQueryExists("templateID"))
            {
                CreateManager.CreateFile(PublishmentSystemId, Body.GetQueryInt("templateID"));

                PageUtils.Redirect(ModalTipMessage.GetRedirectUrlString("已成功将文件放入生成队列"));
            }
            else if (Body.IsQueryExists("CreateByIDsCollection") && Body.IsQueryExists("IDsCollection"))
            {
                foreach (var nodeIdContentId in
                    TranslateUtils.StringCollectionToStringCollection(Body.GetQueryString("IDsCollection")))
                {
                    var pair = nodeIdContentId.Split('_');
                    CreateManager.CreateContent(PublishmentSystemId, TranslateUtils.ToInt(pair[0]),
                        TranslateUtils.ToInt(pair[1]));
                }

                PageUtils.Redirect(ModalTipMessage.GetRedirectUrlString("已成功将文件放入生成队列"));
            }
            //---------------------------------------------------------------------------------------//
            else if (Body.IsQueryExists("SiteTemplateDownload"))
            {
                var userKeyPrefix = StringUtils.Guid();

                var parameters = AjaxOtherService.GetSiteTemplateDownloadParameters(Body.GetQueryString("DownloadUrl"),
                    Body.GetQueryString("DirectoryName"), userKeyPrefix);
                RegisterScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxOtherService.GetSiteTemplateDownloadUrl(), parameters, userKeyPrefix, AjaxOtherService.GetCountArrayUrl());
            }
            else if (Body.IsQueryExists("SiteTemplateZip"))
            {
                var userKeyPrefix = StringUtils.Guid();

                var parameters = AjaxOtherService.GetSiteTemplateZipParameters(Body.GetQueryString("DirectoryName"), userKeyPrefix);
                RegisterScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxOtherService.GetSiteTemplateZipUrl(), parameters, userKeyPrefix, AjaxOtherService.GetCountArrayUrl());
            }
        }
    }
}
