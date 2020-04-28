using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersViewController : ControllerBase
    {
        private const string Route = "settings/usersView";

        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UsersViewController(IUserRepository userRepository, IUserGroupRepository userGroupRepository)
        {
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
        }

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

            if (user == null)
            {
                return NotFound();
            }

            var groupName = await _userGroupRepository.GetUserGroupNameAsync(user.GroupId);

            return new GetResult
            {
                User = user,
                GroupName = groupName
            };
        }
    }
}
