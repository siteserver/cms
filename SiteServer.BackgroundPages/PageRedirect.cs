using System;
using System.Collections.Specialized;
using SiteServer.Utils;
using SiteServer.CMS.Controllers.Preview;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages
{
	public class PageRedirect : BasePage
    {
	    protected override bool IsSinglePage => true;

	    protected override bool IsAccessable => true;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageRedirect), new NameValueCollection
            {
                {nameof(siteId), siteId.ToString() }
            });
        }

        public static string GetRedirectUrlToChannel(int siteId, int channelId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageRedirect), new NameValueCollection
            {
                {nameof(siteId), siteId.ToString() },
                {nameof(channelId), channelId.ToString() }
            });
        }

        public static string GetRedirectUrlToContent(int siteId, int channelId, int contentId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageRedirect), new NameValueCollection
            {
                {nameof(siteId), siteId.ToString() },
                {nameof(channelId), channelId.ToString() },
                {nameof(contentId), contentId.ToString() }
            });
        }

        public static string GetRedirectUrlToFile(int siteId, int templateId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageRedirect), new NameValueCollection
            {
                {nameof(siteId), siteId.ToString() },
                {nameof(templateId), templateId.ToString() }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var siteId = TranslateUtils.ToInt(Request.QueryString["siteId"]);
            var channelId = TranslateUtils.ToInt(Request.QueryString["channelId"]);
            var contentId = TranslateUtils.ToInt(Request.QueryString["contentId"]);
            var templateId = TranslateUtils.ToInt(Request.QueryString["templateId"]);
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var url = string.Empty;
            var isLocal = siteInfo.Additional.IsSeparatedWeb || siteInfo.Additional.IsSeparatedAssets;

            if (siteId > 0 && channelId > 0 && contentId > 0)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                url = PageUtility.GetContentUrl(siteInfo, nodeInfo, contentId, isLocal);
            }
            else if (siteId > 0 && channelId > 0)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                url = PageUtility.GetChannelUrl(siteInfo, nodeInfo, isLocal);
            }
            else if (siteId > 0 && templateId > 0)
            {
                url = PageUtility.GetFileUrl(siteInfo, templateId, isLocal);
            }
            else if (siteId > 0)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, siteId);
                url = PageUtility.GetChannelUrl(siteInfo, nodeInfo, isLocal);
            }

            //if (siteInfo.Additional.IsSeparatedWeb)
            //{
            //    if (siteId > 0 && channelId > 0 && contentId > 0)
            //    {
            //        url = PreviewApi.GetContentUrl(siteId, channelId, contentId);
            //    }
            //    else if (siteId > 0 && channelId > 0)
            //    {
            //        url = PreviewApi.GetChannelUrl(siteId, channelId);
            //    }
            //    else if (siteId > 0 && templateId > 0)
            //    {
            //        url = PreviewApi.GetFileUrl(siteId, templateId);
            //    }
            //    else if (siteId > 0)
            //    {
            //        url = PreviewApi.GetSiteUrl(siteId);
            //    }
            //}
            //else
            //{
                
            //}

            if (string.IsNullOrEmpty(url) || StringUtils.EqualsIgnoreCase(url, PageUtils.UnclickedUrl))
            {
                DefaultRedirect(siteId);
            }
            else
            {
                PageUtils.Redirect(url);
            }
        }

        private static void DefaultRedirect(int siteId)
        {
            if (siteId == 0)
            {
                siteId = DataProvider.SiteDao.GetIdByIsRoot();
            }
            if (siteId != 0)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var url = siteInfo.Additional.IsSeparatedWeb
                    ? ApiRoutePreview.GetSiteUrl(siteId)
                    : siteInfo.Additional.WebUrl;
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
