using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.StlParser.StlElement;
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
                var rest = new Rest(Request);

                var siteId = rest.GetPostInt("siteId");
                var pageChannelId = rest.GetPostInt("pageChannelId");
                if (pageChannelId == 0)
                {
                    pageChannelId = siteId;
                }
                var pageContentId = rest.GetPostInt("pageContentId");
                var pageTemplateId = rest.GetPostInt("pageTemplateId");
                var isPageRefresh = rest.GetPostBool("isPageRefresh");
                var templateContent = TranslateUtils.DecryptStringBySecretKey(rest.GetPostString("templateContent"));
                var ajaxDivId = AttackUtils.FilterSqlAndXss(rest.GetPostString("ajaxDivId"));

                var channelId = rest.GetPostInt("channelId");
                if (channelId == 0)
                {
                    channelId = pageChannelId;
                }
                var contentId = rest.GetPostInt("contentId");
                if (contentId == 0)
                {
                    contentId = pageContentId;
                }

                var pageUrl = TranslateUtils.DecryptStringBySecretKey(rest.GetPostString("pageUrl"));
                var pageIndex = rest.GetPostInt("pageNum");
                if (pageIndex > 0)
                {
                    pageIndex--;
                }

                var queryString = PageUtils.GetQueryStringFilterXss(PageUtils.UrlDecode(HttpContext.Current.Request.RawUrl));
                queryString.Remove("siteId");

                return Ok(new
                {
                    Html = StlDynamic.ParseDynamicContent(siteId, channelId, contentId, pageTemplateId, isPageRefresh, templateContent, pageUrl, pageIndex, ajaxDivId, queryString, rest.UserInfo)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
