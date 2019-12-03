using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public class AdministratorsInRolesRepository : IRepository
    {
        private readonly Repository<AdministratorsInRoles> _repository;

        public AdministratorsInRolesRepository()
        {
            _repository = new Repository<AdministratorsInRoles>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
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
            return roleNames.ToList();
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

        public async Task RemoveUserFromRolesAsync(string userName, string[] roleNames)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(AdministratorsInRoles.UserName), userName)
                .WhereIn(nameof(AdministratorsInRoles.RoleName), roleNames)
            );
        }

        public async Task RemoveUserFromRoleAsync(string userName, string roleName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(AdministratorsInRoles.UserName), userName)
                .Where(nameof(AdministratorsInRoles.RoleName), roleName)
            );
        }

        public async Task<IList<string>> FindUsersInRoleAsync(string roleName, string userNameToMatch)
        {
            var userNames = await _repository.GetAllAsync<string>(Q
                .Select(nameof(AdministratorsInRoles.UserName))
                .Where(nameof(AdministratorsInRoles.RoleName), roleName)
                .WhereLike(nameof(AdministratorsInRoles.UserName), $"%{userNameToMatch}%")
                .OrderBy(nameof(AdministratorsInRoles.UserName))
            );
            return userNames.ToList();
        }

        public async Task<bool> IsUserInRoleAsync(string userName, string roleName)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(AdministratorsInRoles.UserName), userName)
                .Where(nameof(AdministratorsInRoles.RoleName), roleName)
            );
        }

        public async Task AddUserToRolesAsync(string userName, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                await AddUserToRoleAsync(userName, roleName);
            }
        }

        public async Task AddUserToRoleAsync(string userName, string roleName)
        {
            if (!await DataProvider.AdministratorRepository.IsUserNameExistsAsync(userName)) return;
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
