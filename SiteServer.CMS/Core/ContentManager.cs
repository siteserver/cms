using System.Collections.Generic;
using System.Linq;
using NuGet;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Cache;

namespace SiteServer.CMS.Core
{
    public static class ContentManager
    {
        private static class ContentManagerCache
        {
            private static readonly object LockObject = new object();
            private const string CacheKey = "SiteServer.CMS.Core.ContentManager";

            private static string GetPageCacheKey(int siteId, int channelId)
            {
                return $"{CacheKey}.{siteId}.{channelId}";
            }

            public static void Remove(int siteId, int channelId)
            {
                var cacheKey = GetPageCacheKey(siteId, channelId);
                CacheUtils.Remove(cacheKey);
            }

            public static List<ContentInfo> GetContentInfoList(SiteInfo siteInfo, ChannelInfo channelInfo)
            {
                var cacheKey = GetPageCacheKey(siteInfo.Id, channelInfo.Id);
                var list = CacheUtils.Get<List<ContentInfo>>(cacheKey);
                if (list != null) return list;

                list = new List<ContentInfo>();
                CacheUtils.Insert(cacheKey, list);
                return list;
            }
        }

        public static void RemoveCache(int siteId, int channelId)
        {
            ContentManagerCache.Remove(siteId, channelId);
            Content.ClearCache();
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            var list = ContentManagerCache.GetContentInfoList(siteInfo, channelInfo);
            var contentInfo = list.FirstOrDefault(o => o.Id == contentId);
            if (contentInfo != null) return contentInfo;

            return DataProvider.ContentDao.GetContentInfo(ChannelManager.GetTableName(siteInfo, channelInfo),
                contentId);
        }

        public static List<ContentInfo> GetContentInfoList(SiteInfo siteInfo, ChannelInfo channelInfo, int page)
        {
            var list = ContentManagerCache.GetContentInfoList(siteInfo, channelInfo);
            var offset = siteInfo.Additional.PageSize * (page - 1);
            var limit = siteInfo.Additional.PageSize;
            if (list.Count > offset)
            {
                return list.Skip(offset).Take(limit).ToList();
            }

            if (list.Count + limit == page * limit)
            {
                list.AddRange(DataProvider.ContentDao.GetCacheList(siteInfo, channelInfo, offset, limit));
                return list;
            }

            return DataProvider.ContentDao.GetCacheList(siteInfo, channelInfo, offset, limit);
        }
    }
}