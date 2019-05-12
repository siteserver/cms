using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
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
            var request = new AuthenticatedRequest(HttpContext.Current.Request);

            var redirectUrl = request.GetQueryString("redirectUrl");
            var encryptedUrl = request.GetQueryString("encryptedUrl");
            var siteId = request.GetQueryInt("siteId");
            var channelId = request.GetQueryInt("channelId");
            var contentId = request.GetQueryInt("contentId");
            var fileTemplateId = request.GetQueryInt("fileTemplateId");
            var specialId = request.GetQueryInt("specialId");

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

            var isLocal = siteInfo.Additional.IsSeparatedWeb || siteInfo.Additional.IsSeparatedAssets;

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

            if (string.IsNullOrEmpty(redirectUrl) || StringUtils.EqualsIgnoreCase(redirectUrl, PageUtils.UnclickedUrl))
            {
                if (siteId == 0)
                {
                    siteId = DataProvider.SiteDao.GetIdByIsRoot();
                }

                if (siteId == 0)
                {
                    redirectUrl = PageUtilsEx.ApplicationPath;
                }
                else
                {
                    var redirectSiteInfo = SiteManager.GetSiteInfo(siteId);
                    redirectUrl = redirectSiteInfo.Additional.IsSeparatedWeb
                        ? ApiRoutePreview.GetSiteUrl(siteId)
                        : redirectSiteInfo.Additional.WebUrl;
                }
            }

            return redirectUrl;
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Load()
        {
            try
            {
                var request = new AuthenticatedRequest(HttpContext.Current.Request);

                var redirectUrl = request.GetPostString("redirectUrl");
                var encryptedUrl = request.GetPostString("encryptedUrl");
                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentId = request.GetPostInt("contentId");
                var fileTemplateId = request.GetPostInt("fileTemplateId");
                var specialId = request.GetPostInt("specialId");

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