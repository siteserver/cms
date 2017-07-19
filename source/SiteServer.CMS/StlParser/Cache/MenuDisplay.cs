using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class MenuDisplay
    {
        public static int GetMenuDisplayIdByName(int publishmentSystemId, string styleName, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(MenuDisplay), nameof(GetMenuDisplayIdByName), guid, publishmentSystemId.ToString(), styleName);
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.MenuDisplayDao.GetMenuDisplayIdByName(publishmentSystemId, styleName);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static MenuDisplayInfo GetDefaultMenuDisplayInfo(int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(MenuDisplay), nameof(GetDefaultMenuDisplayInfo), guid, publishmentSystemId.ToString());
            var retval = Utils.GetCache<MenuDisplayInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.MenuDisplayDao.GetDefaultMenuDisplayInfo(publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static MenuDisplayInfo GetMenuDisplayInfo(int menuDisplayId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(MenuDisplay), nameof(GetMenuDisplayInfo), guid, menuDisplayId.ToString());
            var retval = Utils.GetCache<MenuDisplayInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.MenuDisplayDao.GetMenuDisplayInfo(menuDisplayId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
