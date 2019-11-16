using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class UserGroupDao : IRepository
    {
        private readonly Repository<UserGroup> _repository;

        public UserGroupDao()
        {
            _repository = new Repository<UserGroup>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(UserGroup group)
        {
            var groupId = await _repository.InsertAsync(group);
            UserGroupManager.ClearCache();
            return groupId;
        }

        public async Task UpdateAsync(UserGroup group)
        {
            await _repository.UpdateAsync(group);
            UserGroupManager.ClearCache();
        }

        public async Task DeleteAsync(int groupId)
        {
            await _repository.DeleteAsync(groupId);
            UserGroupManager.ClearCache();
        }

        public async Task<List<UserGroup>> GetUserGroupListAsync()
        {
            var config = await ConfigManager.GetInstanceAsync();

            var list = (await _repository.GetAllAsync(Q.OrderBy(nameof(UserGroup.Id)))).ToList();

            list.Insert(0, new UserGroup
            {
                Id = 0,
                GroupName = "默认用户组",
                AdminName = config.UserDefaultGroupAdminName
            });

            return list;
        }
    }
}
