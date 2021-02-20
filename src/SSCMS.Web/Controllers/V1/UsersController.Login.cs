using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("用户登录 API", "用户登录，使用POST发起请求，请求地址为/api/v1/users/actions/login，此接口可以直接访问，无需身份验证")]
        [HttpPost, Route(RouteActionsLogin)]
        public async Task<ActionResult<LoginResult>> Login([FromBody]LoginRequest request)
        {
            var (user, _, errorMessage) = await _userRepository.ValidateAsync(request.Account, request.Password, true);
            if (user == null)
            {
                return this.Error(errorMessage);
            }

            var accessToken = _authManager.AuthenticateUser(user, request.IsPersistent);

            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(user);

            await _statRepository.AddCountAsync(StatType.UserLogin);
            await _logRepository.AddUserLogAsync(user, PageUtils.GetIpAddress(Request), Constants.ActionsLoginSuccess);

            return new LoginResult
            {
                User = user,
                AccessToken = accessToken
            };
        }
    }
}
