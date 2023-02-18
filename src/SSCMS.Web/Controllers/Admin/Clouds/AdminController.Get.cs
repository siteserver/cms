using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    public partial class AdminController
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
                IsCloudAdmin = config.IsCloudAdmin,
                AdminTitle = config.AdminTitle,
                AdminFaviconUrl = config.AdminFaviconUrl,
                AdminLogoUrl = config.AdminLogoUrl,
                AdminWelcomeHtml = config.AdminWelcomeHtml
            };
        }
    }
}
