using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using SSCMS.Dto;
using SSCMS.Extensions;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class MenusController
    {
        [HttpPost, Route(RouteActionsPush)]
        public async Task<ActionResult<BoolResult>> Push([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Menus))
            {
                return Unauthorized();
            }

            var openMenus = await _openMenuRepository.GetOpenMenusAsync(request.SiteId);
            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);
            var (success, token, errorMessage) = _openManager.GetWxAccessToken(account.WxAppId, account.WxAppSecret);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var resultFull = new GetMenuResultFull
            {
                menu = new MenuFull_ButtonGroup
                {
                    button = new List<MenuFull_RootButton>()
                }
            };

            foreach (var firstMenu in openMenus.Where(x => x.ParentId == 0))
            {
                var root = new MenuFull_RootButton
                {
                    name = firstMenu.Text,
                    type = firstMenu.MenuType.GetValue(),
                    url = firstMenu.Url, 
                    key = firstMenu.Key,
                    sub_button = new List<MenuFull_RootButton>()
                };
                foreach (var child in openMenus.Where(x => x.ParentId == firstMenu.Id))
                {
                    root.sub_button.Add(new MenuFull_RootButton
                    {
                        name = child.Text,
                        type = child.MenuType.GetValue(),
                        url = child.Url,
                        key = child.Key
                    });
                }

                resultFull.menu.button.Add(root);
            }

            var buttonGroup = CommonApi.GetMenuFromJsonResult(resultFull, new ButtonGroup()).menu;
            var result = CommonApi.CreateMenu(token, buttonGroup);

            if (result.errmsg != "ok")
            {
                return this.Error(result.errmsg);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
