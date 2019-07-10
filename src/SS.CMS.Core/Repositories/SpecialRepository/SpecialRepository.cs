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
    public partial class SpecialRepository : ISpecialRepository
    {
        private readonly IDistributedCache _cache;
        private readonly Repository<Special> _repository;
        private readonly ISettingsManager _settingsManager;

        public SpecialRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _repository = new Repository<Special>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(Special.Id);
            public const string SiteId = nameof(Special.SiteId);
            public const string Title = nameof(Special.Title);
            public const string Url = nameof(Special.Url);
        }

        public async Task<int> InsertAsync(Special specialInfo)
        {
            specialInfo.Id = await _repository.InsertAsync(specialInfo);

            await RemoveCacheAsync(specialInfo.SiteId);

            return specialInfo.Id;
        }

        public async Task<bool> UpdateAsync(Special specialInfo)
        {
            var updated = await _repository.UpdateAsync(specialInfo);

            await RemoveCacheAsync(specialInfo.SiteId);

            return updated;
        }

        public async Task<Special> DeleteAsync(int siteId, int specialId)
        {
            if (specialId <= 0) return null;

            var specialInfo = await GetSpecialInfoAsync(siteId, specialId);

            await _repository.DeleteAsync(specialId);

            await RemoveCacheAsync(siteId);

            return specialInfo;
        }

        public async Task<bool> IsTitleExistsAsync(int siteId, string title)
        {
            return await _repository.ExistsAsync(Q.Where(Attr.SiteId, siteId).Where(Attr.Title, title));
        }

        public async Task<bool> IsUrlExistsAsync(int siteId, string url)
        {
            return await _repository.ExistsAsync(Q.Where(Attr.SiteId, siteId).Where(Attr.Url, url));
        }

        public async Task<IEnumerable<Special>> GetSpecialInfoListAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q.Where(Attr.SiteId, siteId).OrderByDesc(Attr.Id));
        }

        public async Task<IEnumerable<Special>> GetSpecialInfoListAsync(int siteId, string keyword)
        {
            return await _repository.GetAllAsync(Q
                .Where(Attr.SiteId, siteId)
                .OrWhereContains(Attr.Title, keyword)
                .OrWhereContains(Attr.Url, keyword)
                .OrderByDesc(Attr.Id));
        }

        public async Task<Dictionary<int, Special>> GetSpecialInfoDictionaryBySiteIdAsync(int siteId)
        {
            return await _cache.GetOrCreateAsync(_cache.GetKey(nameof(SpecialRepository), siteId.ToString()), async options =>
            {
                var specialInfoList = await GetSpecialInfoListAsync(siteId);

                return specialInfoList.ToDictionary(specialInfo => specialInfo.Id);
            });
        }
    }
}