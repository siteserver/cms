using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Plugin;
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
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var pageChannelId = Request.GetPostInt("pageChannelId");
                var templateId = Request.GetPostInt("templateId");
                var totalNum = Request.GetPostInt("totalNum");
                var pageCount = Request.GetPostInt("pageCount");
                var currentPageIndex = Request.GetPostInt("currentPageIndex");
                var stlPageContentsElement = TranslateUtils.DecryptStringBySecretKey(Request.GetPostString("stlPageContentsElement"));

                var userInfo = UserManager.GetUserInfoByUserId(rest.UserId);

                var nodeInfo = ChannelManager.GetChannelInfo(siteId, pageChannelId);
                var templateInfo = TemplateManager.GetTemplateInfo(siteId, templateId);
                var pageInfo = new PageInfo(nodeInfo.Id, 0, siteInfo, templateInfo, new Dictionary<string, object>())
                {
                    UserInfo = userInfo
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
