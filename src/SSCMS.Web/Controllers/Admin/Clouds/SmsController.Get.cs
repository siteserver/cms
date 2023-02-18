using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class SmsController
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
                IsCloudSms = config.IsCloudSms,
                IsCloudSmsAdmin = config.IsCloudSmsAdmin,
                IsCloudSmsAdminAndDisableAccount = config.IsCloudSmsAdminAndDisableAccount,
                IsCloudSmsUser = config.IsCloudSmsUser,
            };
        }
    }
}
