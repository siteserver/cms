using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Api.Stl;
using SS.CMS.Framework;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlElement;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsPageContentsController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public ActionsPageContentsController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(ApiRouteActionsPageContents.Route)]
        public async Task<string> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var stlPageContentsElement = TranslateUtils.DecryptStringBySecretKey(request.StlPageContentsElement, WebConfigUtils.SecretKey);

            var nodeInfo = await DataProvider.ChannelRepository.GetAsync(request.PageChannelId);
            var templateInfo = await DataProvider.TemplateRepository.GetAsync(request.TemplateId);
            var pageInfo = await PageInfo.GetPageInfoAsync(nodeInfo.Id, 0, site, templateInfo, new Dictionary<string, object>());
            pageInfo.User = auth.User;

            var contextInfo = new ContextInfo(pageInfo);

            var stlPageContents = await StlPageContents.GetAsync(stlPageContentsElement, pageInfo, contextInfo);

            var pageHtml = await stlPageContents.ParseAsync(request.TotalNum, request.CurrentPageIndex, request.PageCount, false);

            return pageHtml;
        }
    }
}
