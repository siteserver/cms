using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("新增用户 API", "注册新用户，使用POST发起请求，请求地址为/api/v1/users")]
        [HttpPost, Route(Route)]
        public async Task<ActionResult<User>> Create([FromBody]User request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeUsers))
            {
                return Unauthorized();
            }
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var config = await _configRepository.GetAsync();

            if (!config.IsUserRegistrationGroup)
            {
                request.GroupId = 0;
            }
            var password = request.Password;

            if (!config.IsUserRegistrationAllowed)
            {
                return this.Error("对不起，系统已禁止新用户注册！");
            }

            var (user, errorMessage) = await _userRepository.InsertAsync(request, password, config.IsUserRegistrationChecked, PageUtils.GetIpAddress(Request));
            if (user == null)
            {
                return this.Error(errorMessage);
            }

            await _statRepository.AddCountAsync(StatType.UserRegister);

            return user;
        }
    }
}
