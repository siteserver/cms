using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsController
    {
        [HttpPost, Route(RouteUnLock)]
        public async Task<ActionResult<BoolResult>> UnLock([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var adminInfo = await _administratorRepository.GetByUserIdAsync(request.Id);

            await _administratorRepository.UnLockAsync(new List<string>
            {
                adminInfo.UserName
            });

            await _authManager.AddAdminLogAsync("解锁管理员", $"管理员:{adminInfo.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
