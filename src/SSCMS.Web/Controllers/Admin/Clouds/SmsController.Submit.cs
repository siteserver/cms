using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class SmsController
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
            config.IsCloudSmsAdmin = request.IsCloudSmsAdmin;
            config.IsCloudSmsAdminAndDisableAccount = request.IsCloudSmsAdminAndDisableAccount;
            config.IsCloudSmsUser = request.IsCloudSmsUser;

            await _configRepository.UpdateAsync(config);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
