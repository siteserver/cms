using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Photo
    {
        public static List<PhotoInfo> GetPhotoInfoList(int publishmentSystemId, int contentId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Photo), nameof(GetPhotoInfoList), guid, publishmentSystemId.ToString(), contentId.ToString());
            var retval = Utils.GetCache<List<PhotoInfo>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.PhotoDao.GetPhotoInfoList(publishmentSystemId, contentId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static PhotoInfo GetFirstPhotoInfo(int publishmentSystemId, int contentId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Photo), nameof(GetFirstPhotoInfo), guid, publishmentSystemId.ToString(), contentId.ToString());
            var retval = Utils.GetCache<PhotoInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.PhotoDao.GetFirstPhotoInfo(publishmentSystemId, contentId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
