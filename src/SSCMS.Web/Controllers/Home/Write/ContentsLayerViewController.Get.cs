using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            var content = await _contentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null || content.UserId != _authManager.UserId) return this.Error(Constants.ErrorNotFound);

            content.Set(ColumnsManager.CheckState, CheckManager.GetCheckState(site, content));

            var channelName = await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId);

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var attributes = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            return new GetResult
            {
                Content = content,
                ChannelName = channelName,
                Attributes = attributes
            };
        }
    }
}
