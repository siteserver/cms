using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Extensions;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
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
            await _logRepository.AddUserLogAsync(user, Constants.ActionsLoginSuccess, string.Empty);

            return new LoginResult
            {
                User = user,
                AccessToken = accessToken
            };
        }
    }
}
