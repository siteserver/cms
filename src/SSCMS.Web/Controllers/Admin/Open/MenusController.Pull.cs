using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Open
{
    public partial class MenusController
    {
        [HttpPost, Route(RouteActionsPull)]
        public async Task<ActionResult<List<MenuFull_RootButton>>> Pull([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.OpenPermissions.Menus))
            {
                return Unauthorized();
            }

            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);
            var (success, token, errorMessage) = _openManager.GetWxAccessToken(account.WxAppId, account.WxAppSecret);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            var result = CommonApi.GetMenu(token);

            if (result == null)
            {
                return this.Error("微信公众号未获取到菜单设置!");
            }

            await _openMenuRepository.DeleteAllAsync(request.SiteId);

            var json = result.menu.button.ToJson();
            var buttons = TranslateUtils.JsonDeserialize<List<MenuFull_RootButton>>(json);

            foreach (var button in buttons)
            {
                var first = new OpenMenu
                {
                    SiteId = request.SiteId,
                    ParentId = 0,
                    Taxis = 0,
                    Text = button.name,
                    MenuType = TranslateUtils.ToEnum(button.type, OpenMenuType.View),
                    Key = button.key,
                    Url = button.url,
                    AppId = button.appid,
                    PagePath = button.pagepath,
                    MediaId = button.media_id
                };
                var menuId = await _openMenuRepository.InsertAsync(first);
                if (button.sub_button != null && button.sub_button.Count > 0)
                {
                    foreach (var sub in button.sub_button)
                    {
                        var child = new OpenMenu
                        {
                            SiteId = request.SiteId,
                            ParentId = menuId,
                            Taxis = 0,
                            Text = sub.name,
                            MenuType = TranslateUtils.ToEnum(sub.type, OpenMenuType.View),
                            Key = sub.key,
                            Url = sub.url,
                            AppId = sub.appid,
                            PagePath = sub.pagepath,
                            MediaId = sub.media_id
                        };
                        await _openMenuRepository.InsertAsync(child);
                    }
                }
            }

            return buttons;

            //foreach (var firstButton in result.menu.button)
            //{
            //    var btn = (MenuFull_RootButton) firstButton;
            //}

            //return result;
        }
    }
}
