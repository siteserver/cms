using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteCheck)]
        public async Task<ActionResult<BoolResult>> Check([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            await _userRepository.CheckAsync(new List<int>
            {
                request.Id
            });

            await _authManager.AddAdminLogAsync("审核用户", $"用户Id:{request.Id}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
