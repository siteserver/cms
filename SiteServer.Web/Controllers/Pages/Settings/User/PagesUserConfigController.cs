using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/userConfig")]
    public class PagesUserConfigController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
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
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var config = await ConfigManager.GetInstanceAsync();

                config.IsUserRegistrationAllowed = request.GetPostBool("isUserRegistrationAllowed");
                config.IsUserRegistrationChecked = request.GetPostBool("isUserRegistrationChecked");
                config.IsUserUnRegistrationAllowed = request.GetPostBool("isUserUnRegistrationAllowed");
                config.UserPasswordMinLength = request.GetPostInt("userPasswordMinLength");
                config.UserPasswordRestriction = request.GetPostString("userPasswordRestriction");
                config.UserRegistrationMinMinutes = request.GetPostInt("userRegistrationMinMinutes");
                config.IsUserLockLogin = request.GetPostBool("isUserLockLogin");
                config.UserLockLoginCount = request.GetPostInt("userLockLoginCount");
                config.UserLockLoginType = request.GetPostString("userLockLoginType");
                config.UserLockLoginHours = request.GetPostInt("userLockLoginHours");

                await DataProvider.ConfigDao.UpdateAsync(config);

                await request.AddAdminLogAsync("修改用户设置");

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
