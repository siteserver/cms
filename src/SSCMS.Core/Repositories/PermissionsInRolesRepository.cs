using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class PermissionsInRolesRepository : IPermissionsInRolesRepository
    {
        private readonly Repository<PermissionsInRoles> _repository;

        public PermissionsInRolesRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<PermissionsInRoles>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(PermissionsInRoles pr)
        {
            await _repository.InsertAsync(pr);
        }

        public async Task DeleteAsync(string roleName)
        {
            await _repository.DeleteAsync(Q.Where(nameof(PermissionsInRoles.RoleName), roleName));
        }

        private async Task<PermissionsInRoles> GetPermissionsInRolesAsync(string roleName)
        {
            return await _repository.GetAsync(Q.Where(nameof(PermissionsInRoles.RoleName), roleName));
        }

		public async Task<List<string>> GetAppPermissionsAsync(IEnumerable<string> roles)
		{
            var list = new List<string>();
		    if (roles == null) return list;

            foreach (var roleName in roles)
			{
                var pr = await GetPermissionsInRolesAsync(roleName);
                if (pr?.AppPermissions == null) continue;

                foreach (var permission in pr.AppPermissions)
                {
                    if (!list.Contains(permission)) list.Add(permission);
                }
            }

			return list;
		}
	}
}
