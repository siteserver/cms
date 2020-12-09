using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class MenusController
    {
        [HttpPost, Route(RouteActionsPull)]
        public async Task<ActionResult<WxMenusResult>> Pull([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxMenus))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);

            if (success)
            {
                await _wxManager.PullMenuAsync(token, request.SiteId);
            }

            var wxMenus = await _wxMenuRepository.GetMenusAsync(request.SiteId);

            return new WxMenusResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                WxMenus = wxMenus
            };
        }
    }
}
