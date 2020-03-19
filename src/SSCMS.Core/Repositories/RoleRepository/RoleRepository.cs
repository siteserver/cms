using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly Repository<Role> _repository;

        public RoleRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Role>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<Role> GetRoleAsync(int roleId)
        {
            return await _repository.GetAsync(roleId);
        }

        public async Task<List<Role>> GetRoleListAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(nameof(Role.RoleName)));
        }

        public async Task<List<Role>> GetRoleListByCreatorUserNameAsync(string creatorUserName)
        {
            if (string.IsNullOrEmpty(creatorUserName)) return new List<Role>();

            return await _repository.GetAllAsync(Q
                .Where(nameof(Role.CreatorUserName), creatorUserName)
                .OrderBy(nameof(Role.RoleName))
            );
        }

        public async Task<List<string>> GetRoleNameListAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Role.RoleName))
                .OrderBy(nameof(Role.RoleName))
            );
        }

		public async Task<List<string>> GetRoleNameListByCreatorUserNameAsync(string creatorUserName)
		{
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Role.RoleName))
                .Where(nameof(Role.CreatorUserName), creatorUserName)
                .OrderBy(nameof(Role.RoleName))
            );
        }

        public async Task<int> InsertRoleAsync(Role role)
        {
            if (IsPredefinedRole(role.RoleName)) return 0;

            return await _repository.InsertAsync(role);
        }

        public async Task UpdateRoleAsync(Role role)
        {
            await _repository.UpdateAsync(role);
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            return await _repository.DeleteAsync(roleId);
        }

        public async Task<bool> IsRoleExistsAsync(string roleName)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(Role.RoleName), roleName));
        }

        public bool IsPredefinedRole(string roleName)
        {
            var roles = TranslateUtils.GetEnums<PredefinedRole>().Select(x => x.GetValue()).ToList();
            return StringUtils.ContainsIgnoreCase(roles, roleName);
        }

        public bool IsConsoleAdministrator(IList<string> roles)
        {
            return roles != null &&
                   StringUtils.ContainsIgnoreCase(roles, PredefinedRole.ConsoleAdministrator.GetValue());
        }

        public bool IsConsoleAdministrator(string role)
        {
            return StringUtils.EqualsIgnoreCase(role, PredefinedRole.ConsoleAdministrator.GetValue());
        }

        public bool IsSystemAdministrator(IList<string> roles)
        {
            return roles != null &&
                   (StringUtils.ContainsIgnoreCase(roles, PredefinedRole.ConsoleAdministrator.GetValue()) ||
                    StringUtils.ContainsIgnoreCase(roles, PredefinedRole.SystemAdministrator.GetValue()));
        }

        public bool IsSystemAdministrator(string role)
        {
            return StringUtils.EqualsIgnoreCase(role, PredefinedRole.ConsoleAdministrator.GetValue()) ||
                    StringUtils.EqualsIgnoreCase(role, PredefinedRole.SystemAdministrator.GetValue());
        }
    }
}
