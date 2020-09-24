using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsChannelGroupController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsChannelGroup))
            {
                return Unauthorized();
            }

            await _channelGroupRepository.DeleteAsync(request.SiteId, request.GroupName);

            var groups = await _channelGroupRepository.GetChannelGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}