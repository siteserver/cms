using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var adminInfo = await _administratorRepository.GetByUserIdAsync(request.Id);
            await _administratorsInRolesRepository.RemoveUserAsync(adminInfo.UserName);
            await _administratorRepository.DeleteAsync(adminInfo.Id);

            await _authManager.AddAdminLogAsync("删除管理员", $"管理员:{adminInfo.UserName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
