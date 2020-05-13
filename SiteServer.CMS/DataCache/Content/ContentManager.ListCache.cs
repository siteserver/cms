using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache.Content
{
    public static partial class ContentManager
    {
        private static string ListCacheKey(int siteId, int channelId, int adminId, bool isAllContents) =>
            $"{nameof(ContentManager)}:{siteId}:{channelId}:{adminId}:{isAllContents}";

        public static void RemoveListCache(int siteId, int channelId)
        {
            CacheUtils.RemoveByStartString($"{nameof(ContentManager)}:{siteId}:{channelId}:");
        }

        public static List<(int ChannelId, int ContentId)> GetChannelContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int adminId, bool isAllContents)
        {
            var cacheKey = ListCacheKey(siteInfo.Id, channelInfo.Id, adminId, isAllContents);
            var retVal = CacheUtils.Get<List<(int ChannelId, int ContentId)>>(cacheKey);
            if (retVal != null) return retVal;

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            retVal = DataProvider.ContentDao.GetCacheChannelContentIdList(tableName, DataProvider.ContentDao.GetCacheWhereString(siteInfo, channelInfo, adminId, isAllContents, string.Empty, string.Empty),
                DataProvider.ContentDao.GetOrderString(string.Empty, isAllContents));

            CacheUtils.Insert(cacheKey, retVal);

            return retVal;
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked)
        {
            return CountCache.GetChannelCountByIsChecked(siteInfo, channelInfo, isChecked);
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            var ccIds = GetChannelContentIdList(siteInfo, channelInfo, 0, false);
            return ccIds.Count();
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, int adminId, bool isAllContents = false)
        {
            var ccIds = GetChannelContentIdList(siteInfo, channelInfo, adminId, isAllContents);
            return ccIds.Count();
        }
    }
}