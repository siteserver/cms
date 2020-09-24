using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var user = await _userRepository.DeleteAsync(request.Id);

            await _authManager.AddAdminLogAsync("删除用户", $"用户:{user.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
