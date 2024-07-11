using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Common
{
    public partial class UserLayerViewController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsLogsUser) &&
                !await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }
            
            User user = null;
            if (!string.IsNullOrEmpty(request.Guid))
            {
                user = await _userRepository.GetByGuidAsync(request.Guid);
            }

            if (user == null) return this.Error(Constants.ErrorNotFound);

            user.Remove("confirmPassword");

            // var groupName = await _userGroupRepository.GetUserGroupNameAsync(user.GroupId);

            // return new GetResult
            // {
            //     User = user,
            //     GroupName = groupName
            // };

            var groups = await _usersInGroupsRepository.GetGroupsAsync(user);
            var departmentFullName = await _departmentRepository.GetFullNameAsync(user.DepartmentId);

            return new GetResult
            {
                User = user,
                Groups = groups,
                DepartmentFullName = departmentFullName
            };
        }
    }
}
