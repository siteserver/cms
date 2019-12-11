using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/adminConfig")]
    public class PagesAdminConfigController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminConfig))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance.SystemConfigInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsAdminConfig))
                {
                    return Unauthorized();
                }

                ConfigManager.SystemConfigInfo.AdminUserNameMinLength =
                    request.GetPostInt("adminUserNameMinLength");
                ConfigManager.SystemConfigInfo.AdminPasswordMinLength =
                    request.GetPostInt("adminPasswordMinLength");
                ConfigManager.SystemConfigInfo.AdminPasswordRestriction =
                    request.GetPostString("adminPasswordRestriction");

                ConfigManager.SystemConfigInfo.IsAdminLockLogin = request.GetPostBool("isAdminLockLogin");
                ConfigManager.SystemConfigInfo.AdminLockLoginCount = request.GetPostInt("adminLockLoginCount");
                ConfigManager.SystemConfigInfo.AdminLockLoginType = request.GetPostString("adminLockLoginType");
                ConfigManager.SystemConfigInfo.AdminLockLoginHours = request.GetPostInt("adminLockLoginHours");

                ConfigManager.SystemConfigInfo.IsViewContentOnlySelf = request.GetPostBool("isViewContentOnlySelf");

                ConfigManager.SystemConfigInfo.IsAdminEnforcePasswordChange = request.GetPostBool("isAdminEnforcePasswordChange");
                ConfigManager.SystemConfigInfo.AdminEnforcePasswordChangeDays = request.GetPostInt("adminEnforcePasswordChangeDays");

                ConfigManager.SystemConfigInfo.IsAdminEnforceLogout = request.GetPostBool("isAdminEnforceLogout");
                ConfigManager.SystemConfigInfo.AdminEnforceLogoutMinutes = request.GetPostInt("adminEnforceLogoutMinutes");

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                request.AddAdminLog("修改管理员设置");

                return Ok(new
                {
                    Value = ConfigManager.SystemConfigInfo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
