using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalProgressBar : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public Literal LtlScripts;

        public static string GetOpenWindowStringWithCreateContentsOneByOne(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("生成内容页",
                PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
                {
                    {"channelId", channelId.ToString()},
                    {"CreateContentsOneByOne", true.ToString()}
                }), "contentIdCollection", "请选择需要生成的内容！", 500, 360);
        }

        public static string GetOpenWindowStringWithCreateByTemplate(int siteId, int templateId)
        {
            return LayerUtils.GetOpenScript("生成页面",
                PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
                {
                    {"templateID", templateId.ToString()},
                    {"CreateByTemplate", true.ToString()}
                }), 500, 360);
        }

        public static string GetRedirectUrlStringWithCreateChannelsOneByOne(int siteId,
            string channelIdCollection, string isIncludeChildren, string isCreateContents)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
            {
                {"CreateChannelsOneByOne", true.ToString()},
                {"ChannelIDCollection", channelIdCollection},
                {"IsIncludeChildren", isIncludeChildren},
                {"IsCreateContents", isCreateContents}
            });
        }

        public static string GetRedirectUrlStringWithCreateContentsOneByOne(int siteId, int channelId,
            string contentIdCollection)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"CreateContentsOneByOne", true.ToString()},
                {"contentIdCollection", contentIdCollection}
            });
        }

        public static string GetRedirectUrlStringWithCreateByIDsCollection(int siteId, string idsCollection)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
            {
                {"CreateByIDsCollection", true.ToString()},
                {"IDsCollection", idsCollection}
            });
        }

        public static string GetOpenWindowStringWithSiteTemplateDownload(int siteId, string downloadUrl)
        {
            return LayerUtils.GetOpenScript("下载在线模板",
                PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
                {
                    {"SiteTemplateDownload", true.ToString()},
                    {"DownloadUrl", TranslateUtils.EncryptStringBySecretKey(downloadUrl)}
                }), 460, 360);
        }

        public static string GetRedirectUrlStringWithSiteTemplateDownload(int siteId, string downloadUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
            {
                {"SiteTemplateDownload", true.ToString()},
                {"DownloadUrl", TranslateUtils.EncryptStringBySecretKey(downloadUrl)}
            });
        }

        public static string GetOpenWindowStringWithSiteTemplateZip(int siteId, string directoryName)
        {
            return LayerUtils.GetOpenScript("站点模板压缩",
                PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
                {
                    {"SiteTemplateZip", true.ToString()},
                    {"DirectoryName", directoryName}
                }), 460, 360);
        }

        public static string GetOpenWindowStringWithSiteTemplateUnZip(int siteId, string fileName)
        {
            return LayerUtils.GetOpenScript("站点模板解压",
                PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
                {
                    {"SiteTemplateUnZip", true.ToString()},
                    {"FileName", fileName}
                }), 460, 360);
        }

        public static string GetRedirectUrlStringWithPluginDownload(int siteId, string downloadUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(ModalProgressBar), new NameValueCollection
            {
                {"PluginDownload", true.ToString()},
                {"DownloadUrl", downloadUrl}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (Body.IsQueryExists("CreateChannelsOneByOne") && Body.IsQueryExists("ChannelIDCollection"))
            {
                foreach (var channelId in TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ChannelIDCollection")))
                {
                    CreateManager.CreateChannel(SiteId, channelId);
                }

                LayerUtils.CloseAndOpenPageCreateStatus(Page);
                //PageUtils.Redirect(ModalTipMessage.GetRedirectUrlString(SiteId, "已成功将栏目放入生成队列"));
            }
            else if (Body.IsQueryExists("CreateContentsOneByOne") && Body.IsQueryExists("channelId") &&
                     Body.IsQueryExists("contentIdCollection"))
            {
                foreach (var contentId in TranslateUtils.StringCollectionToIntList(Body.GetQueryString("contentIdCollection")))
                {
                    CreateManager.CreateContent(SiteId, Body.GetQueryInt("channelId"),
                        contentId);
                }

                LayerUtils.CloseAndOpenPageCreateStatus(Page);
                //PageUtils.Redirect(ModalTipMessage.GetRedirectUrlString(SiteId, "已成功将内容放入生成队列"));
            }
            else if (Body.IsQueryExists("CreateByTemplate") && Body.IsQueryExists("templateID"))
            {
                CreateManager.CreateFile(SiteId, Body.GetQueryInt("templateID"));

                LayerUtils.CloseAndOpenPageCreateStatus(Page);
                //PageUtils.Redirect(ModalTipMessage.GetRedirectUrlString(SiteId, "已成功将文件放入生成队列"));
            }
            else if (Body.IsQueryExists("CreateByIDsCollection") && Body.IsQueryExists("IDsCollection"))
            {
                foreach (var channelIdContentId in
                    TranslateUtils.StringCollectionToStringCollection(Body.GetQueryString("IDsCollection")))
                {
                    var pair = channelIdContentId.Split('_');
                    CreateManager.CreateContent(SiteId, TranslateUtils.ToInt(pair[0]),
                        TranslateUtils.ToInt(pair[1]));
                }

                LayerUtils.CloseAndOpenPageCreateStatus(Page);
                //PageUtils.Redirect(ModalTipMessage.GetRedirectUrlString(SiteId, "已成功将文件放入生成队列"));
            }
            //---------------------------------------------------------------------------------------//
            else if (Body.IsQueryExists("SiteTemplateDownload"))
            {
                var userKeyPrefix = StringUtils.Guid();

                var downloadUrl = TranslateUtils.DecryptStringBySecretKey(Body.GetQueryString("DownloadUrl"));
                var directoryName = PathUtils.GetFileNameWithoutExtension(downloadUrl);

                var parameters = AjaxOtherService.GetSiteTemplateDownloadParameters(downloadUrl, directoryName, userKeyPrefix);
                LtlScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxOtherService.GetSiteTemplateDownloadUrl(), parameters, userKeyPrefix, AjaxOtherService.GetCountArrayUrl());
            }
            else if (Body.IsQueryExists("SiteTemplateZip"))
            {
                var userKeyPrefix = StringUtils.Guid();

                var parameters = AjaxOtherService.GetSiteTemplateZipParameters(Body.GetQueryString("DirectoryName"), userKeyPrefix);
                LtlScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxOtherService.GetSiteTemplateZipUrl(), parameters, userKeyPrefix, AjaxOtherService.GetCountArrayUrl());
            }
            else if (Body.IsQueryExists("SiteTemplateUnZip"))
            {
                var userKeyPrefix = StringUtils.Guid();

                var parameters = AjaxOtherService.GetSiteTemplateUnZipParameters(Body.GetQueryString("FileName"), userKeyPrefix);
                LtlScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxOtherService.GetSiteTemplateUnZipUrl(), parameters, userKeyPrefix, AjaxOtherService.GetCountArrayUrl());
            }
            //---------------------------------------------------------------------------------------//
            else if (Body.IsQueryExists("PluginDownload"))
            {
                var userKeyPrefix = StringUtils.Guid();

                var parameters = AjaxOtherService.GetPluginDownloadParameters(Body.GetQueryString("DownloadUrl"), userKeyPrefix);
                LtlScripts.Text =
                    AjaxManager.RegisterProgressTaskScript(AjaxOtherService.GetPluginDownloadUrl(), parameters, userKeyPrefix, AjaxOtherService.GetCountArrayUrl());
            }
        }
    }
}
