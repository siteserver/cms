using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [OpenApiOperation("获取用户头像 API", "获取用户头像地址，使用GET发起请求，请求地址为/api/v1/users/{id}/avatar，此接口可以直接访问，无需身份验证")]
        [HttpGet, Route(RouteUserAvatar)]
        public async Task<StringResult> GetAvatar([FromRoute] int id)
        {
            var user = await _userRepository.GetByUserIdAsync(id);

            var avatarUrl = !string.IsNullOrEmpty(user?.AvatarUrl) ? user.AvatarUrl : _pathManager.DefaultAvatarUrl;
            avatarUrl = PageUtils.AddProtocolToUrl(avatarUrl);

            return new StringResult
            {
                Value = avatarUrl
            };
        }
    }
}
