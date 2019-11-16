using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    [OpenApiIgnore]
    public class SysStlActionsPageContentsController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsPageContents.Route)]
        public async Task<IHttpActionResult> Main()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();

                var siteId = request.GetPostInt("siteId");
                var site = await SiteManager.GetSiteAsync(siteId);
                var pageChannelId = request.GetPostInt("pageChannelId");
                var templateId = request.GetPostInt("templateId");
                var totalNum = request.GetPostInt("totalNum");
                var pageCount = request.GetPostInt("pageCount");
                var currentPageIndex = request.GetPostInt("currentPageIndex");
                var stlPageContentsElement = WebConfigUtils.DecryptStringBySecretKey(request.GetPostString("stlPageContentsElement"));

                var nodeInfo = await ChannelManager.GetChannelAsync(siteId, pageChannelId);
                var templateInfo = await TemplateManager.GetTemplateAsync(siteId, templateId);
                var pageInfo = await PageInfo.GetPageInfoAsync(nodeInfo.Id, 0, site, templateInfo, new Dictionary<string, object>());
                pageInfo.User = request.User;

                var contextInfo = new ContextInfo(pageInfo);

                var stlPageContents = await StlPageContents.GetAsync(stlPageContentsElement, pageInfo, contextInfo);

                var pageHtml = await stlPageContents.ParseAsync(totalNum, currentPageIndex, pageCount, false);

                return Ok(pageHtml);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
