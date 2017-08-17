using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class GovInteractChannel
    {
        private static readonly object LockObject = new object();

        public static int GetNodeIdByInteractName(int publishmentSystemId, string interactName)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(GovInteractChannel),
                       nameof(GetNodeIdByInteractName), publishmentSystemId.ToString(), interactName);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.GovInteractChannelDao.GetNodeIdByInteractName(publishmentSystemId, interactName);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetApplyStyleId(int publishmentSystemId, int nodeId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(GovInteractChannel), nameof(GetApplyStyleId),
                    publishmentSystemId.ToString(), nodeId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.GovInteractChannelDao.GetApplyStyleId(publishmentSystemId, nodeId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetQueryStyleId(int publishmentSystemId, int nodeId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(GovInteractChannel), nameof(GetQueryStyleId),
                    publishmentSystemId.ToString(), nodeId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.GovInteractChannelDao.GetQueryStyleId(publishmentSystemId, nodeId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
