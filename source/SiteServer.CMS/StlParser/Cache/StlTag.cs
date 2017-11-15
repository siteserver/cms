using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class StlTag
    {
        private static readonly object LockObject = new object();

        public static StlTagInfo GetStlTagInfo(int publishmentSystemId, string tagName)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(StlTag), nameof(GetStlTagInfo),
                       publishmentSystemId.ToString(), tagName);
            var retval = StlCacheUtils.GetCache<StlTagInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<StlTagInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.StlTagDao.GetStlTagInfo(publishmentSystemId, tagName);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
