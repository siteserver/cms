using BaiRong.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class User
    {
        private static readonly object LockObject = new object();

        public static string GetDisplayName(string userName, string guid)
        {
            lock (LockObject)
            {
                var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(User), nameof(GetDisplayName), userName);
                var retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval != null) return retval;

                retval = BaiRongDataProvider.UserDao.GetDisplayName(userName);
                StlCacheUtils.SetCache(cacheKey, retval);
                return retval;
            }
        }
    }
}
