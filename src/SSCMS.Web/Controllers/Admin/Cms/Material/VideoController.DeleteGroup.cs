using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class VideoController
    {
        [HttpDelete, Route(RouteActionsDeleteGroup)]
        public async Task<ActionResult<BoolResult>> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialVideo))
            {
                return Unauthorized();
            }

            await _materialGroupRepository.DeleteAsync(MaterialType.Video, request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}