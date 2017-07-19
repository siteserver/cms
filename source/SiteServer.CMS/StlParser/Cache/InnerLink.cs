using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class InnerLink
    {
        public static List<InnerLinkInfo> GetInnerLinkInfoList(int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(InnerLink), nameof(GetInnerLinkInfoList), guid, publishmentSystemId.ToString());
            var retval = Utils.GetCache<List<InnerLinkInfo>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.InnerLinkDao.GetInnerLinkInfoList(publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
