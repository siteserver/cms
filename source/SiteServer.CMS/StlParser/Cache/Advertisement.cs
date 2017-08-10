using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Advertisement
    {
        public static List<string> GetAdvertisementNameList(int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Advertisement), nameof(GetAdvertisementNameList), publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<string>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.AdvertisementDao.GetAdvertisementNameList(publishmentSystemId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static AdvertisementInfo GetAdvertisementInfo(string advertisementName, int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Advertisement), nameof(GetAdvertisementInfo), advertisementName, publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<AdvertisementInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.AdvertisementDao.GetAdvertisementInfo(advertisementName, publishmentSystemId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        
    }
}
