using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin.Settings.AdminConfig
{
    public partial class AdminConfigController
    {
        [HttpGet, Route(Route), Route(LegacyRoute)]
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
                AdminLogoLinkUrl = config.AdminLogoLinkUrl,
                AdminWelcomeHtml = config.AdminWelcomeHtml,
                IsAdminUpdateDisabled = config.IsAdminUpdateDisabled,
                IsAdminTicketsDisabled = config.IsAdminTicketsDisabled,
                IsAdminWatermark = config.IsAdminWatermark
            };
        }
    }
}
