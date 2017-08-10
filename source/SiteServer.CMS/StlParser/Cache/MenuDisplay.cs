using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class MenuDisplay
    {
        private static readonly object LockObject = new object();

        public static int GetMenuDisplayIdByName(int publishmentSystemId, string styleName, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(MenuDisplay), nameof(GetMenuDisplayIdByName),
                    publishmentSystemId.ToString(), styleName);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.MenuDisplayDao.GetMenuDisplayIdByName(publishmentSystemId, styleName);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static MenuDisplayInfo GetDefaultMenuDisplayInfo(int publishmentSystemId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(MenuDisplay),
                    nameof(GetDefaultMenuDisplayInfo), publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<MenuDisplayInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<MenuDisplayInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.MenuDisplayDao.GetDefaultMenuDisplayInfo(publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static MenuDisplayInfo GetMenuDisplayInfo(int menuDisplayId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(MenuDisplay), nameof(GetMenuDisplayInfo),
                    menuDisplayId.ToString());
            var retval = StlCacheUtils.GetCache<MenuDisplayInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<MenuDisplayInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.MenuDisplayDao.GetMenuDisplayInfo(menuDisplayId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
