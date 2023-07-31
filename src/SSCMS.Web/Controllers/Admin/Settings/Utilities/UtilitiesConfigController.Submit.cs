using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesConfigController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUtilitiesConfig))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.IsMaterialSiteOnly = request.IsMaterialSiteOnly;

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改系统设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
