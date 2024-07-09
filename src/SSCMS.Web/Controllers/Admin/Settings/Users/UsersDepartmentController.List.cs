using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersDepartmentController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> List()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersDepartment))
            {
                return Unauthorized();
            }

            var cascade = await _departmentRepository.GetCascadesAsync(0, async department =>
            {
                var count = await _departmentRepository.GetAllCountAsync(department);

                return new
                {
                    Department = department,
                    Count = count,
                };
            });
            
            var config = await _configRepository.GetAsync();

            return new ListResult
            {
                Departments = cascade,
            };
        }
    }
}
