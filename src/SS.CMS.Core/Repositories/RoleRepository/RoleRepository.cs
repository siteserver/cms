using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class RoleRepository : IRoleRepository
    {
        private readonly Repository<Role> _repository;
        public RoleRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Role>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;


        private static class Attr
        {
            public const string RoleName = nameof(Role.RoleName);
            public const string UserId = nameof(Role.UserId);
            public const string Description = nameof(Role.Description);
        }

        public async Task<string> GetRoleDescriptionAsync(string roleName)
        {
            return await _repository.GetAsync<string>(Q
                .Select(Attr.Description)
                .Where(Attr.RoleName, roleName));
        }

        public async Task<IEnumerable<string>> GetRoleNameListAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(Attr.RoleName)
                .OrderBy(Attr.RoleName));
        }

        public async Task<IEnumerable<string>> GetRoleNameListByUserIdAsync(int userId)
        {
            if (userId == 0) return new List<string>();

            return await _repository.GetAllAsync<string>(Q
                .Select(Attr.RoleName)
                .Where(Attr.UserId, userId)
                .OrderBy(Attr.RoleName));
        }

        public async Task<int> InsertAsync(Role roleInfo)
        {
            return await _repository.InsertAsync(roleInfo);
        }

        public async Task UpdateAsync(string roleName, string description)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.Description, description)
                .Where(Attr.RoleName, roleName)
            );
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            await _repository.DeleteAsync(Q.Where(Attr.RoleName, roleName));
        }

        public async Task<bool> IsRoleExistsAsync(string roleName)
        {
            return await _repository.ExistsAsync(Q.Where(Attr.RoleName, roleName));
        }
    }
}
