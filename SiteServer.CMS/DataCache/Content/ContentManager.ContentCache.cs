using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.DataCache.Content
{
    public static partial class ContentManager
    {
        private static class ContentCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(ContentCache);

            private static string GetCacheKey(int channelId)
            {
                return $"{CachePrefix}.{channelId}";
            }

            public static void Remove(int channelId)
            {
                lock (LockObject)
                {
                    var cacheKey = GetCacheKey(channelId);
                    DataCacheManager.Remove(cacheKey);
                }
            }

            public static Dictionary<int, Model.Content> GetContentDict(int channelId)
            {
                lock (LockObject)
                {
                    var cacheKey = GetCacheKey(channelId);
                    var dict = DataCacheManager.Get<Dictionary<int, Model.Content>>(cacheKey);
                    if (dict == null)
                    {
                        dict = new Dictionary<int, Model.Content>();
                        DataCacheManager.InsertHours(cacheKey, dict, 12);
                    }

                    return dict;
                }
            }

            public static async Task<Model.Content> GetContentAsync(Site site, int channelId, int contentId)
            {
                var dict = GetContentDict(channelId);
                dict.TryGetValue(contentId, out var contentInfo);
                if (contentInfo != null && contentInfo.ChannelId == channelId && contentInfo.Id == contentId) return contentInfo;

                contentInfo = await DataProvider.ContentDao.GetContentAsync(await ChannelManager.GetTableNameAsync(site, channelId), contentId);
                dict[contentId] = contentInfo;

                return new Model.Content(contentInfo.ToDictionary());
            }

            public static async Task<Model.Content> GetContentAsync(Site site, Channel channel, int contentId)
            {
                var dict = GetContentDict(channel.Id);
                dict.TryGetValue(contentId, out var contentInfo);
                if (contentInfo != null && contentInfo.ChannelId == channel.Id && contentInfo.Id == contentId) return contentInfo;

                contentInfo = await DataProvider.ContentDao.GetContentAsync(await ChannelManager.GetTableNameAsync(site, channel), contentId);
                dict[contentId] = contentInfo;

                return new Model.Content(contentInfo.ToDictionary());
            }
        }

        public static async Task<Model.Content> GetContentInfoAsync(Site site, int channelId, int contentId)
        {
            return await ContentCache.GetContentAsync(site, channelId, contentId);
        }

        public static async Task<Model.Content> GetContentInfoAsync(Site site, Channel channel, int contentId)
        {
            return await ContentCache.GetContentAsync(site, channel, contentId);
        }
    }
}