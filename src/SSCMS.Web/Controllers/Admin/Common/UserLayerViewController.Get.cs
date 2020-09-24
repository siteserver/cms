using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Common
{
    public partial class UserLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            User user = null;
            if (request.UserId > 0)
            {
                user = await _userRepository.GetByUserIdAsync(request.UserId);
            }
            else if (!string.IsNullOrEmpty(request.UserName))
            {
                user = await _userRepository.GetByUserNameAsync(request.UserName);
            }

            if (user == null) return NotFound();

            var groupName = await _userGroupRepository.GetUserGroupNameAsync(user.GroupId);

            return new GetResult
            {
                User = user,
                GroupName = groupName
            };
        }
    }
}
