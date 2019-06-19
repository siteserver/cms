using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository
    {
        public int GetSiteIdByIsRoot()
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(SiteRepository),
                       nameof(GetSiteIdByIsRoot));
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval = GetIdByIsRoot();
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public int GetSiteIdBySiteDir(string siteDir)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(SiteRepository),
                       nameof(GetSiteIdBySiteDir), siteDir);
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval =
                    GetIdBySiteDir(
                        siteDir);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }
    }
}
