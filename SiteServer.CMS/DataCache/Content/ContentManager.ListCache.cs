using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache.Content
{
    public static partial class ContentManager
    {
        //private static string ListCacheKey(int siteId, int channelId, bool isAllContents) =>
        //    $"{nameof(ContentManager)}:{siteId}:{channelId}:{isAllContents}";

        //public static List<(int ChannelId, int ContentId)> GetChannelContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo)
        //{
        //    var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

        //    var cacheKey = GetListKey(tableName, channelInfo.Additional.IsAllContents, siteInfo.Id, channelInfo.Id);

        //    return Caching.CacheManager.GetOrCreate(cacheKey, () =>
        //    {
        //        return DataProvider.ContentDao.GetCacheChannelContentIdList(tableName, DataProvider.ContentDao.GetCacheWhereString(siteInfo, channelInfo, channelInfo.Additional.IsAllContents, string.Empty, string.Empty),
        //            DataProvider.ContentDao.GetOrderString(string.Empty, channelInfo.Additional.IsAllContents));
        //    });

        //    //var cacheKey = ListCacheKey(siteInfo.Id, channelInfo.Id, adminId, isAllContents);
        //    //var retVal = CacheUtils.Get<List<(int ChannelId, int ContentId)>>(cacheKey);
        //    //if (retVal != null) return retVal;

        //    //var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
        //    //retVal = DataProvider.ContentDao.GetCacheChannelContentIdList(tableName, DataProvider.ContentDao.GetCacheWhereString(siteInfo, channelInfo, adminId, isAllContents, string.Empty, string.Empty),
        //    //    DataProvider.ContentDao.GetOrderString(string.Empty, isAllContents));

        //    //CacheUtils.Insert(cacheKey, retVal);

        //    //return retVal;
        //}

        //public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked)
        //{
        //    return CountCache.GetChannelCountByIsChecked(siteInfo, channelInfo, isChecked);
        //}

        //public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo)
        //{
        //    var ccIds = GetChannelContentIdList(siteInfo, channelInfo);
        //    return ccIds.Count();
        //}
    }
}