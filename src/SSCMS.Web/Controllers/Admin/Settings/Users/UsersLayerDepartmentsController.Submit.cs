using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerDepartmentsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var userIds = ListUtils.GetIntList(request.UserIds);
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByUserIdAsync(userId);
                if (user == null || user.DepartmentId == request.TransDepartmentId) continue;

                await _userRepository.UpdateDepartmentIdAsync(user, request.TransDepartmentId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}