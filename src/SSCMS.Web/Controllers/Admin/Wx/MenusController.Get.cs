using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class MenusController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxMenus))
            {
                return Unauthorized();
            }

            var wxMenus = await _wxMenuRepository.GetMenusAsync(request.SiteId);
            var menuTypes = ListUtils.GetEnums<WxMenuType>().Select(x => new Select<string>
            {
                Label = x.GetDisplayName(),
                Value = x.GetValue()
            });

            return new GetResult
            {
                WxMenus = wxMenus,
                MenuTypes = menuTypes
            };
        }
    }
}
