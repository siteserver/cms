using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        private string GetCacheKey(string tableName, int contentId)
        {
            return CacheManager.GetEntityKey(tableName, contentId);
        }

        private async Task<Content> GetAsync(string tableName, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

            var repository = GetRepository(tableName);
            return await repository.GetAsync(contentId, Q.CachingGet(GetCacheKey(tableName, contentId)));
        }

        public async Task<Content> GetAsync(Site site, int channelId, int contentId)
        {
            var tableName = await ChannelManager.GetTableNameAsync(site, channelId);
            return await GetAsync(tableName, contentId);
        }

        public async Task<Content> GetAsync(Site site, Channel channel, int contentId)
        {
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
            return await GetAsync(tableName, contentId);
        }
    }
}