using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersConfigController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsersConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();
            var isSmsEnabled = await _smsManager.IsEnabledAsync();

            return new GetResult
            {
                Config = config,
                IsSmsEnabled = isSmsEnabled
            };
        }
    }
}
