using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> InsertRole([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsRole))
            {
                return Unauthorized();
            }

            if (_roleRepository.IsPredefinedRole(request.RoleName))
            {
                return this.Error($"角色添加失败，{request.RoleName}为系统角色！");
            }
            if (await _roleRepository.IsRoleExistsAsync(request.RoleName))
            {
                return this.Error("角色名称已存在，请更换角色名称！");
            }

            await _roleRepository.InsertRoleAsync(new Role
            {
                RoleName = request.RoleName,
                CreatorUserName = _authManager.AdminName,
                Description = request.Description
            });

            if (request.AppPermissions != null && request.AppPermissions.Count > 0)
            {
                var permissionsInRolesInfo = new PermissionsInRoles
                {
                    Id = 0,
                    RoleName = request.RoleName,
                    AppPermissions = request.AppPermissions
                };
                await _permissionsInRolesRepository.InsertAsync(permissionsInRolesInfo);
            }

            if (request.SitePermissions != null && request.SitePermissions.Count > 0)
            {
                foreach (var sitePermissionsInfo in request.SitePermissions)
                {
                    sitePermissionsInfo.RoleName = request.RoleName;
                    await _sitePermissionsRepository.InsertAsync(sitePermissionsInfo);
                }
            }

            _cacheManager.Clear();

            await _authManager.AddAdminLogAsync("新增管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}