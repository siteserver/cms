using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Content
    {
        private static readonly object LockObject = new object();

        public static void ClearCache()
        {
            StlCacheUtils.ClearCache(nameof(Content));
        }

        public static List<int> GetContentIdListChecked(string tableName, int nodeId, string orderByFormatString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetContentIdListChecked),
                    tableName, nodeId.ToString(), orderByFormatString);
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetContentIdListChecked(tableName, nodeId, orderByFormatString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSourceChecked(List<int> nodeIdList, ETableStyle tableStyle, string tableName, int startNum, int totalNum, string orderByString, string whereString, bool isNoDup, LowerNameValueCollection others)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlDataSourceChecked),
                    TranslateUtils.ObjectCollectionToString(nodeIdList), tableName, startNum.ToString(), totalNum.ToString(), orderByString, whereString, isNoDup.ToString(), TranslateUtils.NameValueCollectionToString(others));
            var retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetStlDataSourceChecked(nodeIdList, tableStyle, tableName, startNum, totalNum, orderByString, whereString, isNoDup, others);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static ContentInfo GetContentInfo(ETableStyle tableStyle, string tableName, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetContentInfo),
                    ETableStyleUtils.GetValue(tableStyle), tableName, contentId.ToString());
            var retval = StlCacheUtils.GetCache<ContentInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<ContentInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static ContentInfo GetContentInfo(int publishmentSystemId, int channelId, int contentId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return GetContentInfo(tableStyle, tableName, contentId);
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
                    retval = BaiRongDataProvider.ContentDao.GetValue(tableName, contentId, type);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }                
            }

            return retval;
        }

        public static int GetSequence(string tableName, int nodeId, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetSequence),
                    tableName, nodeId.ToString(), contentId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = BaiRongDataProvider.ContentDao.GetSequence(tableName, nodeId, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetCountCheckedImage(int publishmentSystemId, int nodeId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetCountCheckedImage),
                    publishmentSystemId.ToString(), nodeId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.BackgroundContentDao.GetCountCheckedImage(publishmentSystemId, nodeId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetCountOfContentAdd(string tableName, int publishmentSystemId, int nodeId, EScopeType scope,
            DateTime begin, DateTime end, string userName)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetCountOfContentAdd),
                    publishmentSystemId.ToString(), nodeId.ToString(), EScopeTypeUtils.GetValue(scope),
                    DateUtils.GetDateString(begin), DateUtils.GetDateString(end), userName);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.ContentDao.GetCountOfContentAdd(tableName, publishmentSystemId, nodeId, scope,
                    begin, end, userName);
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
                    retval = BaiRongDataProvider.ContentDao.GetAddDate(tableName, contentId);
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
                    retval = BaiRongDataProvider.ContentDao.GetLastEditDate(tableName, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetContentId(string tableName, int nodeId, int taxis, bool isNextContent)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetContentId), tableName,
                       nodeId.ToString(), taxis.ToString(), isNextContent.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = BaiRongDataProvider.ContentDao.GetContentId(tableName, nodeId, taxis, isNextContent);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetContentId(string tableName, int nodeId, string orderByString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetContentId), tableName,
                    nodeId.ToString(), orderByString);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = BaiRongDataProvider.ContentDao.GetContentId(tableName, nodeId, orderByString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }        

        public static int GetNodeId(string tableName, int contentId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetNodeId), tableName,
                       contentId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = BaiRongDataProvider.ContentDao.GetNodeId(tableName, contentId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereString(int publishmentSystemId, string tableName, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, bool isCreateSearchDuplicate)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlWhereString),
                    publishmentSystemId.ToString(), tableName, group, groupNot,
                    tags, isImageExists.ToString(), isImage.ToString(), isVideoExists.ToString(), isVideo.ToString(),
                    isFileExists.ToString(), isFile.ToString(), isTopExists.ToString(), isTop.ToString(),
                    isRecommendExists.ToString(), isRecommend.ToString(), isHotExists.ToString(), isHot.ToString(),
                    isColorExists.ToString(), isColor.ToString(), where,
                    isCreateSearchDuplicate.ToString());
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.BackgroundContentDao.GetStlWhereString(publishmentSystemId, tableName, group,
                    groupNot,
                    tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop,
                    isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where,
                    isCreateSearchDuplicate);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereString(int publishmentSystemId, string group, string groupNot, string tags, bool isTopExists, bool isTop, string where)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlWhereString),
                    publishmentSystemId.ToString(), group, groupNot, tags, isTopExists.ToString(), isTop.ToString(),
                    where);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = BaiRongDataProvider.ContentDao.GetStlWhereString(publishmentSystemId, group, groupNot, tags,
                    isTopExists, isTop, where);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlSqlStringChecked(string tableName, int publishmentSystemId, int nodeId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isNoDup)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlSqlStringChecked),
                    tableName, publishmentSystemId.ToString(), nodeId.ToString(), startNum.ToString(),
                    totalNum.ToString(), orderByString, whereString, EScopeTypeUtils.GetValue(scopeType), groupChannel,
                    groupChannelNot, isNoDup.ToString());
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    var nodeIdList = Node.GetNodeIdListByScopeType(nodeId, scopeType, groupChannel, groupChannelNot);
                    retval = DataProvider.ContentDao.GetStlSqlStringChecked(nodeIdList, tableName, publishmentSystemId, nodeId, startNum,
                    totalNum, orderByString, whereString, scopeType, groupChannel, groupChannelNot, isNoDup);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereStringBySearch(string tableName, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content), nameof(GetStlWhereStringBySearch),
                    tableName, group, groupNot, tags, isImageExists.ToString(), isImage.ToString(),
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
                    retval = DataProvider.BackgroundContentDao.GetStlWhereStringBySearch(tableName, group, groupNot, tags,
                    isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop,
                    isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString, bool isNoDup)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Content),
                       nameof(GetStlSqlStringCheckedBySearch),
                       tableName, startNum.ToString(), totalNum.ToString(), orderByString, whereString, isNoDup.ToString());
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.ContentDao.GetStlSqlStringCheckedBySearch(tableName, startNum, totalNum,
                    orderByString, whereString, isNoDup);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
