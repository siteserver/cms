using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("获取用户 API", "获取用户，使用GET发起请求，请求地址为/api/v1/users/{account}")]
        [HttpGet, Route(RouteUser)]
        public async Task<ActionResult<User>> Get([FromRoute] string account)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(account))
            {
                return null;
            }

            User user = null;

            if (StringUtils.IsMobile(account))
            {
                user = await _userRepository.GetByMobileAsync(account);
            }
            else if (StringUtils.IsEmail(account))
            {
                user = await _userRepository.GetByEmailAsync(account);
            }
            else if (StringUtils.IsNumber(account))
            {
                var userId = TranslateUtils.ToInt(account);
                if (userId > 0)
                {
                    user = await _userRepository.GetByUserIdAsync(userId);
                }
            }
            else
            {
                if (await _userRepository.IsUserNameExistsAsync(account))
                {
                    user = await _userRepository.GetByUserNameAsync(account);
                }
                else if (await _userRepository.IsOpenIdExistsAsync(account))
                {
                    user = await _userRepository.GetByOpenIdAsync(account);
                }
            }

            return user;
        }
    }
}
