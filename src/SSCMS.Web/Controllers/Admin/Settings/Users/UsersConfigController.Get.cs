using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersConfigController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();
            var smsSettings = await _smsManager.GetSmsSettingsAsync();

            return new GetResult
            {
                Config = config,
                IsSmsEnabled = smsSettings.IsSms && smsSettings.IsSmsUser
            };
        }
    }
}
