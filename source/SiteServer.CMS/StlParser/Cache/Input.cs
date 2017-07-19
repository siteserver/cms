using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Input
    {
        public static InputInfo GetInputInfo(int inputId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Input), nameof(GetInputInfo), guid, inputId.ToString());
            var retval = Utils.GetCache<InputInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.InputDao.GetInputInfo(inputId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetInputIdAsPossible(string inputName, int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Input), nameof(GetInputIdAsPossible), guid, inputName, publishmentSystemId.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.InputDao.GetInputIdAsPossible(inputName, publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
