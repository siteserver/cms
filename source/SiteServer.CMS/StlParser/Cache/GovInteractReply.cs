using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class GovInteractReply
    {
        public static GovInteractReplyInfo GetReplyInfoByContentId(int publishmentSystemId, int contentId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(GovInteractReply), nameof(GetReplyInfoByContentId), publishmentSystemId.ToString(), contentId.ToString());
            var retval = StlCacheUtils.GetCache<GovInteractReplyInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(publishmentSystemId, contentId);
            StlCacheUtils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
