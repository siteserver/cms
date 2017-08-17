using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class InputContent
    {
        private static readonly object LockObject = new object();

        public static InputContentInfo GetContentInfo(int id)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(InputContent), nameof(GetContentInfo),
                    id.ToString());
            var retval = StlCacheUtils.GetCache<InputContentInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<InputContentInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.InputContentDao.GetContentInfo(id);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetSelectSqlStringWithChecked(int publishmentSystemId, int inputId, bool isReplyExists, bool isReply, int startNum, int totalNum, string whereString, string orderByString, LowerNameValueCollection others)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(InputContent),
                    nameof(GetSelectSqlStringWithChecked), publishmentSystemId.ToString(), inputId.ToString(),
                    isReplyExists.ToString(), isReply.ToString(), startNum.ToString(), totalNum.ToString(), whereString,
                    orderByString, TranslateUtils.NameValueCollectionToString(others));
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.InputContentDao.GetSelectSqlStringWithChecked(publishmentSystemId, inputId,
                    isReplyExists, isReply, startNum, totalNum, whereString, orderByString, others);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
