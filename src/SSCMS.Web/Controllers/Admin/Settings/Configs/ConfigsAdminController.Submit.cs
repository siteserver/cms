using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsAdminController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsSuperAdminAsync())
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.AdminTitle = request.AdminTitle;
            config.AdminLogoUrl = request.AdminLogoUrl;
            config.AdminWelcomeHtml = request.AdminWelcomeHtml;

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改管理后台设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
