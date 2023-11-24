using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerStateController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            var content = await _contentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null) return this.Error(Constants.ErrorNotFound);

            var contentChecks = await _contentCheckRepository.GetCheckListAsync(content.SiteId, content.ChannelId, request.ContentId);
            contentChecks.ForEach(async x =>
            {
                x.Set("State", CheckManager.GetCheckState(site, x.Checked, x.CheckedLevel));
                var admin = await _administratorRepository.GetByUserIdAsync(x.AdminId);
                if (admin != null)
                {
                    x.Set("AdminName", _administratorRepository.GetDisplay(admin));
                    x.Set("AdminGuid", admin.Guid);
                }
            });

            var isCheckable = await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ContentsCheck);

            return new GetResult
            {
                ContentChecks = contentChecks,
                Content = content,
                State = CheckManager.GetCheckState(site, content),
                IsCheckable = isCheckable,
            };
        }
    }
}