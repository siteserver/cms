using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteTree)]
        public async Task<ActionResult<TreeResult>> Tree()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var cascade = await _departmentRepository.GetCascadesAsync(0, async department =>
            {
                var count = await _departmentRepository.GetAllCountAsync(department);

                return new
                {
                    Node = department,
                    Count = count,
                    Disabled = false
                };
            });

            var count = await _userRepository.GetCountAsync();

            return new TreeResult
            {
                Departments = cascade,
                Count = count,
            };
        }
    }
}
