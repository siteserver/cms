using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoginController
    {
        [HttpPost, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest request)
        {
            var (administrator, userName, errorMessage) = await _administratorRepository.ValidateAsync(request.Account, request.Password, true);

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
            await _administratorRepository.UpdateLastActivityDateAndCountOfLoginAsync(administrator); // 记录最后登录时间、失败次数清零

            var token = _authManager.AuthenticateAdministrator(administrator, request.IsPersistent);

            await _statRepository.AddCountAsync(StatType.AdminLoginSuccess);
            await _logRepository.AddAdminLogAsync(administrator, Constants.ActionsLoginSuccess);

            var sessionId = StringUtils.Guid();
            var cacheKey = Constants.GetSessionIdCacheKey(administrator.Id);
            await _dbCacheRepository.RemoveAndInsertAsync(cacheKey, sessionId);

            var config = await _configRepository.GetAsync();

            var isEnforcePasswordChange = false;
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

            return new LoginResult
            {
                Administrator = administrator,
                SessionId = sessionId,
                IsEnforcePasswordChange = isEnforcePasswordChange,
                Token = token
            };
        }
    }
}
