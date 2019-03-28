using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/userConfig")]
    public class PagesUserConfigController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
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
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                ConfigManager.Instance.IsUserRegistrationAllowed = rest.GetPostBool("isUserRegistrationAllowed");
                ConfigManager.Instance.IsUserRegistrationChecked = rest.GetPostBool("isUserRegistrationChecked");
                ConfigManager.Instance.IsUserUnRegistrationAllowed = rest.GetPostBool("isUserUnRegistrationAllowed");
                ConfigManager.Instance.UserPasswordMinLength = rest.GetPostInt("userPasswordMinLength");
                ConfigManager.Instance.UserPasswordRestriction = rest.GetPostString("userPasswordRestriction");
                ConfigManager.Instance.UserRegistrationMinMinutes = rest.GetPostInt("userRegistrationMinMinutes");
                ConfigManager.Instance.IsUserLockLogin = rest.GetPostBool("isUserLockLogin");
                ConfigManager.Instance.UserLockLoginCount = rest.GetPostInt("userLockLoginCount");
                ConfigManager.Instance.UserLockLoginType = rest.GetPostString("userLockLoginType");
                ConfigManager.Instance.UserLockLoginHours = rest.GetPostInt("userLockLoginHours");

                DataProvider.Config.Update(ConfigManager.Instance);

                rest.AddAdminLog("修改用户设置");

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
