using System;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageActions : BasePageCms
	{
	    protected override bool IsSinglePage => true;

	    protected override bool IsAccessable => true;

        public const string TypeRedirect = "Redirect";

        public static string GetRedirectUrl(int publishmentSystemId, int channelId)
        {
            return PageUtils.GetCmsUrl(nameof(PageActions), new NameValueCollection
            {
                {"type", TypeRedirect },
                {"publishmentSystemID", publishmentSystemId.ToString() },
                {"channelID", channelId.ToString() }
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int channelId, int contentId)
        {
            return PageUtils.GetCmsUrl(nameof(PageActions), new NameValueCollection
            {
                {"type", TypeRedirect },
                {"publishmentSystemID", publishmentSystemId.ToString() },
                {"channelID", channelId.ToString() },
                {"contentID", contentId.ToString() }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = PageUtils.FilterSql(Request.QueryString["type"]);
            if (string.IsNullOrEmpty(type))
            {
                type = TypeRedirect;
            }

            if (StringUtils.EqualsIgnoreCase(type, TypeRedirect))
            {
                Redirect();
            }
        }

        private void Redirect()
        {
            string url;
            if (Body.IsQueryExists("u"))
            {
                url = Request.QueryString["u"];
                if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                {
                    url = "http://" + url;
                }
                PageUtils.Redirect(url);
            }
            else if (Body.IsQueryExists("publishmentSystemID") && Body.IsQueryExists("channelID") && Body.IsQueryExists("contentID"))
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request.QueryString["publishmentSystemID"]);
                var channelId = TranslateUtils.ToInt(Request.QueryString["channelID"]);
                var contentId = TranslateUtils.ToInt(Request.QueryString["contentID"]);

                if (contentId != 0)
                {
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                    url = PageUtility.GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, true);
                    if (!url.Equals(PageUtils.UnclickedUrl))
                    {
                        PageUtils.Redirect(url);
                    }
                    else
                    {
                        Redirect_DefaultDirection();
                    }
                }
                else
                {
                    Redirect_DefaultDirection();
                }
            }
            else if (Body.IsQueryExists("channelID") && Body.IsQueryExists("contentID"))
            {
                var channelId = TranslateUtils.ToInt(Request.QueryString["channelID"]);
                var contentId = TranslateUtils.ToInt(Request.QueryString["contentID"]);

                if (contentId != 0)
                {
                    var publishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(channelId);
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                    url = PageUtility.GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, true);
                    if (!url.Equals(PageUtils.UnclickedUrl))
                    {
                        PageUtils.Redirect(url);
                    }
                    else
                    {
                        Redirect_DefaultDirection();
                    }
                }
                else
                {
                    Redirect_DefaultDirection();
                }
            }
            else if (Body.IsQueryExists("channelID"))
            {
                var channelId = TranslateUtils.ToInt(Request.QueryString["channelID"]);

                if (channelId != 0)
                {
                    var publishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(channelId);
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                    url = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo, true);
                    if (!url.Equals(PageUtils.UnclickedUrl))
                    {
                        PageUtils.Redirect(url);
                    }
                    else
                    {
                        Redirect_DefaultDirection();
                    }
                }
                else
                {
                    Redirect_DefaultDirection();
                }
            }
            else if (Body.IsQueryExists("channelindex"))
            {
                var channelIndex = Request.QueryString["channelindex"];
                var publishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
                if (publishmentSystemId == 0)
                {
                    publishmentSystemId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
                }
                if (publishmentSystemId != 0)
                {
                    var channelId = DataProvider.NodeDao.GetNodeIdByNodeIndexName(publishmentSystemId, channelIndex);
                    if (channelId != 0)
                    {
                        var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                        url = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo);
                        if (!url.Equals(PageUtils.UnclickedUrl))
                        {
                            PageUtils.Redirect(url);
                        }
                        else
                        {
                            Redirect_DefaultDirection();
                        }
                    }
                    else
                    {
                        Redirect_DefaultDirection();
                    }
                }
                else
                {
                    Redirect_DefaultDirection();
                }
            }
            else
            {
                Redirect_DefaultDirection();
            }
        }

        private void Redirect_DefaultDirection()
        {
            string url;
            if (Request.QueryString["ErrorUrl"] != null)
            {
                url = Request.QueryString["ErrorUrl"];
                url = PageUtils.ParseNavigationUrl(url);
                PageUtils.Redirect(url);
            }
            var publishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
            if (publishmentSystemId == 0)
            {
                publishmentSystemId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
            }
            if (publishmentSystemId != 0)
            {
                url = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId).PublishmentSystemUrl;
                PageUtils.Redirect(url);
            }
            else
            {
                url = WebConfigUtils.ApplicationPath;
                PageUtils.Redirect(url);
            }
        }
    }
}
