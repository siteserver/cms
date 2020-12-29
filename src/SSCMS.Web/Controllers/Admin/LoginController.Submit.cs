using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
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
            if (request.IsSmsLogin)
            {
                var codeCacheKey = GetSmsCodeCacheKey(request.Mobile);
                var code = _cacheManager.Get<int>(codeCacheKey);
                if (code == 0 || TranslateUtils.ToInt(request.Code) != code)
                {
                    return this.Error("输入的验证码有误或验证码已超时，请重试");
                }

                administrator = await _administratorRepository.GetByMobileAsync(request.Mobile);

                if (administrator == null)
                {
                    return this.Error("此手机号码未关联管理员，请更换手机号码");
                }

                if (!administrator.MobileVerified)
                {
                    administrator.MobileVerified = true;
                    await _administratorRepository.UpdateAsync(administrator);
                }
            }
            else
            {
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

            await _administratorRepository.UpdateLastActivityDateAndCountOfLoginAsync(administrator); // 记录最后登录时间、失败次数清零

            var token = _authManager.AuthenticateAdministrator(administrator, request.IsPersistent);

            await _statRepository.AddCountAsync(StatType.AdminLoginSuccess);
            await _logRepository.AddAdminLogAsync(administrator, PageUtils.GetIpAddress(Request), Constants.ActionsLoginSuccess);

            var cacheKey = Constants.GetSessionIdCacheKey(administrator.Id);
            var isEnforcePasswordChange = false;
            var sessionId = StringUtils.Guid();
            await _dbCacheRepository.RemoveAndInsertAsync(cacheKey, sessionId);

            var config = await _configRepository.GetAsync();

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
                Administrator = administrator,
                SessionId = sessionId,
                IsEnforcePasswordChange = isEnforcePasswordChange,
                Token = token
            };
        }
    }
}
