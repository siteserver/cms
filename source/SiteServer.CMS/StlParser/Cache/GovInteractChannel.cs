using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class GovInteractChannel
    {
        public static int GetNodeIdByInteractName(int publishmentSystemId, string interactName, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(GovInteractChannel), nameof(GetNodeIdByInteractName), publishmentSystemId.ToString(), interactName);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.GovInteractChannelDao.GetNodeIdByInteractName(publishmentSystemId, interactName);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetApplyStyleId(int publishmentSystemId, int nodeId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(GovInteractChannel), nameof(GetApplyStyleId), publishmentSystemId.ToString(), nodeId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.GovInteractChannelDao.GetApplyStyleId(publishmentSystemId, nodeId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetQueryStyleId(int publishmentSystemId, int nodeId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(GovInteractChannel), nameof(GetQueryStyleId), publishmentSystemId.ToString(), nodeId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.GovInteractChannelDao.GetQueryStyleId(publishmentSystemId, nodeId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }

        
    }
}
