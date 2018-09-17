using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache
{
    public static class ContentManager
    {
        private static class ListCache
        {
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(ListCache);

            private static string GetCacheKey(int channelId)
            {
                return $"{CachePrefix}.{channelId}";
            }

            public static void Remove(int channelId)
            {
                var cacheKey = GetCacheKey(channelId);
                DataCacheManager.Remove(cacheKey);
            }

            public static List<ContentInfo> GetContentInfoList(int channelId)
            {
                var cacheKey = GetCacheKey(channelId);
                var list = DataCacheManager.Get<List<ContentInfo>>(cacheKey);
                if (list != null) return list;

                list = new List<ContentInfo>();
                DataCacheManager.Insert(cacheKey, list);
                return list;
            }
        }

        private static class ContentCache
        {
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(ContentCache);

            private static string GetCacheKey(int channelId)
            {
                return $"{CachePrefix}.{channelId}";
            }

            public static void Remove(int channelId)
            {
                var cacheKey = GetCacheKey(channelId);
                DataCacheManager.Remove(cacheKey);
            }

            public static Dictionary<int, ContentInfo> GetContentDict(int channelId)
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

        public static void RemoveCache(int channelId)
        {
            ListCache.Remove(channelId);
            ContentCache.Remove(channelId);
            StlContentCache.ClearCache();
        }

        public static void InsertCache(ChannelInfo channelInfo, IContentInfo contentInfo)
        {
            if (contentInfo.SourceId == SourceManager.Preview) return;

            var list = ListCache.GetContentInfoList(channelInfo.Id);

            if (ETaxisTypeUtils.Equals(ETaxisType.OrderByTaxisDesc, channelInfo.Additional.DefaultTaxisType))
            {
                list.Insert(0, (ContentInfo)contentInfo);
            }
            else
            {
                ListCache.Remove(channelInfo.Id);
            }

            var dict = ContentCache.GetContentDict(channelInfo.Id);
            dict[contentInfo.Id] = (ContentInfo)contentInfo;

            StlContentCache.ClearCache();
        }

        public static void UpdateCache(ChannelInfo channelInfo, IContentInfo contentInfoToUpdate)
        {
            var list = ListCache.GetContentInfoList(channelInfo.Id);
            var contentInfo = list.FirstOrDefault(o => o.Id == contentInfoToUpdate.Id);
            if (contentInfo != null)
            {
                var isClearCache = contentInfo.IsTop != contentInfoToUpdate.IsTop;

                if (!isClearCache)
                {
                    var orderAttributeName =
                        ETaxisTypeUtils.GetContentOrderAttributeName(
                            ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType));
                    if (contentInfo.Get(orderAttributeName) != contentInfoToUpdate.Get(orderAttributeName))
                    {
                        isClearCache = true;
                    }
                }

                if (isClearCache)
                {
                    ListCache.Remove(channelInfo.Id);
                }
                else
                {
                    list[list.IndexOf(contentInfo)] = (ContentInfo)contentInfoToUpdate;
                }
            }

            var dict = ContentCache.GetContentDict(channelInfo.Id);
            dict[contentInfoToUpdate.Id] = (ContentInfo)contentInfoToUpdate;

            StlContentCache.ClearCache();
        }

        public static List<ContentInfo> GetContentInfoList(SiteInfo siteInfo, ChannelInfo channelInfo, int offset, int limit)
        {
            var list = ListCache.GetContentInfoList(channelInfo.Id);
            if (list.Count > offset + limit)
            {
                return list.Skip(offset).Take(limit).ToList();
            }

            if (list.Count == offset)
            {
                var pageList = DataProvider.ContentDao.GetCacheList(siteInfo, channelInfo, offset, limit);
                list.AddRange(pageList);
                return pageList;
            }

            return DataProvider.ContentDao.GetCacheList(siteInfo, channelInfo, offset, limit);
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, int channelId, int contentId)
        {
            var list = ListCache.GetContentInfoList(channelId);
            var contentInfo = list.FirstOrDefault(o => o.Id == contentId);
            if (contentInfo != null) return contentInfo;

            var dict = ContentCache.GetContentDict(channelId);
            dict.TryGetValue(contentId, out contentInfo);
            if (contentInfo != null) return contentInfo;

            contentInfo = DataProvider.ContentDao.GetCacheContentInfo(ChannelManager.GetTableName(siteInfo, channelId), contentId);
            dict[contentId] = contentInfo;

            return contentInfo;
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            var list = ListCache.GetContentInfoList(channelInfo.Id);
            var contentInfo = list.FirstOrDefault(o => o.Id == contentId);
            if (contentInfo != null) return contentInfo;

            var dict = ContentCache.GetContentDict(channelInfo.Id);
            dict.TryGetValue(contentId, out contentInfo);
            if (contentInfo != null) return contentInfo;

            contentInfo = DataProvider.ContentDao.GetCacheContentInfo(ChannelManager.GetTableName(siteInfo, channelInfo), contentId);
            dict[contentId] = contentInfo;

            return contentInfo;
        }
    }
}