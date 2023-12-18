using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using SSCMS.Models;
using System.Collections.Generic;

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

            var wxMenus = new List<WxMenu>();
            var menuTypes = new List<Select<string>>();

            var site = await _siteRepository.GetAsync(request.SiteId);
            var isWxEnabled = await _wxManager.IsEnabledAsync(site);
            
            if (isWxEnabled)
            {
                wxMenus = await _wxMenuRepository.GetMenusAsync(request.SiteId);
                menuTypes = ListUtils.GetEnums<WxMenuType>().Select(x => new Select<string>
                {
                    Label = x.GetDisplayName(),
                    Value = x.GetValue()
                }).ToList();
            }

            return new GetResult
            {
                IsWxEnabled = isWxEnabled,
                WxMenus = wxMenus,
                MenuTypes = menuTypes
            };
        }
    }
}
