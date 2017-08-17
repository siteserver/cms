using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class VoteContent
    {
        private static readonly object LockObject = new object();

        public static VoteContentInfo GetContentInfo(string tableName, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(VoteContent), nameof(GetContentInfo),
                       tableName, contentId.ToString());
            var retval = StlCacheUtils.GetCache<VoteContentInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<VoteContentInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.VoteContentDao.GetContentInfo(tableName, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
