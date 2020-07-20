using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class MenusController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<OpenMenusResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Menus))
            {
                return Unauthorized();
            }

            if (request.Id == 0)
            {
                await _openMenuRepository.InsertAsync(new OpenMenu
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
                var openMenu = await _openMenuRepository.GetAsync(request.SiteId, request.Id);
                openMenu.ParentId = request.ParentId;
                openMenu.Taxis = request.Taxis;
                openMenu.Text = request.Text;
                openMenu.MenuType = request.MenuType;
                openMenu.Key = request.Key;
                openMenu.Url = request.Url;
                openMenu.AppId = request.AppId;
                openMenu.PagePath = request.PagePath;
                openMenu.MediaId = request.MediaId;

                await _openMenuRepository.UpdateAsync(openMenu);

                await _authManager.AddAdminLogAsync("修改自定义菜单", $"自定义菜单:{request.Text}");
            }

            return new OpenMenusResult
            {
                OpenMenus = await _openMenuRepository.GetOpenMenusAsync(request.SiteId)
            };
        }
    }
}
