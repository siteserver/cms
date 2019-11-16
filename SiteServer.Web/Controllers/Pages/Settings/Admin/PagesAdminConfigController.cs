using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings.Admin
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/adminConfig")]
    public class PagesAdminConfigController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var config = await ConfigManager.GetInstanceAsync();

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
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var config = await ConfigManager.GetInstanceAsync();

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

                await DataProvider.ConfigDao.UpdateAsync(config);

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
