using System.Threading.Tasks;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace SS.CMS.Core.Repositories
{
    public partial class TemplateRepository
    {
        public string GetCacheKey(int templateId)
        {
            return _cache.GetKey(nameof(TemplateRepository), nameof(GetCacheKey), templateId.ToString());
        }

        public async Task RemoveCacheAsync(int templateId)
        {
            await _cache.RemoveAsync(GetCacheKey(templateId));
        }

        public async Task<Template> GetTemplateInfoAsync(int templateId)
        {
            return await _cache.GetOrCreateAsync(GetCacheKey(templateId), async options =>
            {
                return await _repository.GetAsync(templateId);
            });
        }
    }
}
