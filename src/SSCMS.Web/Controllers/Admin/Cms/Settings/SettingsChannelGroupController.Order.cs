using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsChannelGroupController
    {
        [HttpPost, Route(RouteOrder)]
        public async Task<ActionResult<GetResult>> Order([FromBody] OrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsChannelGroup))
            {
                return Unauthorized();
            }

            for (int i = 0; i < request.Rows; i++)
            {
                var group = await _channelGroupRepository.GetAsync(request.SiteId, request.GroupId);
                if (request.IsUp)
                {
                    await _channelGroupRepository.UpdateTaxisUpAsync(request.SiteId, group.Id, group.Taxis);
                }
                else
                {
                    await _channelGroupRepository.UpdateTaxisDownAsync(request.SiteId, group.Id, group.Taxis);
                }
            }

            var groups = await _channelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}