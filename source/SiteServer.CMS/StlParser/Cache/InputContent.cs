using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class InputContent
    {
        public static InputContentInfo GetContentInfo(int id, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(InputContent), nameof(GetContentInfo), guid, id.ToString());
            var retval = Utils.GetCache<InputContentInfo>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.InputContentDao.GetContentInfo(id);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static string GetSelectSqlStringWithChecked(int publishmentSystemId, int inputId, bool isReplyExists, bool isReply, int startNum, int totalNum, string whereString, string orderByString, LowerNameValueCollection others, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(InputContent), nameof(GetSelectSqlStringWithChecked), guid, publishmentSystemId.ToString(), inputId.ToString(), isReplyExists.ToString(), isReply.ToString(), startNum.ToString(), totalNum.ToString(), whereString, orderByString, TranslateUtils.NameValueCollectionToString(others));
            var retval = Utils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.InputContentDao.GetSelectSqlStringWithChecked(publishmentSystemId, inputId, isReplyExists, isReply, startNum, totalNum, whereString, orderByString, others);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        
    }
}
