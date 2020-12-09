using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Home
{
    public partial class HomeConfigController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsHomeConfig))
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
            config.IsUserRegistrationMobile = request.IsUserRegistrationMobile;
            config.IsUserRegistrationEmail = request.IsUserRegistrationEmail;
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
