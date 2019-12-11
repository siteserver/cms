using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/userConfig")]
    public class PagesUserConfigController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUserConfig))
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
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.AppPermissions.SettingsUserConfig))
                {
                    return Unauthorized();
                }

                ConfigManager.SystemConfigInfo.IsUserRegistrationAllowed = request.GetPostBool("isUserRegistrationAllowed");
                ConfigManager.SystemConfigInfo.IsUserRegistrationChecked = request.GetPostBool("isUserRegistrationChecked");
                ConfigManager.SystemConfigInfo.IsUserUnRegistrationAllowed = request.GetPostBool("isUserUnRegistrationAllowed");
                ConfigManager.SystemConfigInfo.UserPasswordMinLength = request.GetPostInt("userPasswordMinLength");
                ConfigManager.SystemConfigInfo.UserPasswordRestriction = request.GetPostString("userPasswordRestriction");
                ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes = request.GetPostInt("userRegistrationMinMinutes");
                ConfigManager.SystemConfigInfo.IsUserLockLogin = request.GetPostBool("isUserLockLogin");
                ConfigManager.SystemConfigInfo.UserLockLoginCount = request.GetPostInt("userLockLoginCount");
                ConfigManager.SystemConfigInfo.UserLockLoginType = request.GetPostString("userLockLoginType");
                ConfigManager.SystemConfigInfo.UserLockLoginHours = request.GetPostInt("userLockLoginHours");

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                request.AddAdminLog("修改用户设置");

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
