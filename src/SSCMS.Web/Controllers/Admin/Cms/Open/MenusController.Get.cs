using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class MenusController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Menus))
            {
                return Unauthorized();
            }

            var openMenus = await _openMenuRepository.GetOpenMenusAsync(request.SiteId);
            var menuTypes = ListUtils.GetEnums<OpenMenuType>().Select(x => new Select<string>
            {
                Label = x.GetDisplayName(),
                Value = x.GetValue()
            });

            return new GetResult
            {
                OpenMenus = openMenus,
                MenuTypes = menuTypes
            };
        }
    }
}
