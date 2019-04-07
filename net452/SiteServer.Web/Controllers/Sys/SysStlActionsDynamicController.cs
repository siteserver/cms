using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsDynamicController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsDynamic.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                var pageChannelId = Request.GetPostInt("pageChannelId");
                if (pageChannelId == 0)
                {
                    pageChannelId = siteId;
                }
                var pageContentId = Request.GetPostInt("pageContentId");
                var pageTemplateId = Request.GetPostInt("pageTemplateId");
                var isPageRefresh = Request.GetPostBool("isPageRefresh");
                var templateContent = TranslateUtils.DecryptStringBySecretKey(Request.GetPostString("templateContent"));
                var ajaxDivId = AttackUtils.FilterSqlAndXss(Request.GetPostString("ajaxDivId"));

                var channelId = Request.GetPostInt("channelId");
                if (channelId == 0)
                {
                    channelId = pageChannelId;
                }
                var contentId = Request.GetPostInt("contentId");
                if (contentId == 0)
                {
                    contentId = pageContentId;
                }

                var pageUrl = TranslateUtils.DecryptStringBySecretKey(Request.GetPostString("pageUrl"));
                var pageIndex = Request.GetPostInt("pageNum");
                if (pageIndex > 0)
                {
                    pageIndex--;
                }

                var queryString = PageUtils.GetQueryStringFilterXss(PageUtils.UrlDecode(HttpContext.Current.Request.RawUrl));
                queryString.Remove("siteId");

                var userInfo = UserManager.GetUserInfoByUserId(rest.UserId);

                return Ok(new
                {
                    Html = StlDynamic.ParseDynamicContent(siteId, channelId, contentId, pageTemplateId, isPageRefresh, templateContent, pageUrl, pageIndex, ajaxDivId, queryString, userInfo)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
