using System.Collections;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Core.Repositories
{
    public partial class TagRepository
    {
        public async Task<SortedList> ReadContentAsync(int siteId)
        {
            var cacheKey = _cache.GetKey(nameof(TagRepository), nameof(ReadContentAsync), siteId.ToString());
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                var arrText = new SortedList();

                var tagList = await GetTagListAsync(siteId);
                if (tagList != null)
                {
                    foreach (var line in tagList)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            arrText.Add(line.Trim(), line.Trim());
                        }
                    }
                }
                return arrText;
            });
        }
    }
}