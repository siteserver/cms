using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class MenusController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<OpenMenusResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Menus))
            {
                return Unauthorized();
            }

            await _openMenuRepository.DeleteAsync(request.SiteId, request.Id);

            return new OpenMenusResult
            {
                OpenMenus = await _openMenuRepository.GetOpenMenusAsync(request.SiteId)
            };
        }
    }
}
