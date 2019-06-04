using System;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/userConfig")]
    public class PagesUserConfigController : ControllerBase
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = GetRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance
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
                var request = GetRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                ConfigManager.Instance.IsUserRegistrationAllowed = request.GetPostBool("isUserRegistrationAllowed");
                ConfigManager.Instance.IsUserRegistrationChecked = request.GetPostBool("isUserRegistrationChecked");
                ConfigManager.Instance.IsUserUnRegistrationAllowed = request.GetPostBool("isUserUnRegistrationAllowed");
                ConfigManager.Instance.UserPasswordMinLength = request.GetPostInt("userPasswordMinLength");
                ConfigManager.Instance.UserPasswordRestriction = request.GetPostString("userPasswordRestriction");
                ConfigManager.Instance.UserRegistrationMinMinutes = request.GetPostInt("userRegistrationMinMinutes");
                ConfigManager.Instance.IsUserLockLogin = request.GetPostBool("isUserLockLogin");
                ConfigManager.Instance.UserLockLoginCount = request.GetPostInt("userLockLoginCount");
                ConfigManager.Instance.UserLockLoginType = request.GetPostString("userLockLoginType");
                ConfigManager.Instance.UserLockLoginHours = request.GetPostInt("userLockLoginHours");

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                request.AddAdminLog("修改用户设置");

                return Ok(new
                {
                    Value = ConfigManager.Instance
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
