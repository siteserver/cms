using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using SSCMS.Dto;
using SSCMS.Extensions;
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

            var json = result.menu.button.ToJson();
            var buttons = TranslateUtils.JsonDeserialize<List<MenuFull_RootButton>>(json);

            foreach (var button in buttons)
            {
                
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
