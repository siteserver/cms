using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class MailController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }
            
            var config = await _configRepository.GetAsync();
            config.IsCloudMail = request.IsCloudMail;
            config.CloudMailFromAlias = request.CloudMailFromAlias;
            config.IsCloudMailContentAdd = request.IsCloudMailContentAdd;
            config.IsCloudMailContentEdit = request.IsCloudMailContentEdit;
            config.CloudMailAddress = request.CloudMailAddress;

            await _configRepository.UpdateAsync(config);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
