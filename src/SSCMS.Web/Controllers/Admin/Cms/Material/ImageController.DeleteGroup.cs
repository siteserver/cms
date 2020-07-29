using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ImageController
    {
        [HttpDelete, Route(RouteActionsDeleteGroup)]
        public async Task<ActionResult<BoolResult>> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.MaterialImage))
            {
                return Unauthorized();
            }

            await _materialGroupRepository.DeleteAsync(MaterialType.Image, request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
