using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class GovInteractReply
    {
        private static readonly object LockObject = new object();

        public static GovInteractReplyInfo GetReplyInfoByContentId(int publishmentSystemId, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(GovInteractReply),
                    nameof(GetReplyInfoByContentId), publishmentSystemId.ToString(), contentId.ToString());
            var retval = StlCacheUtils.GetCache<GovInteractReplyInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<GovInteractReplyInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.GovInteractReplyDao.GetReplyInfoByContentId(publishmentSystemId, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
