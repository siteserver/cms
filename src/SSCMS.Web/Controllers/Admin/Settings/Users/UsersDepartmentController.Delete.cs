using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersDepartmentController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<List<int>>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersDepartment))
            {
                return Unauthorized();
            }

            var department = await _departmentRepository.GetAsync(request.DepartmentId);
            if (department == null) return this.Error("无法确定父部门");

            var departmentIdList = await _departmentRepository.GetDepartmentIdsAsync(request.DepartmentId, ScopeType.All);

            foreach (var departmentId in departmentIdList)
            {
                await _departmentRepository.DeleteAsync(departmentId);
            }

            await _authManager.AddAdminLogAsync("删除部门", $"部门：{department.Name}");

            return new List<int>
            {
                department.ParentId
            };
        }
    }
}
