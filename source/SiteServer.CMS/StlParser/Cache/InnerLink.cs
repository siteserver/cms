using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class InnerLink
    {
        public static List<InnerLinkInfo> GetInnerLinkInfoList(int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(InnerLink), nameof(GetInnerLinkInfoList), publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<InnerLinkInfo>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.InnerLinkDao.GetInnerLinkInfoList(publishmentSystemId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
