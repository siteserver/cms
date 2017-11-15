using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Advertisement
    {
        private static readonly object LockObject = new object();

        public static List<string> GetAdvertisementNameList(int publishmentSystemId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Advertisement),
                    nameof(GetAdvertisementNameList), publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<string>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<string>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.AdvertisementDao.GetAdvertisementNameList(publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static AdvertisementInfo GetAdvertisementInfo(string advertisementName, int publishmentSystemId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Advertisement), nameof(GetAdvertisementInfo),
                    advertisementName, publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<AdvertisementInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<AdvertisementInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.AdvertisementDao.GetAdvertisementInfo(advertisementName, publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

    }
}
