using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Config
{
    
    [RoutePrefix("pages/settings/configHomeMenu")]
    public class PagesConfigHomeMenuController : ApiController
    {
        private const string Route = "";
        private const string RouteReset = "actions/reset";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = await UserMenuManager.GetAllUserMenuListAsync(),
                    Groups = await UserGroupManager.GetUserGroupListAsync()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                await DataProvider.UserMenuDao.DeleteAsync(id);

                return Ok(new
                {
                    Value = await UserMenuManager.GetAllUserMenuListAsync()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit([FromBody] UserMenu menuInfo)
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                if (menuInfo.Id == 0)
                {
                    await DataProvider.UserMenuDao.InsertAsync(menuInfo);

                    await request.AddAdminLogAsync("新增用户菜单", $"用户菜单:{menuInfo.Text}");
                }
                else if (menuInfo.Id > 0)
                {
                    await DataProvider.UserMenuDao.UpdateAsync(menuInfo);

                    await request.AddAdminLogAsync("修改用户菜单", $"用户菜单:{menuInfo.Text}");
                }

                return Ok(new
                {
                    Value = await UserMenuManager.GetAllUserMenuListAsync()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteReset)]
        public async Task<IHttpActionResult> Reset()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Config))
                {
                    return Unauthorized();
                }

                foreach (var userMenuInfo in await UserMenuManager.GetAllUserMenuListAsync())
                {
                    await DataProvider.UserMenuDao.DeleteAsync(userMenuInfo.Id);
                }

                await request.AddAdminLogAsync("重置用户菜单");

                return Ok(new
                {
                    Value = await UserMenuManager.GetAllUserMenuListAsync()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
