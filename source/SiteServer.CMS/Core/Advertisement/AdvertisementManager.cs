using System.Collections;
using BaiRong.Core;

namespace SiteServer.CMS.Core.Advertisement
{
    public class AdvertisementManager
    {
        public static ArrayList[] GetAdvertisementArrayLists(int publishmentSystemID)
        {
            var cacheKey = GetCacheKey(publishmentSystemID);
            lock (lockObject)
            {
                if (CacheUtils.Get(cacheKey) == null)
                {
                    var arraylists = DataProvider.AdvertisementDao.GetAdvertisementArrayLists(publishmentSystemID);
                    CacheUtils.Insert(cacheKey, arraylists, 30);
                    return arraylists;
                }
                return CacheUtils.Get(cacheKey) as ArrayList[];
            }
        }

        public static void RemoveCache(int publishmentSystemID)
        {
            var cacheKey = GetCacheKey(publishmentSystemID);
            CacheUtils.Remove(cacheKey);
        }

        private static string GetCacheKey(int publishmentSystemID)
        {
            return cacheKeyPrefix + publishmentSystemID;
        }

        private static readonly object lockObject = new object();
        private const string cacheKeyPrefix = "SiteServer.CMS.Core.Advertisement.";
    }
}
