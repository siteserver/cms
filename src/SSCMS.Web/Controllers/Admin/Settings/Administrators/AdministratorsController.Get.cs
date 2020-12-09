using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministrators))
            {
                return Unauthorized();
            }

            var roles = new List<KeyValuePair<string, string>>();

            var adminId = _authManager.AdminId;
            var adminName = _authManager.AdminName;
            var roleNameList = await _authManager.IsSuperAdminAsync() ? await _roleRepository.GetRoleNamesAsync() : await _roleRepository.GetRoleNamesByCreatorUserNameAsync(adminName);

            var predefinedRoles = ListUtils.GetEnums<PredefinedRole>();
            foreach (var predefinedRole in predefinedRoles)
            {
                roles.Add(new KeyValuePair<string, string>(predefinedRole.GetValue(), predefinedRole.GetDisplayName()));
            }
            foreach (var roleName in roleNameList)
            {
                if (!roles.Any(x => StringUtils.EqualsIgnoreCase(x.Key, roleName)))
                {
                    roles.Add(new KeyValuePair<string, string>(roleName, roleName));
                }
            }

            var isSuperAdmin = await _authManager.IsSuperAdminAsync();
            var creatorUserName = isSuperAdmin ? string.Empty : adminName;
            var count = await _administratorRepository.GetCountAsync(creatorUserName, request.Role, request.LastActivityDate, request.Keyword);
            var administrators = await _administratorRepository.GetAdministratorsAsync(creatorUserName, request.Role, request.Order, request.LastActivityDate, request.Keyword, request.Offset, request.Limit);
            var admins = new List<Admin>();
            foreach (var administratorInfo in administrators)
            {
                admins.Add(new Admin
                {
                    Id = administratorInfo.Id,
                    AvatarUrl = administratorInfo.AvatarUrl,
                    UserName = administratorInfo.UserName,
                    DisplayName = string.IsNullOrEmpty(administratorInfo.DisplayName)
                        ? administratorInfo.UserName
                        : administratorInfo.DisplayName,
                    Mobile = administratorInfo.Mobile,
                    LastActivityDate = administratorInfo.LastActivityDate,
                    CountOfLogin = administratorInfo.CountOfLogin,
                    Locked = administratorInfo.Locked,
                    Roles = await _administratorRepository.GetRolesAsync(administratorInfo.UserName)
                });
            }

            return new GetResult
            {
                Administrators = admins,
                Count = count,
                Roles = roles,
                IsSuperAdmin = await _authManager.IsSuperAdminAsync(),
                AdminId = adminId
            };
        }
    }
}
