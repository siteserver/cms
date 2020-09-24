using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentGroupController
    {
        [HttpPost, Route(RouteOrder)]
        public async Task<ActionResult<GetResult>> Order([FromBody] OrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsContentGroup))
            {
                return Unauthorized();
            }

            if (request.IsUp)
            {
                await _contentGroupRepository.UpdateTaxisUpAsync(request.SiteId, request.GroupId, request.Taxis);
            }
            else
            {
                await _contentGroupRepository.UpdateTaxisDownAsync(request.SiteId, request.GroupId, request.Taxis);
            }

            var groups = await _contentGroupRepository.GetContentGroupsAsync(request.SiteId);

            return new GetResult
            {
                Groups = groups
            };
        }
    }
}