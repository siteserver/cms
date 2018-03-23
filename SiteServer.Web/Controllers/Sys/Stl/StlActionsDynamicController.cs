using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsDynamicController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsDynamic.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var request = new AuthRequest();

                var siteId = request.GetPostInt("siteId");
                var pageChannelId = request.GetPostInt("pageChannelId");
                if (pageChannelId == 0)
                {
                    pageChannelId = siteId;
                }
                var pageContentId = request.GetPostInt("pageContentId");
                var pageTemplateId = request.GetPostInt("pageTemplateId");
                var isPageRefresh = request.GetPostBool("isPageRefresh");
                var templateContent = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("templateContent"));
                var ajaxDivId = PageUtils.FilterSqlAndXss(request.GetPostString("ajaxDivId"));

                var channelId = request.GetPostInt("channelId");
                if (channelId == 0)
                {
                    channelId = pageChannelId;
                }
                var contentId = request.GetPostInt("contentId");
                if (contentId == 0)
                {
                    contentId = pageContentId;
                }

                var pageUrl = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("pageUrl"));
                var pageIndex = request.GetPostInt("pageNum");
                if (pageIndex > 0)
                {
                    pageIndex--;
                }

                var queryString = PageUtils.GetQueryStringFilterXss(PageUtils.UrlDecode(HttpContext.Current.Request.RawUrl));
                queryString.Remove("siteId");

                return Ok(new
                {
                    Html = StlDynamic.ParseDynamicContent(siteId, channelId, contentId, pageTemplateId, isPageRefresh, templateContent, pageUrl, pageIndex, ajaxDivId, queryString, request.UserInfo)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
