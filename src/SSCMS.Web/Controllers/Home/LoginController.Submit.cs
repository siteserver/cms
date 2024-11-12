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
                var codeCacheKey = GetSmsCodeCacheKey(request.Mobile);
                var code = _cacheManager.Get<int>(codeCacheKey);
                if (code == 0 || TranslateUtils.ToInt(request.Code) != code)
                {
                    return this.Error("输入的验证码有误或验证码已超时");
                }

                user = await _userRepository.GetByMobileAsync(request.Mobile);

                if (user == null)
                {
                    return this.Error("此手机号码未关联用户");
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
                    var captcha = TranslateUtils.JsonDeserialize<CaptchaUtils.Captcha>(_settingsManager.Decrypt(request.Token));
                    if (captcha == null || string.IsNullOrEmpty(captcha.Value) || captcha.ExpireAt < DateTime.Now)
                    {
                        return this.Error("验证码已超时，请点击刷新验证码！");
                    }
                    if (!StringUtils.EqualsIgnoreCase(captcha.Value, request.Value) || CaptchaUtils.IsAlreadyUsed(captcha, _cacheManager))
                    {
                        return this.Error("验证码不正确，请重新输入！");
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
