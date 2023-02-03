using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class SpellController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }
            
            var config = await _configRepository.GetAsync();
            config.IsCloudSpellingCheck = request.IsCloudSpellingCheck;
            config.IsCloudSpellingCheckAuto = request.IsCloudSpellingCheckAuto;
            config.IsCloudSpellingCheckIgnore = request.IsCloudSpellingCheckIgnore;
            config.IsCloudSpellingCheckWhiteList = request.IsCloudSpellingCheckWhiteList;

            await _configRepository.UpdateAsync(config);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
