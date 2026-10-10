using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoginController
    {
        [HttpPost, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            Administrator administrator;
            var config = await _configRepository.GetAsync();

            // 强制登录必须携带首次登录成功时签发的一次性票据，不能再跳过验证码与口令校验（GJ20260924）
            var forceAdminId = 0;
            if (request.IsForceLogoutAndLogin)
            {
                forceAdminId = ConsumeForceLoginTicket(request.ForceLoginTicket);
                if (forceAdminId <= 0)
                {
                    return this.Error("登录状态已失效，请重新登录");
                }
            }

            if (forceAdminId > 0)
            {
                administrator = await _administratorRepository.GetByUserIdAsync(forceAdminId);
                if (administrator == null)
                {
                    return this.Error("登录状态已失效，请重新登录");
                }
                var (isUnlocked, lockMessage) = await _administratorRepository.ValidateLockAsync(administrator);
                if (!isUnlocked)
                {
                    return this.Error(lockMessage);
                }
            }
            else if (request.IsSmsLogin)
            {
                if (string.IsNullOrEmpty(request.Mobile)) return this.Error(SmsCodeGuard.ErrorInvalidCode);
                var codeKey = GetSmsCodeCacheKey(request.Mobile);
                var (isCodeValid, codeError) = SmsCodeGuard.Verify(_cacheManager, codeKey, request.Code, PageUtils.GetIpAddress(Request));
                if (!isCodeValid)
                {
                    return this.Error(codeError);
                }

                administrator = await _administratorRepository.GetByMobileAsync(request.Mobile);

                if (administrator == null)
                {
                    return this.Error(SmsCodeGuard.ErrorInvalidCode);
                }

                var (isUnlocked, lockMessage) = await _administratorRepository.ValidateLockAsync(administrator);
                if (!isUnlocked)
                {
                    return this.Error(lockMessage);
                }

                if (!administrator.MobileVerified)
                {
                    administrator.MobileVerified = true;
                    await _administratorRepository.UpdateAsync(administrator);
                }
            }
            else
            {
                if (!config.IsAdminCaptchaDisabled)
                {
                    if (!CaptchaUtils.Validate(_cacheManager, request.Token, request.Value))
                    {
                        return this.Error(CaptchaUtils.ErrorInvalid);
                    }
                }

                string userName;
                string errorMessage;
                (administrator, userName, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);

                if (administrator == null)
                {
                    administrator = await _administratorRepository.GetByUserNameAsync(userName);
                    if (administrator != null)
                    {
                        await _administratorRepository.UpdateLastActivityDateAndCountOfFailedLoginAsync(administrator); // 记录最后登录时间、失败次数+1
                    }

                    await _statRepository.AddCountAsync(StatType.AdminLoginFailure);
                    return this.Error(errorMessage);
                }

                administrator = await _administratorRepository.GetByUserNameAsync(userName);
            }

            var cacheKey = Constants.GetSessionIdCacheKey(administrator.Id);
            if (forceAdminId == 0)
            {
                var cacheState = await _dbCacheRepository.GetValueAndCreatedDateAsync(cacheKey);
                if (!string.IsNullOrEmpty(cacheState.Item1) && cacheState.Item2 > DateTime.Now.AddMinutes(-30))
                {
                    // 已完成口令/验证码校验，签发一次性强制登录票据，由用户确认后携带票据再次提交
                    return new SubmitResult
                    {
                        IsLoginExists = true,
                        ForceLoginTicket = IssueForceLoginTicket(administrator.Id)
                    };
                }
            }

            await _administratorRepository.UpdateLastActivityDateAndCountOfLoginAsync(administrator); // 记录最后登录时间、失败次数清零

            var token = _authManager.AuthenticateAdministrator(administrator, request.IsPersistent);

            await _statRepository.AddCountAsync(StatType.AdminLoginSuccess);
            await _logRepository.AddAdminLogAsync(administrator, PageUtils.GetIpAddress(Request), Constants.ActionsLoginSuccess);

            var isEnforcePasswordChange = false;
            var sessionId = StringUtils.Guid();
            await _dbCacheRepository.RemoveAndInsertAsync(cacheKey, sessionId);

            if (config.IsAdminEnforcePasswordChange)
            {
                if (administrator.LastChangePasswordDate == null)
                {
                    isEnforcePasswordChange = true;
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - administrator.LastChangePasswordDate.Value.Ticks);
                    if (ts.TotalDays > config.AdminEnforcePasswordChangeDays)
                    {
                        isEnforcePasswordChange = true;
                    }
                }
            }

            return new SubmitResult
            {
                IsLoginExists = false,
                Administrator = administrator,
                SessionId = sessionId,
                IsEnforcePasswordChange = isEnforcePasswordChange,
                Token = token
            };
        }
    }
}
