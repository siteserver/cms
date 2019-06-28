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
        private readonly Repository<RoleInfo> _repository;
        public RoleRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<RoleInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;


        private static class Attr
        {
            public const string RoleName = nameof(RoleInfo.RoleName);
            public const string CreatorUserName = nameof(RoleInfo.CreatorUserName);
            public const string Description = nameof(RoleInfo.Description);
        }

        public string GetRoleDescription(string roleName)
        {
            return _repository.Get<string>(Q
                .Select(Attr.Description)
                .Where(Attr.RoleName, roleName));
        }

        public IList<string> GetRoleNameList()
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.RoleName)
                .OrderBy(Attr.RoleName)).ToList();
        }

        public IList<string> GetRoleNameListByCreatorUserName(string creatorUserName)
        {
            if (string.IsNullOrEmpty(creatorUserName)) return new List<string>();

            return _repository.GetAll<string>(Q
                .Select(Attr.RoleName)
                .Where(Attr.CreatorUserName, creatorUserName)
                .OrderBy(Attr.RoleName)).ToList();
        }

        public void InsertRole(RoleInfo roleInfo)
        {
            _repository.Insert(roleInfo);
        }

        public void UpdateRole(string roleName, string description)
        {
            _repository.Update(Q
                .Set(Attr.Description, description)
                .Where(Attr.RoleName, roleName)
            );
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            await _repository.DeleteAsync(Q.Where(Attr.RoleName, roleName));
        }

        public bool IsRoleExists(string roleName)
        {
            return _repository.Exists(Q.Where(Attr.RoleName, roleName));
        }
    }
}
