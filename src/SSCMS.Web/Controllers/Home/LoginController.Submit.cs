using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
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

            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(user
            ); // 记录最后登录时间、失败次数清零

            await _statRepository.AddCountAsync(StatType.UserLogin);
            await _logRepository.AddUserLogAsync(user, ipAddress, Constants.ActionsLoginSuccess);
            var token = _authManager.AuthenticateUser(user, request.IsPersistent);

            var redirectToVerifyMobile = false;
            if (!user.MobileVerified)
            {
                var config = await _configRepository.GetAsync();
                var isSmsEnabled = await _smsManager.IsEnabledAsync();
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
