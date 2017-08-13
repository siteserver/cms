using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class SeoMeta
    {
        private static readonly object LockObject = new object();

        public static int GetSeoMetaIdByNodeId(int nodeId, bool isChannel, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(SeoMeta), nameof(GetSeoMetaIdByNodeId),
                       nodeId.ToString(), isChannel.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.SeoMetaDao.GetSeoMetaIdByNodeId(nodeId, isChannel);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetDefaultSeoMetaId(int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(SeoMeta), nameof(GetDefaultSeoMetaId),
                       publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.SeoMetaDao.GetDefaultSeoMetaId(publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }    
            }

            return retval;
        }

        public static SeoMetaInfo GetSeoMetaInfo(int seoMetaId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(SeoMeta), nameof(GetSeoMetaInfo),
                       seoMetaId.ToString());
            var retval = StlCacheUtils.GetCache<SeoMetaInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<SeoMetaInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.SeoMetaDao.GetSeoMetaInfo(seoMetaId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
