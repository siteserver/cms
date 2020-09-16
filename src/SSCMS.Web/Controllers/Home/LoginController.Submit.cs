using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class LoginController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            var (user, userName, errorMessage) = await _userRepository.ValidateAsync(request.Account, request.Password, true);

            if (user == null)
            {
                user = await _userRepository.GetByUserNameAsync(userName);
                if (user != null)
                {
                    await _logRepository.AddUserLogAsync(user, Constants.ActionsLoginFailure, "帐号或密码错误");
                }
                return this.Error(errorMessage);
            }

            user = await _userRepository.GetByUserNameAsync(userName);
            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(user
            ); // 记录最后登录时间、失败次数清零

            await _statRepository.AddCountAsync(StatType.UserLogin);
            await _logRepository.AddUserLogAsync(user, Constants.ActionsLoginSuccess);
            var token = _authManager.AuthenticateUser(user, request.IsPersistent);

            return new SubmitResult
            {
                User = user,
                Token = token
            };
        }
    }
}
