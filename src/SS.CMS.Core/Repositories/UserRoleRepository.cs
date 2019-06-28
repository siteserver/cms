using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly Repository<UserRoleInfo> _repository;

        public UserRoleRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<UserRoleInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(UserRoleInfo.Id);
            public const string Guid = nameof(UserRoleInfo.Guid);
            public const string LastModifiedDate = nameof(UserRoleInfo.LastModifiedDate);
            public const string RoleName = nameof(UserRoleInfo.RoleName);
            public const string UserName = nameof(UserRoleInfo.UserName);
        }

        public IList<string> GetUserNameListByRoleName(string roleName)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.UserName)
                .Where(Attr.RoleName, roleName)
                .Distinct()).ToList();
        }

        public async Task RemoveUserAsync(string userName)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.UserName, userName));
        }

        public async Task RemoveUserFromRoleAsync(string userName, string roleName)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.UserName, userName)
                .Where(Attr.RoleName, roleName));
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            return _repository.Exists(Q
                .Where(Attr.UserName, userName)
                .Where(Attr.RoleName, roleName));
        }

        public int AddUserToRole(string userName, string roleName)
        {
            if (!IsUserInRole(userName, roleName))
            {
                return _repository.Insert(new UserRoleInfo
                {
                    UserName = userName,
                    RoleName = roleName
                });
            }

            return 0;
        }

        public async Task<IList<string>> GetRolesAsync(string userName)
        {
            var roleNameList = new List<string>();

            var roles = await _repository.GetAllAsync<string>(Q
                .Select(Attr.RoleName)
                .Where(Attr.UserName, userName)
                .Distinct());

            foreach (var role in roles)
            {
                if (role == AuthTypes.Roles.SuperAdministrator)
                {
                    return new List<string> {
                        AuthTypes.Roles.SuperAdministrator
                    };
                }
                else
                {
                    roleNameList.Add(role);
                }
            }

            return roleNameList;
        }
    }
}
