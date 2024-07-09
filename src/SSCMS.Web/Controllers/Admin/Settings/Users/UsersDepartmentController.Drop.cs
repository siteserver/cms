using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersDepartmentController
    {
        [HttpPost, Route(RouteDrop)]
        public async Task<ActionResult<BoolResult>> Drop([FromBody] DropRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersDepartment))
            {
                return Unauthorized();
            }

            await _departmentRepository.DropAsync(request.SourceId, request.TargetId, request.DropType);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
