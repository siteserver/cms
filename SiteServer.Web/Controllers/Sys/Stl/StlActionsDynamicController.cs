using System;
using System.Web;
using System.Web.Http;
using SiteServer.Utils;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Plugin.Model;
using SiteServer.CMS.StlParser.Utility;

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
                var context = new RequestContext();

                var siteId = context.GetPostInt("siteId");
                var pageChannelId = context.GetPostInt("pageChannelId");
                if (pageChannelId == 0)
                {
                    pageChannelId = siteId;
                }
                var pageContentId = context.GetPostInt("pageContentId");
                var pageTemplateId = context.GetPostInt("pageTemplateId");
                var isPageRefresh = context.GetPostBool("isPageRefresh");
                var templateContent = TranslateUtils.DecryptStringBySecretKey(context.GetPostString("templateContent"));
                var ajaxDivId = PageUtils.FilterSqlAndXss(context.GetPostString("ajaxDivId"));

                var channelId = context.GetPostInt("channelId");
                if (channelId == 0)
                {
                    channelId = pageChannelId;
                }
                var contentId = context.GetPostInt("contentId");
                if (contentId == 0)
                {
                    contentId = pageContentId;
                }

                var pageUrl = TranslateUtils.DecryptStringBySecretKey(context.GetPostString("pageUrl"));
                var pageIndex = context.GetPostInt("pageNum");
                if (pageIndex > 0)
                {
                    pageIndex--;
                }

                var queryString = PageUtils.GetQueryStringFilterXss(PageUtils.UrlDecode(HttpContext.Current.Request.RawUrl));
                queryString.Remove("siteId");

                return Ok(new
                {
                    Html = StlUtility.ParseDynamicContent(siteId, channelId, contentId, pageTemplateId, isPageRefresh, templateContent, pageUrl, pageIndex, ajaxDivId, queryString, context.UserInfo)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
