using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS;
using SSCMS.Core.Utils;

namespace SSCMS.Core.Repositories.SpecialRepository
{
    public partial class SpecialRepository
    {
        private readonly string _cacheKey = CacheUtils.GetCacheKey(nameof(SpecialRepository));
        private readonly object _syncRoot = new object();

        public async Task<Special> GetSpecialAsync(int siteId, int specialId)
        {
            Special special = null;
            var specialDictionary = await GetSpecialDictionaryBySiteIdAsync(siteId);

            if (specialDictionary != null && specialDictionary.ContainsKey(specialId))
            {
                special = specialDictionary[specialId];
            }
            return special;
        }

        public async Task<string> GetTitleAsync(int siteId, int specialId)
        {
            var title = string.Empty;

            var special = await GetSpecialAsync(siteId, specialId);
            if (special != null)
            {
                title = special.Title;
            }

            return title;
        }

        public async Task<List<int>> GetAllSpecialIdListAsync(int siteId)
        {
            var list = new List<int>();

            var specialDictionary = await GetSpecialDictionaryBySiteIdAsync(siteId);
            if (specialDictionary == null) return list;

            foreach (var special in specialDictionary.Values)
            {
                list.Add(special.Id);
            }

            return list;
        }

        private void RemoveCache(int siteId)
        {
            var dictionary = GetCacheDictionary();

            lock (_syncRoot)
            {
                dictionary.Remove(siteId);
            }
        }

        private Dictionary<int, Dictionary<int, Special>> GetCacheDictionary()
        {
            var dictionary = CacheUtils.Get<Dictionary<int, Dictionary<int, Special>>>(_cacheKey);
            if (dictionary != null) return dictionary;

            dictionary = new Dictionary<int, Dictionary<int, Special>>();
            CacheUtils.InsertHours(_cacheKey, dictionary, 24);
            return dictionary;
        }
    }
}
