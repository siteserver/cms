using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Content
    {
        private static readonly object LockObject = new object();

        public static void ClearCache()
        {
            StlCacheUtils.ClearCache(nameof(Content));
        }

        public static List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetContentIdListChecked),
                    tableName, channelId.ToString(), orderByFormatString);
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetContentIdListChecked(tableName, channelId, orderByFormatString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSourceChecked(List<int> channelIdList, string tableName, int startNum, int totalNum, string orderByString, string whereString, LowerNameValueCollection others)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlDataSourceChecked),
                    TranslateUtils.ObjectCollectionToString(channelIdList), tableName, startNum.ToString(), totalNum.ToString(), orderByString, whereString, TranslateUtils.NameValueCollectionToString(others));
            var retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetStlDataSourceChecked(channelIdList, tableName, startNum, totalNum, orderByString, whereString, others);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static ContentInfo GetContentInfo(string tableName, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetContentInfo), tableName, contentId.ToString());
            var retval = StlCacheUtils.GetCache<ContentInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<ContentInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static ContentInfo GetContentInfo(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var tableName = ChannelManager.GetTableName(siteInfo, channelId);

            return GetContentInfo(tableName, contentId);
        }

        public static string GetValue(string tableName, int contentId, string type)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetValue), tableName,
                       contentId.ToString(), type);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetValue(tableName, contentId, type);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }                
            }

            return retval;
        }

        public static int GetSequence(string tableName, int channelId, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetSequence),
                    tableName, channelId.ToString(), contentId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ContentDao.GetSequence(tableName, channelId, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetCountCheckedImage(int siteId, int channelId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetCountCheckedImage),
                    siteId.ToString(), channelId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ContentDao.GetCountCheckedImage(siteId, channelId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetCountOfContentAdd(string tableName, int siteId, int channelId, EScopeType scope,
            DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetCountOfContentAdd),
                    siteId.ToString(), channelId.ToString(), EScopeTypeUtils.GetValue(scope),
                    DateUtils.GetDateString(begin), DateUtils.GetDateString(end), userName, ETriStateUtils.GetValue(checkedState));
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ContentDao.GetCountOfContentAdd(tableName, siteId, channelId, scope,
                    begin, end, userName, checkedState);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DateTime GetAddDate(string tableName, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetAddDate), tableName,
                       contentId.ToString());
            var retval = StlCacheUtils.GetDateTimeCache(cacheKey);
            if (retval != DateTime.MinValue) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetDateTimeCache(cacheKey);
                if (retval == DateTime.MinValue)
                {
                    retval = DataProvider.ContentDao.GetAddDate(tableName, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DateTime GetLastEditDate(string tableName, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetLastEditDate), tableName,
                       contentId.ToString());
            var retval = StlCacheUtils.GetDateTimeCache(cacheKey);
            if (retval != DateTime.MinValue) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetDateTimeCache(cacheKey);
                if (retval == DateTime.MinValue)
                {
                    retval = DataProvider.ContentDao.GetLastEditDate(tableName, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetContentId), tableName,
                       channelId.ToString(), taxis.ToString(), isNextContent.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ContentDao.GetContentId(tableName, channelId, taxis, isNextContent);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetContentId(string tableName, int channelId, string orderByString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetContentId), tableName,
                    channelId.ToString(), orderByString);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ContentDao.GetContentId(tableName, channelId, orderByString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }        

        public static int GetChannelId(string tableName, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetChannelId), tableName,
                       contentId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ContentDao.GetChannelId(tableName, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlWhereString),
                    siteId.ToString(), group, groupNot,
                    tags, isImageExists.ToString(), isImage.ToString(), isVideoExists.ToString(), isVideo.ToString(),
                    isFileExists.ToString(), isFile.ToString(), isTopExists.ToString(), isTop.ToString(),
                    isRecommendExists.ToString(), isRecommend.ToString(), isHotExists.ToString(), isHot.ToString(),
                    isColorExists.ToString(), isColor.ToString(), where);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetStlWhereString(siteId, group,
                    groupNot,
                    tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop,
                    isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isTopExists, bool isTop, string where)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlWhereString),
                    siteId.ToString(), group, groupNot, tags, isTopExists.ToString(), isTop.ToString(),
                    where);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetStlWhereString(siteId, group, groupNot, tags,
                    isTopExists, isTop, where);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlSqlStringChecked(string tableName, int siteId, int channelId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlSqlStringChecked),
                    tableName, siteId.ToString(), channelId.ToString(), startNum.ToString(),
                    totalNum.ToString(), orderByString, whereString, EScopeTypeUtils.GetValue(scopeType), groupChannel,
                    groupChannelNot);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
                    retval = DataProvider.ContentDao.GetStlSqlStringChecked(channelIdList, tableName, siteId, channelId, startNum,
                    totalNum, orderByString, whereString, scopeType, groupChannel, groupChannelNot);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereStringBySearch(string group, string groupNot, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlWhereStringBySearch), group, groupNot, isImageExists.ToString(), isImage.ToString(),
                    isVideoExists.ToString(), isVideo.ToString(), isFileExists.ToString(), isFile.ToString(),
                    isTopExists.ToString(), isTop.ToString(), isRecommendExists.ToString(), isRecommend.ToString(),
                    isHotExists.ToString(), isHot.ToString(), isColorExists.ToString(), isColor.ToString(), where);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetStlWhereStringBySearch(group, groupNot,
                    isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop,
                    isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content),
                       nameof(GetStlSqlStringCheckedBySearch),
                       tableName, startNum.ToString(), totalNum.ToString(), orderByString, whereString);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetStlSqlStringCheckedBySearch(tableName, startNum, totalNum,
                    orderByString, whereString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
