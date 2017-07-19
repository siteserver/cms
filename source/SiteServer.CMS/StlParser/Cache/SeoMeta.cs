using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class SeoMeta
    {
        public static int GetSeoMetaIdByNodeId(int nodeId, bool isChannel, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(SeoMeta), nameof(GetSeoMetaIdByNodeId), guid, nodeId.ToString(), isChannel.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.SeoMetaDao.GetSeoMetaIdByNodeId(nodeId, isChannel);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetDefaultSeoMetaId(int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(SeoMeta), nameof(GetDefaultSeoMetaId), guid, publishmentSystemId.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.SeoMetaDao.GetDefaultSeoMetaId(publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static SeoMetaInfo GetSeoMetaInfo(int seoMetaId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(SeoMeta), nameof(GetSeoMetaInfo), guid, seoMetaId.ToString());
            var retval = Utils.GetCache<SeoMetaInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.SeoMetaDao.GetSeoMetaInfo(seoMetaId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        
    }
}
