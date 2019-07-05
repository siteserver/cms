using System.Collections.Generic;
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

            public static Dictionary<int, ContentInfo> GetContentDict(int channelId)
            {
                lock (LockObject)
                {
                    var cacheKey = GetCacheKey(channelId);
                    var dict = DataCacheManager.Get<Dictionary<int, ContentInfo>>(cacheKey);
                    if (dict == null)
                    {
                        dict = new Dictionary<int, ContentInfo>();
                        DataCacheManager.InsertHours(cacheKey, dict, 12);
                    }

                    return dict;
                }
            }

            public static ContentInfo GetContent(SiteInfo siteInfo, int channelId, int contentId)
            {
                lock (LockObject)
                {
                    var dict = GetContentDict(channelId);
                    dict.TryGetValue(contentId, out var contentInfo);
                    if (contentInfo != null && contentInfo.ChannelId == channelId && contentInfo.Id == contentId) return contentInfo;

                    contentInfo = DataProvider.ContentDao.GetCacheContentInfo(ChannelManager.GetTableName(siteInfo, channelId), channelId, contentId);
                    dict[contentId] = contentInfo;

                    return new ContentInfo(contentInfo);
                }
            }

            public static ContentInfo GetContent(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
            {
                lock (LockObject)
                {
                    var dict = GetContentDict(channelInfo.Id);
                    dict.TryGetValue(contentId, out var contentInfo);
                    if (contentInfo != null && contentInfo.ChannelId == channelInfo.Id && contentInfo.Id == contentId) return contentInfo;

                    contentInfo = DataProvider.ContentDao.GetCacheContentInfo(ChannelManager.GetTableName(siteInfo, channelInfo), channelInfo.Id, contentId);
                    dict[contentId] = contentInfo;

                    return new ContentInfo(contentInfo);
                }
            }
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, int channelId, int contentId)
        {
            return ContentCache.GetContent(siteInfo, channelId, contentId);
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            return ContentCache.GetContent(siteInfo, channelInfo, contentId);
        }
    }
}