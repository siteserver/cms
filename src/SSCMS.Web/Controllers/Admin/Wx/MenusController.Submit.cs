using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class MenusController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<WxMenusResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxMenus))
            {
                return Unauthorized();
            }

            if (request.Id == 0)
            {
                await _wxMenuRepository.InsertAsync(new WxMenu
                {
                    SiteId = request.SiteId,
                    ParentId = request.ParentId,
                    Taxis = request.Taxis,
                    Text = request.Text,
                    MenuType = request.MenuType,
                    Key = request.Key,
                    Url = request.Url,
                    AppId = request.AppId,
                    PagePath = request.PagePath,
                    MediaId = request.MediaId
                });

                await _authManager.AddAdminLogAsync("新增自定义菜单", $"自定义菜单:{request.Text}");
            }
            else if (request.Id > 0)
            {
                var wxMenu = await _wxMenuRepository.GetAsync(request.SiteId, request.Id);
                wxMenu.ParentId = request.ParentId;
                wxMenu.Taxis = request.Taxis;
                wxMenu.Text = request.Text;
                wxMenu.MenuType = request.MenuType;
                wxMenu.Key = request.Key;
                wxMenu.Url = request.Url;
                wxMenu.AppId = request.AppId;
                wxMenu.PagePath = request.PagePath;
                wxMenu.MediaId = request.MediaId;

                await _wxMenuRepository.UpdateAsync(wxMenu);

                await _authManager.AddAdminLogAsync("修改自定义菜单", $"自定义菜单:{request.Text}");
            }

            return new WxMenusResult
            {
                WxMenus = await _wxMenuRepository.GetMenusAsync(request.SiteId)
            };
        }
    }
}
