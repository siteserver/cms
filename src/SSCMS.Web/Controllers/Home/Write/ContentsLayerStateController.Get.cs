using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class ContentsLayerStateController
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

            var contentChecks = await _contentCheckRepository.GetCheckListAsync(content.SiteId, content.ChannelId, request.ContentId);
            contentChecks.ForEach(async x =>
            {
                x.Set("State", CheckManager.GetCheckState(site, x.Checked, x.CheckedLevel));
                x.Set("AdminName", await _administratorRepository.GetDisplayAsync(x.AdminId));
            });

            return new GetResult
            {
                ContentChecks = contentChecks,
                Content = content,
                State = CheckManager.GetCheckState(site, content)
            };
        }
    }
}
