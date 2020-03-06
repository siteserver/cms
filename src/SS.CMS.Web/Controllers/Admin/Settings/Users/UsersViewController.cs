using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    [Route("admin/settings/usersView")]
    public partial class UsersViewController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UsersViewController(IAuthManager authManager, IUserRepository userRepository, IUserGroupRepository userGroupRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin)
            {
                return Unauthorized();
            }

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
