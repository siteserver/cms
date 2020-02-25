using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Settings.Users
{
    [Route("admin/settings/usersView")]
    public partial class UsersViewController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public UsersViewController(IAuthManager authManager)
        {
            _authManager = authManager;
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
                user = await DataProvider.UserRepository.GetByUserIdAsync(request.UserId);
            }
            else if (!string.IsNullOrEmpty(request.UserName))
            {
                user = await DataProvider.UserRepository.GetByUserNameAsync(request.UserName);
            }

            if (user == null)
            {
                return NotFound();
            }

            var groupName = await DataProvider.UserGroupRepository.GetUserGroupNameAsync(user.GroupId);

            return new GetResult
            {
                User = user,
                GroupName = groupName
            };
        }
    }
}
