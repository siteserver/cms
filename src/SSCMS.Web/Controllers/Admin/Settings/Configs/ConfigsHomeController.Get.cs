using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsConfigsHome))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Config = config,
                Styles = await _tableStyleRepository.GetUserStylesAsync()
            };
        }
    }
}
