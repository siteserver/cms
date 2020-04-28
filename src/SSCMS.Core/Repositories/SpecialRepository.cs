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
    public partial class SpecialRepository : ISpecialRepository
    {
        private readonly Repository<Special> _repository;

        public SpecialRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Special>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Special special)
        {
            var specialId = await _repository.InsertAsync(special, Q
                .CachingRemove(CacheKey(special.SiteId))
            );
            return specialId;
        }

        public async Task UpdateAsync(Special special)
        {
            await _repository.UpdateAsync(special, Q
                .CachingRemove(CacheKey(special.SiteId))
            );
        }

        public async Task DeleteAsync(int siteId, int specialId)
        {
            if (specialId <= 0) return;

            await _repository.DeleteAsync(specialId, Q
                .CachingRemove(CacheKey(siteId))
            );
        }

        public async Task<bool> IsTitleExistsAsync(int siteId, string title)
        {
            var specials = await GetSpecialsAsync(siteId);
            return specials.Exists(x => x.Url == title);
        }

        public async Task<bool> IsUrlExistsAsync(int siteId, string url)
        {
            var specials = await GetSpecialsAsync(siteId);
            return specials.Exists(x => x.Url == url);
        }

        public async Task<List<Special>> GetSpecialsAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Special.SiteId), siteId)
                .OrderByDesc(nameof(Special.Id))
                .CachingGet(CacheKey(siteId))
            );
        }

        private string CacheKey(int siteId) => CacheUtils.GetListKey(TableName, siteId);

        public async Task<Special> GetSpecialAsync(int siteId, int specialId)
        {
            var specials = await GetSpecialsAsync(siteId);
            return specials.FirstOrDefault(x => x.Id == specialId);
        }

        public async Task<string> GetTitleAsync(int siteId, int specialId)
        {
            var special = await GetSpecialAsync(siteId, specialId);
            return special != null ? special.Title : string.Empty;
        }

        public async Task<List<int>> GetAllSpecialIdListAsync(int siteId)
        {
            var specials = await GetSpecialsAsync(siteId);
            return specials.Select(x => x.Id).ToList();
        }
    }
}
