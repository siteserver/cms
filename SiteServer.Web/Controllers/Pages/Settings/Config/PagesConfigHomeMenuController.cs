using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigHomeMenu))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = await DataProvider.UserMenuRepository.GetUserMenuListAsync(),
                    Groups = await DataProvider.UserGroupRepository.GetUserGroupListAsync()
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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigHomeMenu))
                {
                    return Unauthorized();
                }

                var id = request.GetPostInt("id");

                await DataProvider.UserMenuRepository.DeleteAsync(id);

                return Ok(new
                {
                    Value = await DataProvider.UserMenuRepository.GetUserMenuListAsync()
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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigHomeMenu))
                {
                    return Unauthorized();
                }

                if (menuInfo.Id == 0)
                {
                    await DataProvider.UserMenuRepository.InsertAsync(menuInfo);

                    await request.AddAdminLogAsync("新增用户菜单", $"用户菜单:{menuInfo.Text}");
                }
                else if (menuInfo.Id > 0)
                {
                    await DataProvider.UserMenuRepository.UpdateAsync(menuInfo);

                    await request.AddAdminLogAsync("修改用户菜单", $"用户菜单:{menuInfo.Text}");
                }

                return Ok(new
                {
                    Value = await DataProvider.UserMenuRepository.GetUserMenuListAsync()
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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsConfigHomeMenu))
                {
                    return Unauthorized();
                }

                foreach (var userMenuInfo in await DataProvider.UserMenuRepository.GetUserMenuListAsync())
                {
                    await DataProvider.UserMenuRepository.DeleteAsync(userMenuInfo.Id);
                }

                await request.AddAdminLogAsync("重置用户菜单");

                return Ok(new
                {
                    Value = await DataProvider.UserMenuRepository.GetUserMenuListAsync()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
