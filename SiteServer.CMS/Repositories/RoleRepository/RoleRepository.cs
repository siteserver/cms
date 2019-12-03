using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;

namespace SiteServer.CMS.Repositories
{
    public class RoleRepository : IRepository
    {
        private readonly Repository<Role> _repository;

        public RoleRepository()
        {
            _repository = new Repository<Role>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<Role> GetRoleAsync(int roleId)
        {
            return await _repository.GetAsync(roleId);
        }

        public async Task<IEnumerable<Role>> GetRoleListAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(nameof(Role.RoleName)));
        }

        public async Task<IEnumerable<Role>> GetRoleListByCreatorUserNameAsync(string creatorUserName)
        {
            if (string.IsNullOrEmpty(creatorUserName)) return new List<Role>();

            return await _repository.GetAllAsync(Q
                .Where(nameof(Role.CreatorUserName), creatorUserName)
                .OrderBy(nameof(Role.RoleName))
            );
        }

        public async Task<IEnumerable<string>> GetRoleNameListAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Role.RoleName))
                .OrderBy(nameof(Role.RoleName))
            );
        }

		public async Task<IEnumerable<string>> GetRoleNameListByCreatorUserNameAsync(string creatorUserName)
		{
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Role.RoleName))
                .Where(nameof(Role.CreatorUserName), creatorUserName)
                .OrderBy(nameof(Role.RoleName))
            );
        }

        public async Task<int> InsertRoleAsync(Role role)
        {
            if (EPredefinedRoleUtils.IsPredefinedRole(role.RoleName)) return 0;

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
	}
}
