using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class SpellController
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
                IsCloudSpellingCheck = config.IsCloudSpellingCheck,
                IsCloudSpellingCheckAuto = config.IsCloudSpellingCheckAuto,
                IsCloudSpellingCheckIgnore = config.IsCloudSpellingCheckIgnore,
                IsCloudSpellingCheckWhiteList = config.IsCloudSpellingCheckWhiteList,
            };
        }
    }
}
