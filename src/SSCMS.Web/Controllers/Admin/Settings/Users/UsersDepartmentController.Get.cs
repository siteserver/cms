using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersDepartmentController
    {
        [HttpGet, Route(RouteGet)]
        public async Task<ActionResult<GetResult>> Get(int departmentId)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersDepartment))
            {
                return Unauthorized();
            }

            var department = await _departmentRepository.GetAsync(departmentId);

            return new GetResult
            {
                Department = department
            };
        }
    }
}