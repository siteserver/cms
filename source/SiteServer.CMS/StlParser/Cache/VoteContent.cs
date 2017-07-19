using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class VoteContent
    {
        public static VoteContentInfo GetContentInfo(string tableName, int contentId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(VoteContent), nameof(GetContentInfo), guid, tableName, contentId.ToString());
            var retval = Utils.GetCache<VoteContentInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.VoteContentDao.GetContentInfo(tableName, contentId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
