using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class SettingsAdminController
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
                AdminTitle = config.AdminTitle,
                AdminLogoUrl = config.AdminLogoUrl,
                AdminWelcomeHtml = config.AdminWelcomeHtml
            };
        }
    }
}
