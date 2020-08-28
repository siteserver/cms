using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class DashboardController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var admin = await _authManager.GetAdminAsync();
            var lastActivityDate = admin.LastActivityDate ?? Constants.SqlMinValue;
            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Version = _settingsManager.Version,
                LastActivityDate = DateUtils.GetDateString(lastActivityDate, DateFormatType.Chinese),
                UpdateDate = DateUtils.GetDateString(config.UpdateDate, DateFormatType.Chinese),
                AdminWelcomeHtml = config.AdminWelcomeHtml,
                FrameworkDescription = _settingsManager.FrameworkDescription,
                OSArchitecture = _settingsManager.OSArchitecture,
                OSDescription = _settingsManager.OSDescription,
                Containerized = _settingsManager.Containerized,
                CPUCores = _settingsManager.CPUCores,
                UserName = admin.UserName,
                Level = await _authManager.GetAdminLevelAsync()
            };
        }
    }
}