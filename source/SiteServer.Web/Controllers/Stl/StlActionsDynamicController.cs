using System;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsDynamicController : ApiController
    {
        [HttpPost, Route(ActionsDynamic.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var pageNodeId = body.GetPostInt("pageNodeId");
                if (pageNodeId == 0)
                {
                    pageNodeId = publishmentSystemId;
                }
                var pageContentId = body.GetPostInt("pageContentId");
                var pageTemplateId = body.GetPostInt("pageTemplateId");
                var isPageRefresh = body.GetPostBool("isPageRefresh");
                var templateContent = TranslateUtils.DecryptStringBySecretKey(body.GetPostString("templateContent"));
                var ajaxDivId = PageUtils.FilterSqlAndXss(body.GetPostString("ajaxDivId"));

                var channelId = body.GetPostInt("channelId");
                if (channelId == 0)
                {
                    channelId = pageNodeId;
                }
                var contentId = body.GetPostInt("contentId");
                if (contentId == 0)
                {
                    contentId = pageContentId;
                }

                var pageUrl = TranslateUtils.DecryptStringBySecretKey(body.GetPostString("pageUrl"));
                var pageIndex = body.GetPostInt("pageNum");
                if (pageIndex > 0)
                {
                    pageIndex--;
                }

                var queryString = PageUtils.GetQueryStringFilterXss(PageUtils.UrlDecode(HttpContext.Current.Request.RawUrl));
                queryString.Remove("publishmentSystemID");

                return Ok(new
                {
                    Html = StlUtility.ParseDynamicContent(publishmentSystemId, channelId, contentId, pageTemplateId, isPageRefresh, templateContent, pageUrl, pageIndex, ajaxDivId, queryString, body.UserInfo)
                });
            }
            catch(Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
