using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    
    [RoutePrefix("pages/settings/adminConfig")]
    public class PagesAdminConfigController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var config = await DataProvider.ConfigRepository.GetAsync();

                return Ok(new
                {
                    Value = config
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var config = await DataProvider.ConfigRepository.GetAsync();

                config.AdminUserNameMinLength =
                    request.GetPostInt("adminUserNameMinLength");
                config.AdminPasswordMinLength =
                    request.GetPostInt("adminPasswordMinLength");
                config.AdminPasswordRestriction =
                    request.GetPostString("adminPasswordRestriction");

                config.IsAdminLockLogin = request.GetPostBool("isAdminLockLogin");
                config.AdminLockLoginCount = request.GetPostInt("adminLockLoginCount");
                config.AdminLockLoginType = request.GetPostString("adminLockLoginType");
                config.AdminLockLoginHours = request.GetPostInt("adminLockLoginHours");

                config.IsViewContentOnlySelf = request.GetPostBool("isViewContentOnlySelf");

                config.IsAdminEnforcePasswordChange = request.GetPostBool("isAdminEnforcePasswordChange");
                config.AdminEnforcePasswordChangeDays = request.GetPostInt("adminEnforcePasswordChangeDays");

                config.IsAdminEnforceLogout = request.GetPostBool("isAdminEnforceLogout");
                config.AdminEnforceLogoutMinutes = request.GetPostInt("adminEnforceLogoutMinutes");

                await DataProvider.ConfigRepository.UpdateAsync(config);

                await request.AddAdminLogAsync("修改管理员设置");

                return Ok(new
                {
                    Value = config
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
