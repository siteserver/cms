using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    public partial class ConfigsHomeController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsConfigsHome))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            config.IsHomeClosed = request.IsHomeClosed;
            config.HomeTitle = request.HomeTitle;
            config.IsHomeLogo = request.IsHomeLogo;
            config.HomeLogoUrl = request.HomeLogoUrl;
            config.HomeDefaultAvatarUrl = request.HomeDefaultAvatarUrl;
            config.UserRegistrationAttributes = request.UserRegistrationAttributes;
            config.IsUserRegistrationGroup = request.IsUserRegistrationGroup;
            config.IsHomeAgreement = request.IsHomeAgreement;
            config.HomeAgreementHtml = request.HomeAgreementHtml;
            config.HomeWelcomeHtml = request.HomeWelcomeHtml;

            await _configRepository.UpdateAsync(config);

            await _authManager.AddAdminLogAsync("修改用户中心设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
