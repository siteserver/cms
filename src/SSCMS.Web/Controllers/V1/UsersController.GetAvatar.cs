using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpGet, Route(RouteUserAvatar)]
        public async Task<StringResult> GetAvatar(int id)
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
