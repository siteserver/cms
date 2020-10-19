using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersLayerProfileController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] User request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            if (request.Id == 0)
            {
                var (user, errorMessage) = await _userRepository.InsertAsync(request, request.Password, string.Empty);
                if (user == null)
                {
                    return this.Error($"用户添加失败：{errorMessage}");
                }

                await _authManager.AddAdminLogAsync("添加用户", $"用户:{request.UserName}");
            }
            else
            {
                var (success, errorMessage) = await _userRepository.UpdateAsync(request);
                if (!success)
                {
                    return this.Error($"用户修改失败：{errorMessage}");
                }

                await _authManager.AddAdminLogAsync("修改用户", $"用户:{request.UserName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
