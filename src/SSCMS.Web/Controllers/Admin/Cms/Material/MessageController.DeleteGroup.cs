using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class MessageController
    {
        [HttpDelete, Route(RouteActionsDeleteGroup)]
        public async Task<ActionResult<BoolResult>> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            await _materialGroupRepository.DeleteAsync(MaterialType.Message, request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
