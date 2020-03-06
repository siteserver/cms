using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SS.CMS;
using SS.CMS.Core;

namespace SS.CMS.Repositories
{
    public partial class AdministratorRepository
    {
        public async Task AddUserToRolesAsync(string userName, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                await AddUserToRoleAsync(userName, roleName);
            }
        }

        public async Task AddUserToRoleAsync(string userName, string roleName)
        {
            if (!await IsUserNameExistsAsync(userName)) return;
            await _administratorsInRolesRepository.InsertAsync(userName, roleName);
        }

        public async Task<string> GetRolesAsync(string userName)
        {
            var isConsoleAdministrator = false;
            var isSystemAdministrator = false;
            var roleNameList = new List<string>();
            var roles = await _administratorsInRolesRepository.GetRolesForUserAsync(userName);
            foreach (var role in roles)
            {
                if (!_roleRepository.IsPredefinedRole(role))
                {
                    roleNameList.Add(role);
                }
                else
                {
                    if (_roleRepository.IsConsoleAdministrator(role))
                    {
                        isConsoleAdministrator = true;
                        break;
                    }
                    if (_roleRepository.IsSystemAdministrator(role))
                    {
                        isSystemAdministrator = true;
                        break;
                    }
                }
            }

            var roleNames = string.Empty;

            if (isConsoleAdministrator)
            {
                roleNames += PredefinedRole.ConsoleAdministrator.GetDisplayName();
            }
            else if (isSystemAdministrator)
            {
                roleNames += PredefinedRole.SystemAdministrator.GetDisplayName();
            }
            else
            {
                roleNames += Utilities.ToString(roleNameList);
            }
            return roleNames;
        }
    }
}
