using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class CensorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }
            
            var config = await _configRepository.GetAsync();
            config.IsCloudCensorText = request.IsCloudCensorText;
            config.IsCloudCensorTextAuto = request.IsCloudCensorTextAuto;
            config.IsCloudCensorTextIgnore = request.IsCloudCensorTextIgnore;
            config.IsCloudCensorTextWhiteList = request.IsCloudCensorTextWhiteList;

            await _configRepository.UpdateAsync(config);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
