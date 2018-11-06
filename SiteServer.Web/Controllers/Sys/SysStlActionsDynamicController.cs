using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Plugin.Impl;
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
                var request = new RequestImpl();

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
                var ajaxDivId = AttackUtils.FilterSqlAndXss(request.GetPostString("ajaxDivId"));

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
