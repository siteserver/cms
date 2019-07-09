using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class UserMenuRepository : IUserMenuRepository
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<UserMenuInfo> _repository;

        public UserMenuRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(UserMenuRepository));
            _settingsManager = settingsManager;
            _repository = new Repository<UserMenuInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(UserMenuInfo.Id);
            public const string ParentId = nameof(UserMenuInfo.ParentId);
        }

        public async Task<int> InsertAsync(UserMenuInfo menuInfo)
        {
            menuInfo.Id = await _repository.InsertAsync(menuInfo);

            await _cache.RemoveAsync(_cacheKey);

            return menuInfo.Id;
        }

        public async Task<bool> UpdateAsync(UserMenuInfo menuInfo)
        {
            var updated = await _repository.UpdateAsync(menuInfo);

            await _cache.RemoveAsync(_cacheKey);

            return updated;
        }

        public async Task<bool> DeleteAsync(int menuId)
        {
            await _repository.DeleteAsync(Q.Where(Attr.Id, menuId).OrWhere(Attr.ParentId, menuId));

            await _cache.RemoveAsync(_cacheKey);

            return true;
        }

        private async Task<List<UserMenuInfo>> GetUserMenuInfoListToCacheAsync()
        {
            var list = new List<UserMenuInfo>();
            list.AddRange(await _repository.GetAllAsync());

            var systemMenus = SystemMenus.Value;
            foreach (var kvp in systemMenus)
            {
                var parent = kvp.Key;
                var children = kvp.Value;

                if (list.All(x => x.SystemId != parent.SystemId))
                {
                    parent.Id = await InsertAsync(parent);
                    list.Add(parent);
                }
                else
                {
                    parent = list.First(x => x.SystemId == parent.SystemId);
                }

                if (children != null)
                {
                    foreach (var child in children)
                    {
                        if (list.All(x => x.SystemId != child.SystemId))
                        {
                            child.ParentId = parent.Id;
                            child.Id = await InsertAsync(child);
                            list.Add(child);
                        }
                    }
                }
            }

            return list.OrderBy(menuInfo => menuInfo.Taxis == 0 ? int.MaxValue : menuInfo.Taxis).ToList();
        }
    }
}
