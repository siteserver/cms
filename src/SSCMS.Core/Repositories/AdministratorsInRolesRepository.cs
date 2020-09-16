using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public class AdministratorsInRolesRepository : IAdministratorsInRolesRepository
    {
        private readonly Repository<AdministratorsInRoles> _repository;

        public AdministratorsInRolesRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<AdministratorsInRoles>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<IList<string>> GetRolesForUserAsync(string userName)
        {
            var roleNames = await _repository.GetAllAsync<string>(Q
                .Select(nameof(AdministratorsInRoles.RoleName))
                .Where(nameof(AdministratorsInRoles.UserName), userName)
                .OrderBy(nameof(AdministratorsInRoles.RoleName))
            );
            var defaultRole = PredefinedRole.Administrator.GetValue();
            var list = new List<string> { defaultRole };
            if (roleNames != null)
            {
                list.AddRange(roleNames.Where(roleName => !StringUtils.EqualsIgnoreCase(roleName, defaultRole)));
            }

            return list;
        }

        public async Task<IList<string>> GetUsersInRoleAsync(string roleName)
        {
            var userNames = await _repository.GetAllAsync<string>(Q
                .Select(nameof(AdministratorsInRoles.UserName))
                .Where(nameof(AdministratorsInRoles.RoleName), roleName)
                .OrderBy(nameof(AdministratorsInRoles.UserName))
            );
            return userNames.ToList();
        }

        public async Task RemoveUserAsync(string userName)
        {
            await _repository.DeleteAsync(Q.Where(nameof(AdministratorsInRoles.UserName), userName));
        }

        private async Task<bool> IsUserInRoleAsync(string userName, string roleName)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(AdministratorsInRoles.UserName), userName)
                .Where(nameof(AdministratorsInRoles.RoleName), roleName)
            );
        }

        public async Task InsertAsync(string userName, string roleName)
        {
            if (!await IsUserInRoleAsync(userName, roleName))
            {
                await _repository.InsertAsync(new AdministratorsInRoles
                {
                    RoleName = roleName,
                    UserName = userName
                });
            }
        }

        
    }
}
