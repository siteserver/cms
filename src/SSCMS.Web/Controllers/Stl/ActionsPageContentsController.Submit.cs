using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsPageContentsController
    {
        [HttpPost, Route(Constants.RouteStlActionsPageContents)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            var user = await _authManager.GetUserAsync();

            var site = await _siteRepository.GetAsync(request.SiteId);
            var stlPageContentsElement = _settingsManager.Decrypt(request.StlPageContentsElement);

            var channel = await _channelRepository.GetAsync(request.PageChannelId);
            var template = await _templateRepository.GetAsync(request.TemplateId);

            await _parseManager.InitAsync(EditMode.Default, site, channel.Id, 0, template);
            _parseManager.PageInfo.User = user;

            var stlPageContents = await StlPageContents.GetAsync(stlPageContentsElement, _parseManager);

            var pageHtml = await stlPageContents.ParseAsync(request.TotalNum, request.CurrentPageIndex, request.PageCount, false);

            return new SubmitResult
            {
                Html = pageHtml
            };
        }
    }
}
