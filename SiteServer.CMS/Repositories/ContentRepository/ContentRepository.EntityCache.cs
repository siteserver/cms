using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        private string GetCacheKey(IRepository repository, int contentId)
        {
            return _cache.GetEntityKey(repository, contentId);
        }

        private async Task RemoveEntityCacheAsync(IRepository repository, int contentId)
        {
            if (repository == null || string.IsNullOrEmpty(repository.TableName) || contentId <= 0) return;

            var cacheKey = GetCacheKey(repository, contentId);

            await _cache.RemoveAsync(cacheKey);
        }

        private async Task SetEntityCacheAsync(IRepository repository, Content content)
        {
            if (repository == null || string.IsNullOrEmpty(repository.TableName) || content == null) return;

            var cacheKey = GetCacheKey(repository, content.Id);

            await _cache.SetAsync(cacheKey, content);
        }

        private async Task<Content> GetAsync(string tableName, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

            var repository = GetRepository(tableName);
            var cacheKey = GetCacheKey(repository, contentId);
            return await _cache.GetOrCreateAsync(cacheKey, async options => await repository.GetAsync(contentId));
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