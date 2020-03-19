using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS;

namespace SSCMS.Core.Repositories.SpecialRepository
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
            var specialId = await _repository.InsertAsync(special);
            RemoveCache(special.SiteId);
            return specialId;
        }

        public async Task UpdateAsync(Special special)
        {
            await _repository.UpdateAsync(special);
            RemoveCache(special.SiteId);
        }

        public async Task DeleteAsync(int siteId, int specialId)
        {
            if (specialId <= 0) return;
            await _repository.DeleteAsync(specialId);
            RemoveCache(siteId);
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

        public async Task<List<Special>> GetSpecialListAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(Special.SiteId), siteId)
                .OrderByDesc(nameof(Special.Id))
            );
        }

        private async Task<Dictionary<int, Special>> GetSpecialDictionaryBySiteIdAsync(int siteId)
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
