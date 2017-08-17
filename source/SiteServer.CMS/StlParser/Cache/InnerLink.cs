using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class InnerLink
    {
        private static readonly object LockObject = new object();

        public static List<InnerLinkInfo> GetInnerLinkInfoList(int publishmentSystemId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(InnerLink), nameof(GetInnerLinkInfoList),
                       publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<InnerLinkInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<InnerLinkInfo>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.InnerLinkDao.GetInnerLinkInfoList(publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
