using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class GovInteractReply
    {
        public static GovInteractReplyInfo GetReplyInfoByContentId(int publishmentSystemId, int contentId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(GovInteractReply), nameof(GetReplyInfoByContentId), guid, publishmentSystemId.ToString(), contentId.ToString());
            var retval = Utils.GetCache<GovInteractReplyInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(publishmentSystemId, contentId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
