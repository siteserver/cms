using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
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
                var request = new RequestImpl();

                var siteId = request.GetPostInt("siteId");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var pageChannelId = request.GetPostInt("pageChannelId");
                var templateId = request.GetPostInt("templateId");
                var totalNum = request.GetPostInt("totalNum");
                var pageCount = request.GetPostInt("pageCount");
                var currentPageIndex = request.GetPostInt("currentPageIndex");
                var stlPageContentsElement = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("stlPageContentsElement"));

                var nodeInfo = ChannelManager.GetChannelInfo(siteId, pageChannelId);
                var templateInfo = TemplateManager.GetTemplateInfo(siteId, templateId);
                var pageInfo = new PageInfo(nodeInfo.Id, 0, siteInfo, templateInfo, new Dictionary<string, object>())
                {
                    UserInfo = request.UserInfo
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
