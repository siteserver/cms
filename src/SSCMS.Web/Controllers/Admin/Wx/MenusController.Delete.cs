using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class MenusController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<WxMenusResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxMenus))
            {
                return Unauthorized();
            }

            await _wxMenuRepository.DeleteAsync(request.SiteId, request.Id);

            return new WxMenusResult
            {
                WxMenus = await _wxMenuRepository.GetMenusAsync(request.SiteId)
            };
        }
    }
}
