using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class StarSetting
    {
        private static readonly object LockObject = new object();

        public static object[] GetTotalCountAndPointAverage(int publishmentSystemId, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(StarSetting),
                       nameof(GetTotalCountAndPointAverage), publishmentSystemId.ToString(), contentId.ToString());
            var retval = StlCacheUtils.GetCache<object[]>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<object[]>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.StarSettingDao.GetTotalCountAndPointAverage(publishmentSystemId, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
