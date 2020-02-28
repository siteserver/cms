using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.Api.Stl;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlElement;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsPageContentsController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IParseManager _parseManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateRepository _templateRepository;

        public ActionsPageContentsController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IParseManager parseManager, IConfigRepository configRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _parseManager = parseManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateRepository = templateRepository;
        }

        [HttpPost, Route(ApiRouteActionsPageContents.Route)]
        public async Task<string> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();

            var site = await _siteRepository.GetAsync(request.SiteId);
            var stlPageContentsElement = _settingsManager.Decrypt(request.StlPageContentsElement);

            var nodeInfo = await _channelRepository.GetAsync(request.PageChannelId);
            var templateInfo = await _templateRepository.GetAsync(request.TemplateId);

            var config = await _configRepository.GetAsync();
            var pageInfo = ParsePage.GetPageInfo(_pathManager, config, nodeInfo.Id, 0, site, templateInfo, new Dictionary<string, object>());
            pageInfo.User = auth.User;

            var contextInfo = new ParseContext(pageInfo);

            var stlPageContents = await StlPageContents.GetAsync(stlPageContentsElement, _parseManager);

            var pageHtml = await stlPageContents.ParseAsync(request.TotalNum, request.CurrentPageIndex, request.PageCount, false);

            return pageHtml;
        }
    }
}
