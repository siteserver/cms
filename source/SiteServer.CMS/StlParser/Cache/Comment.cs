using System.Collections.Generic;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Comment
    {
        public static int GetCountChecked(int publishmentSystemId, int channelId, int contentId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Comment), nameof(GetCountChecked), guid, publishmentSystemId.ToString(), channelId.ToString(), contentId.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.CommentDao.GetCountChecked(publishmentSystemId, channelId, contentId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static List<int> GetContentIdListByCount(int publishmentSystemId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Comment), nameof(GetContentIdListByCount), guid, publishmentSystemId.ToString());
            var retval = Utils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.CommentDao.GetContentIdListByCount(publishmentSystemId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static string GetSelectSqlStringWithChecked(int publishmentSystemId, int nodeId, int contentId, int startNum, int totalNum, bool isRecommend, string whereString, string orderByString, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Comment), nameof(GetSelectSqlStringWithChecked), guid, publishmentSystemId.ToString(), nodeId.ToString(), contentId.ToString(), startNum.ToString(), totalNum.ToString(), isRecommend.ToString(), whereString, orderByString);
            var retval = Utils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.CommentDao.GetSelectSqlStringWithChecked(publishmentSystemId, nodeId, contentId, startNum, totalNum, isRecommend, whereString, orderByString);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
