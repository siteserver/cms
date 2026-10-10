using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class LoginController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            User user;
            var config = await _configRepository.GetAsync();
            var ipAddress = PageUtils.GetIpAddress(Request);
            if (request.IsSmsLogin)
            {
                if (string.IsNullOrEmpty(request.Mobile)) return this.Error(SmsCodeGuard.ErrorInvalidCode);
                var codeKey = GetSmsCodeCacheKey(request.Mobile);
                var (isCodeValid, codeError) = SmsCodeGuard.Verify(_cacheManager, codeKey, request.Code, ipAddress);
                if (!isCodeValid)
                {
                    return this.Error(codeError);
                }

                user = await _userRepository.GetByMobileAsync(request.Mobile);

                if (user == null)
                {
                    return this.Error(SmsCodeGuard.ErrorInvalidCode);
                }

                var (isStateValid, stateMessage) = await _userRepository.ValidateStateAsync(user);
                if (!isStateValid)
                {
                    return this.Error(stateMessage);
                }

                if (!user.MobileVerified)
                {
                    user.MobileVerified = true;
                    await _userRepository.UpdateAsync(user);
                }
            }
            else
            {
                if (!config.IsUserCaptchaDisabled)
                {
                    if (!CaptchaUtils.Validate(_cacheManager, request.Token, request.Value))
                    {
                        return this.Error(CaptchaUtils.ErrorInvalid);
                    }
                }
                
                string userName;
                string errorMessage;
                (user, userName, errorMessage) = await _userRepository.ValidateAsync(request.Account, request.Password, true);

                if (user == null)
                {
                    user = await _userRepository.GetByUserNameAsync(userName);
                    if (user != null)
                    {
                        await _logRepository.AddUserLogAsync(user, ipAddress, Constants.ActionsLoginFailure, "帐号或密码错误");
                    }
                    return this.Error(errorMessage);
                }

                user = await _userRepository.GetByUserNameAsync(userName);
            }

            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(user); // 记录最后登录时间、失败次数清零

            await _statRepository.AddCountAsync(StatType.UserLogin);
            await _logRepository.AddUserLogAsync(user, ipAddress, Constants.ActionsLoginSuccess);
            var token = _authManager.AuthenticateUser(user, request.IsPersistent);

            var redirectToVerifyMobile = false;
            if (!user.MobileVerified)
            {
                var smsSettings = await _smsManager.GetSmsSettingsAsync();
                var isSmsEnabled = smsSettings.IsSms && smsSettings.IsSmsUser;
                if (isSmsEnabled && config.IsUserForceVerifyMobile)
                {
                    redirectToVerifyMobile = true;
                }
            }

            return new SubmitResult
            {
                RedirectToVerifyMobile = redirectToVerifyMobile,
                User = user,
                Token = token
            };
        }
    }
}
