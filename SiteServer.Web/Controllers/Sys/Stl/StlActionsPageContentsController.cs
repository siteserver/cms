using System;
using System.Web.Http;
using SiteServer.Utils;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsPageContentsController : ApiController
    {
        [HttpPost, Route(ActionsPageContents.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var context = new RequestContext();

                var siteId = context.GetPostInt("siteId");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var pageNodeId = context.GetPostInt("pageNodeId");
                var templateId = context.GetPostInt("templateId");
                var totalNum = context.GetPostInt("totalNum");
                var pageCount = context.GetPostInt("pageCount");
                var currentPageIndex = context.GetPostInt("currentPageIndex");
                var stlPageContentsElement = TranslateUtils.DecryptStringBySecretKey(context.GetPostString("stlPageContentsElement"));

                var nodeInfo = ChannelManager.GetChannelInfo(siteId, pageNodeId);
                var templateInfo = TemplateManager.GetTemplateInfo(siteId, templateId);
                var pageInfo = new PageInfo(nodeInfo.Id, 0, siteInfo, templateInfo)
                {
                    UserInfo = context.UserInfo
                };
                var contextInfo = new ContextInfo(pageInfo);

                var stlPageContents = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, false);

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
