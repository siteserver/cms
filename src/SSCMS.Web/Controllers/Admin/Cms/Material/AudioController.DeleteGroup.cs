using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class AudioController
    {
        [HttpDelete, Route(RouteActionsDeleteGroup)]
        public async Task<ActionResult<BoolResult>> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.MaterialAudio))
            {
                return Unauthorized();
            }

            await _materialGroupRepository.DeleteAsync(MaterialType.Audio, request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
