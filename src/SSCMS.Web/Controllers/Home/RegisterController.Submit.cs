using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class RegisterController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] User request)
        {
            var config = await _configRepository.GetAsync();
            var isUserVerifyMobile = false;
            var smsSettings = await _smsManager.GetSmsSettingsAsync();
            if (smsSettings.IsSms && smsSettings.IsSmsUser && config.IsUserForceVerifyMobile)
            {
                isUserVerifyMobile = true;
            }

            if (!config.IsUserRegistrationAllowed)
            {
                return this.Error("对不起，系统已禁止新用户注册！");
            }

            // 只有通过 verifyMobile 接口校验过短信验证码的手机号才标记为已验证
            request.MobileVerified = !string.IsNullOrEmpty(request.Mobile) &&
                                     SmsCodeGuard.ConsumeVerified(_cacheManager, GetSmsCodeCacheKey(request.Mobile));
            if (isUserVerifyMobile && !request.MobileVerified)
            {
                return this.Error("请先完成手机号码验证");
            }

            var (user, errorMessage) = await _userRepository.InsertAsync(request, request.Password, config.IsUserRegistrationChecked, PageUtils.GetIpAddress(Request));
            if (user == null)
            {
                return this.Error($"用户注册失败：{errorMessage}");
            }

            await _statRepository.AddCountAsync(StatType.UserRegister);

            return new BoolResult
            {
                Value = user.Checked
            };
        }
    }
}
