using System.Collections.Generic;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Comment
    {
        private static readonly object LockObject = new object();

        public static int GetCountChecked(int publishmentSystemId, int channelId, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Comment), nameof(GetCountChecked),
                       publishmentSystemId.ToString(), channelId.ToString(), contentId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.CommentDao.GetCountChecked(publishmentSystemId, channelId, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<int> GetContentIdListByCount(int publishmentSystemId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Comment), nameof(GetContentIdListByCount),
                    publishmentSystemId.ToString());
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.CommentDao.GetContentIdListByCount(publishmentSystemId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetSelectSqlStringWithChecked(int publishmentSystemId, int nodeId, int contentId,
            int startNum, int totalNum, bool isRecommend, string whereString, string orderByString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Comment),
                    nameof(GetSelectSqlStringWithChecked), publishmentSystemId.ToString(), nodeId.ToString(),
                    contentId.ToString(), startNum.ToString(), totalNum.ToString(), isRecommend.ToString(), whereString,
                    orderByString);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.CommentDao.GetSelectSqlStringWithChecked(publishmentSystemId, nodeId, contentId,
                       startNum, totalNum, isRecommend, whereString, orderByString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
