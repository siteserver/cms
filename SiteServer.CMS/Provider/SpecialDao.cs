using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class SpecialDao : IRepository
    {
        private readonly Repository<Special> _repository;

        public SpecialDao()
        {
            _repository = new Repository<Special>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Special special)
        {
            var specialId = await _repository.InsertAsync(special);
            SpecialManager.RemoveCache(special.SiteId);
            return specialId;
        }

        public async Task UpdateAsync(Special special)
        {
            await _repository.UpdateAsync(special);
            SpecialManager.RemoveCache(special.SiteId);
        }

        public async Task DeleteAsync(int siteId, int specialId)
        {
            if (specialId <= 0) return;
            await _repository.DeleteAsync(specialId);
            SpecialManager.RemoveCache(siteId);
        }

        public async Task<bool> IsTitleExistsAsync(int siteId, string title)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(Special.SiteId), siteId)
                .Where(nameof(Special.Title), title)
            );
        }

        public async Task<bool> IsUrlExistsAsync(int siteId, string url)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(Special.SiteId), siteId)
                .Where(nameof(Special.Url), url)
            );
        }

        public async Task<IEnumerable<Special>> GetSpecialListAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Special.SiteId), siteId)
                .OrderByDesc(nameof(Special.Id))
            );
        }

        public async Task<IEnumerable<Special>> GetSpecialListAsync(int siteId, string keyword)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Special.SiteId), siteId)
                .Where(q => q
                    .WhereLike(nameof(Special.Title), $"%{keyword}%")
                    .OrWhereLike(nameof(Special.Url), $"%{keyword}%")
                )
                .OrderByDesc(nameof(Special.Id))
            );
        }

        public async Task<Dictionary<int, Special>> GetSpecialDictionaryBySiteIdAsync(int siteId)
        {
            var dictionary = new Dictionary<int, Special>();

            var list = await _repository.GetAllAsync(Q.Where(nameof(Special.SiteId), siteId));
            foreach (var special in list)
            {
                dictionary.Add(special.Id, special);
            }

            return dictionary;
        }
    }
}
