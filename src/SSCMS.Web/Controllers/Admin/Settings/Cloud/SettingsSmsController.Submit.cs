using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Cloud
{
    public partial class SettingsSmsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }
            
            var config = await _configRepository.GetAsync();
            config.IsCloudSms = request.IsCloudSms;
            config.IsCloudSmsAdministrator = request.IsCloudSmsAdministrator;
            config.IsCloudSmsUser = request.IsCloudSmsUser;

            await _configRepository.UpdateAsync(config);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
