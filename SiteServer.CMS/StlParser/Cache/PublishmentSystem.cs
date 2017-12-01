using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class PublishmentSystem
    {
        private static readonly object LockObject = new object();

        public static int GetPublishmentSystemIdByIsHeadquarters()
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(PublishmentSystem),
                       nameof(GetPublishmentSystemIdByIsHeadquarters));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetPublishmentSystemIdByPublishmentSystemDir(string publishmentSystemDir)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(PublishmentSystem),
                       nameof(GetPublishmentSystemIdByPublishmentSystemDir), publishmentSystemDir);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval =
                        DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByPublishmentSystemDir(
                            publishmentSystemDir);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        
    }
}
