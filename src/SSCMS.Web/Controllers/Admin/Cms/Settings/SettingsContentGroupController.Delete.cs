using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentGroupController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsContentGroup))
            {
                return Unauthorized();
            }

            await _contentGroupRepository.DeleteAsync(request.SiteId, request.GroupName);

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}