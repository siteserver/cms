using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
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
            var config = await _configRepository.GetAsync();

            if (!config.IsUserRegistrationGroup)
            {
                request.GroupId = 0;
            }
            var password = request.Password;

            var (user, errorMessage) = await _userRepository.InsertAsync(request, password, string.Empty);
            if (user == null)
            {
                return this.Error(errorMessage);
            }

            await _statRepository.AddCountAsync(StatType.UserRegister);

            return user;
        }
    }
}
