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
        private readonly Repository<UserRole> _repository;

        public UserRoleRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<UserRole>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(UserRole.Id);
            public const string Guid = nameof(UserRole.Guid);
            public const string LastModifiedDate = nameof(UserRole.LastModifiedDate);
            public const string RoleId = nameof(UserRole.RoleId);
            public const string UserId = nameof(UserRole.UserId);
        }

        public async Task<IEnumerable<int>> GetUserNameListByRoleNameAsync(int roleId)
        {
            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.UserId)
                .Where(Attr.RoleId, roleId)
                .Distinct());
        }

        public async Task RemoveUserAsync(int userId)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.UserId, userId));
        }

        public async Task RemoveUserFromRoleAsync(int userId, int roleId)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.UserId, userId)
                .Where(Attr.RoleId, roleId));
        }

        public async Task<bool> IsUserInRoleAsync(int userId, int roleId)
        {
            return await _repository.ExistsAsync(Q
                .Where(Attr.UserId, userId)
                .Where(Attr.RoleId, roleId));
        }

        public async Task<int> AddUserToRoleAsync(int userId, int roleId)
        {
            if (!await IsUserInRoleAsync(userId, roleId))
            {
                return await _repository.InsertAsync(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }

            return 0;
        }

        public async Task<IEnumerable<int>> GetRolesAsync(int userId)
        {
            var roleIdList = new List<int>();

            return await _repository.GetAllAsync<int>(Q
                .Select(Attr.RoleId)
                .Where(Attr.UserId, userId)
                .Distinct());

            // foreach (var role in roles)
            // {
            //     if (role == AuthTypes.Roles.SuperAdministrator)
            //     {
            //         return new List<string> {
            //             AuthTypes.Roles.SuperAdministrator
            //         };
            //     }
            //     else
            //     {
            //         roleNameList.Add(role);
            //     }
            // }

            // return roleNameList;
        }
    }
}
