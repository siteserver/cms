using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class GovInteractChannel
    {
        public static int GetNodeIdByInteractName(int publishmentSystemId, string interactName, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(GovInteractChannel), nameof(GetNodeIdByInteractName), guid, publishmentSystemId.ToString(), interactName);
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.GovInteractChannelDao.GetNodeIdByInteractName(publishmentSystemId, interactName);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetApplyStyleId(int publishmentSystemId, int nodeId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(GovInteractChannel), nameof(GetApplyStyleId), guid, publishmentSystemId.ToString(), nodeId.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.GovInteractChannelDao.GetApplyStyleId(publishmentSystemId, nodeId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetQueryStyleId(int publishmentSystemId, int nodeId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(GovInteractChannel), nameof(GetQueryStyleId), guid, publishmentSystemId.ToString(), nodeId.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.GovInteractChannelDao.GetQueryStyleId(publishmentSystemId, nodeId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        
    }
}
