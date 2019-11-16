using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class PermissionsInRolesDao : IRepository
    {
        private readonly Repository<PermissionsInRoles> _repository;

        public PermissionsInRolesDao()
        {
            _repository = new Repository<PermissionsInRoles>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
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

		public async Task<List<string>> GetGeneralPermissionListAsync(IEnumerable<string> roles)
		{
            var list = new List<string>();
		    if (roles == null) return list;

            foreach (var roleName in roles)
			{
                var pr = await GetPermissionsInRolesAsync(roleName);
                if (pr?.GeneralPermissions == null) continue;

                foreach (var permission in pr.GeneralPermissionList)
                {
                    if (!list.Contains(permission)) list.Add(permission);
                }
            }

			return list;
		}
	}
}
