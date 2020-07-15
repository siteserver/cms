using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class OpenMenuRepository : IOpenMenuRepository
    {
        private readonly Repository<OpenMenu> _repository;

        public OpenMenuRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<OpenMenu>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int siteId) => CacheUtils.GetListKey(_repository.TableName, siteId);

        public async Task<int> InsertAsync(OpenMenu openMenu)
        {
            return await _repository.InsertAsync(openMenu, Q
                .CachingRemove(GetCacheKey(openMenu.SiteId))
            );
        }

        public async Task UpdateAsync(OpenMenu openMenu)
        {
            await _repository.UpdateAsync(openMenu, Q
                .CachingRemove(GetCacheKey(openMenu.SiteId))
            );
        }

        public async Task DeleteAsync(int siteId, int menuId)
        {
            await _repository.DeleteAsync(menuId, Q
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(OpenMenu.SiteId), siteId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task<List<OpenMenu>> GetOpenMenusAsync(int siteId)
        {
            var infoList = await _repository.GetAllAsync(Q
                .CachingGet(GetCacheKey(siteId))
            );
            var list = infoList.ToList();

            return list.OrderBy(openMenu => openMenu.Taxis == 0 ? int.MaxValue : openMenu.Taxis).ToList();
        }

        public async Task<OpenMenu> GetAsync(int siteId, int id)
        {
            var infoList = await _repository.GetAllAsync(Q
                .CachingGet(GetCacheKey(siteId))
            );
            return infoList.FirstOrDefault(x => x.Id == id);
        }
    }
}
