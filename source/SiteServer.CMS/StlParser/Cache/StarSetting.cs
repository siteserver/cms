using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class StarSetting
    {
        public static object[] GetTotalCountAndPointAverage(int publishmentSystemId, int contentId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(StarSetting), nameof(GetTotalCountAndPointAverage), guid, publishmentSystemId.ToString(), contentId.ToString());
            var retval = Utils.GetCache<object[]>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.StarSettingDao.GetTotalCountAndPointAverage(publishmentSystemId, contentId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        
        
    }
}
