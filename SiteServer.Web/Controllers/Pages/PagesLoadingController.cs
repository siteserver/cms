using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.Preview;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages
{
    [RoutePrefix("pages/loading")]
    public class PagesLoadingController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public void Redirect()
        {
            var rest = new Rest(Request);

            var redirectUrl = rest.GetQueryString("redirectUrl");
            var encryptedUrl = rest.GetQueryString("encryptedUrl");
            var siteId = rest.GetQueryInt("siteId");
            var channelId = rest.GetQueryInt("channelId");
            var contentId = rest.GetQueryInt("contentId");
            var fileTemplateId = rest.GetQueryInt("fileTemplateId");
            var specialId = rest.GetQueryInt("specialId");

            var url = GetUrl(redirectUrl, encryptedUrl, siteId, channelId, contentId, fileTemplateId,
                specialId);

            HttpContext.Current.Response.Redirect(url);
        }

        private static string GetUrl(string redirectUrl, string encryptedUrl, int siteId, int channelId, int contentId, int fileTemplateId, int specialId)
        {
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return redirectUrl;
            }

            if (!string.IsNullOrEmpty(encryptedUrl))
            {
                return TranslateUtils.DecryptStringBySecretKey(encryptedUrl);
            }

            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var isLocal = siteInfo.IsSeparatedWeb || siteInfo.IsSeparatedAssets;

            if (siteId > 0 && channelId > 0 && contentId > 0)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                redirectUrl = PageUtility.GetContentUrl(siteInfo, nodeInfo, contentId, isLocal);
            }
            else if (siteId > 0 && channelId > 0)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                redirectUrl = PageUtility.GetChannelUrl(siteInfo, nodeInfo, isLocal);
            }
            else if (siteId > 0 && fileTemplateId > 0)
            {
                redirectUrl = PageUtility.GetFileUrl(siteInfo, fileTemplateId, isLocal);
            }
            else if (siteId > 0 && specialId > 0)
            {
                redirectUrl = PageUtility.GetSpecialUrl(siteInfo, specialId, isLocal);
            }
            else if (siteId > 0)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, siteId);
                redirectUrl = PageUtility.GetChannelUrl(siteInfo, nodeInfo, isLocal);
            }

            if (string.IsNullOrEmpty(redirectUrl) || StringUtils.EqualsIgnoreCase(redirectUrl, PageUtils.UnClickedUrl))
            {
                if (siteId == 0)
                {
                    siteId = DataProvider.Site.GetIdByIsRoot();
                }

                if (siteId == 0)
                {
                    redirectUrl = PageUtils.ApplicationPath;
                }
                else
                {
                    var redirectSiteInfo = SiteManager.GetSiteInfo(siteId);
                    redirectUrl = redirectSiteInfo.IsSeparatedWeb
                        ? ApiRoutePreview.GetSiteUrl(siteId)
                        : redirectSiteInfo.WebUrl;
                }
            }

            return redirectUrl;
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Load()
        {
            try
            {
                var rest = new Rest(Request);

                var redirectUrl = rest.GetPostString("redirectUrl");
                var encryptedUrl = rest.GetPostString("encryptedUrl");
                var siteId = rest.GetPostInt("siteId");
                var channelId = rest.GetPostInt("channelId");
                var contentId = rest.GetPostInt("contentId");
                var fileTemplateId = rest.GetPostInt("fileTemplateId");
                var specialId = rest.GetPostInt("specialId");

                var url = GetUrl(redirectUrl, encryptedUrl, siteId, channelId, contentId, fileTemplateId,
                    specialId);

                return Ok(new
                {
                    Value = url
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}