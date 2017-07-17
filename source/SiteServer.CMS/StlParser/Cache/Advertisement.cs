using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Advertisement
    {
        public static List<string> GetAdvertisementNameList(int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Advertisement), nameof(GetAdvertisementNameList), guid, publishmentSystemId.ToString());
            var retval = Utils.GetCache<List<string>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.AdvertisementDao.GetAdvertisementNameList(publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static AdvertisementInfo GetAdvertisementInfo(string advertisementName, int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Advertisement), nameof(GetAdvertisementInfo), guid, advertisementName, publishmentSystemId.ToString());
            var retval = Utils.GetCache<AdvertisementInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.AdvertisementDao.GetAdvertisementInfo(advertisementName, publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        
    }
}
