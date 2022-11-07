using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Settings.Cloud
{
    public partial class SettingsCensorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                IsCloudCensorText = config.IsCloudCensorText,
                IsCloudCensorTextAuto = config.IsCloudCensorTextAuto,
                IsCloudCensorTextIgnore = config.IsCloudCensorTextIgnore,
                IsCloudCensorTextWhiteList = config.IsCloudCensorTextWhiteList,
            };
        }
    }
}
