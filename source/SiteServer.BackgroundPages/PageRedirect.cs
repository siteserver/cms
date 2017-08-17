using System;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages
{
	public class PageRedirect : BasePage
    {
	    protected override bool IsSinglePage => true;

	    protected override bool IsAccessable => true;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageRedirect), new NameValueCollection
            {
                {nameof(publishmentSystemId), publishmentSystemId.ToString() }
            });
        }

        public static string GetRedirectUrlToChannel(int publishmentSystemId, int channelId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageRedirect), new NameValueCollection
            {
                {nameof(publishmentSystemId), publishmentSystemId.ToString() },
                {nameof(channelId), channelId.ToString() }
            });
        }

        public static string GetRedirectUrlToContent(int publishmentSystemId, int channelId, int contentId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageRedirect), new NameValueCollection
            {
                {nameof(publishmentSystemId), publishmentSystemId.ToString() },
                {nameof(channelId), channelId.ToString() },
                {nameof(contentId), contentId.ToString() }
            });
        }

        public static string GetRedirectUrlToFile(int publishmentSystemId, int templateId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageRedirect), new NameValueCollection
            {
                {nameof(publishmentSystemId), publishmentSystemId.ToString() },
                {nameof(templateId), templateId.ToString() }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var publishmentSystemId = TranslateUtils.ToInt(Request.QueryString["publishmentSystemId"]);
            var channelId = TranslateUtils.ToInt(Request.QueryString["channelId"]);
            var contentId = TranslateUtils.ToInt(Request.QueryString["contentId"]);
            var templateId = TranslateUtils.ToInt(Request.QueryString["templateId"]);

            var url = string.Empty;
            if (publishmentSystemId > 0 && channelId > 0 && contentId > 0)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                url = PageUtility.GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, true);
            }
            else if (publishmentSystemId > 0 && channelId > 0)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                url = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo, true);
            }
            else if (publishmentSystemId > 0 && templateId > 0)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                url = PageUtility.GetFileUrl(publishmentSystemInfo, templateId, true);
            }
            else if (publishmentSystemId > 0)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, publishmentSystemId);
                url = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo, true);
            }
            else if (channelId > 0 && contentId > 0)
            {
                publishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(channelId);
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                url = PageUtility.GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, true);
            }
            else if (channelId > 0)
            {
                publishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(channelId);
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                url = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo, true);
            }

            if (string.IsNullOrEmpty(url) || StringUtils.EqualsIgnoreCase(url, PageUtils.UnclickedUrl))
            {
                DefaultRedirect(publishmentSystemId);
            }
            else
            {
                PageUtils.Redirect(url);
            }
        }

        private static void DefaultRedirect(int publishmentSystemId)
        {
            if (publishmentSystemId == 0)
            {
                publishmentSystemId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
            }
            if (publishmentSystemId != 0)
            {
                var url = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId).PublishmentSystemUrl;
                PageUtils.Redirect(url);
            }
            else
            {
                var url = PageUtils.ApplicationPath;
                PageUtils.Redirect(url);
            }
        }
    }
}
