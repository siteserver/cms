using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class MenusController
    {
        [HttpPost, Route(RouteActionsPull)]
        public async Task<ActionResult<OpenMenusResult>> Pull([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.OpenMenus))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _openManager.GetWxAccessTokenAsync(request.SiteId);

            if (success)
            {
                await _openManager.PullMenuAsync(token, request.SiteId);
            }

            var openMenus = await _openMenuRepository.GetOpenMenusAsync(request.SiteId);

            return new OpenMenusResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                OpenMenus = openMenus
            };
        }
    }
}
