using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsPageContentsController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsPageContents.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var rest = new Rest(Request);

                var siteId = rest.GetPostInt("siteId");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var pageChannelId = rest.GetPostInt("pageChannelId");
                var templateId = rest.GetPostInt("templateId");
                var totalNum = rest.GetPostInt("totalNum");
                var pageCount = rest.GetPostInt("pageCount");
                var currentPageIndex = rest.GetPostInt("currentPageIndex");
                var stlPageContentsElement = TranslateUtils.DecryptStringBySecretKey(rest.GetPostString("stlPageContentsElement"));

                var nodeInfo = ChannelManager.GetChannelInfo(siteId, pageChannelId);
                var templateInfo = TemplateManager.GetTemplateInfo(siteId, templateId);
                var pageInfo = new PageInfo(nodeInfo.Id, 0, siteInfo, templateInfo, new Dictionary<string, object>())
                {
                    UserInfo = rest.UserInfo
                };
                var contextInfo = new ContextInfo(pageInfo);

                var stlPageContents = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo);

                var pageHtml = stlPageContents.Parse(totalNum, currentPageIndex, pageCount, false);

                return Ok(pageHtml);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
