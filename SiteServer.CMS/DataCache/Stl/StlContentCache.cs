using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache.Stl
{
    public static class StlContentCache
    {
        private static readonly object LockObject = new object();

        public static void ClearCache()
        {
            StlCacheManager.Clear(nameof(StlContentCache));
        }

        public static List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetContentIdListChecked),
                    tableName, channelId.ToString(), orderByFormatString);
            var retVal = StlCacheManager.Get<List<int>>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<List<int>>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ContentDao.GetContentIdListChecked(tableName, channelId, orderByFormatString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static DataSet GetStlDataSourceChecked(List<int> channelIdList, string tableName, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlDataSourceChecked),
                    TranslateUtils.ObjectCollectionToString(channelIdList), tableName, startNum.ToString(), totalNum.ToString(), orderByString, whereString, TranslateUtils.NameValueCollectionToString(others));
            var retVal = StlCacheManager.Get<DataSet>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<DataSet>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ContentDao.GetStlDataSourceChecked(channelIdList, tableName, startNum, totalNum, orderByString, whereString, others);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetValue(string tableName, int contentId, string type)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetValue), tableName,
                       contentId.ToString(), type);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    var tuple = DataProvider.ContentDao.GetValue(tableName, contentId, type);
                    if (tuple != null)
                    {
                        retVal = tuple.Item2;
                        StlCacheManager.Set(cacheKey, retVal);
                    }
                }                
            }

            return retVal;
        }

        public static int GetSequence(string tableName, int channelId, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetSequence),
                    tableName, channelId.ToString(), contentId.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ContentDao.GetSequence(tableName, channelId, contentId);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static int GetCountCheckedImage(int siteId, int channelId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetCountCheckedImage),
                    siteId.ToString(), channelId.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ContentDao.GetCountCheckedImage(siteId, channelId);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static int GetCountOfContentAdd(string tableName, int siteId, int channelId, EScopeType scope,
            DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetCountOfContentAdd),
                    siteId.ToString(), channelId.ToString(), EScopeTypeUtils.GetValue(scope),
                    DateUtils.GetDateString(begin), DateUtils.GetDateString(end), userName, ETriStateUtils.GetValue(checkedState));
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ContentDao.GetCountOfContentAdd(tableName, siteId, channelId, scope,
                    begin, end, userName, checkedState);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static int GetContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetContentId), tableName,
                       channelId.ToString(), taxis.ToString(), isNextContent.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ContentDao.GetContentId(tableName, channelId, taxis, isNextContent);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static int GetContentId(string tableName, int channelId, bool isCheckedOnly, string orderByString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetContentId), tableName,
                    channelId.ToString(), orderByString);
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ContentDao.GetContentId(tableName, channelId, isCheckedOnly, orderByString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }        

        public static int GetChannelId(string tableName, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetChannelId), tableName,
                       contentId.ToString());
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DataProvider.ContentDao.GetChannelId(tableName, contentId);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlWhereString),
                    siteId.ToString(), group, groupNot,
                    tags, isImageExists.ToString(), isImage.ToString(), isVideoExists.ToString(), isVideo.ToString(),
                    isFileExists.ToString(), isFile.ToString(), isTopExists.ToString(), isTop.ToString(),
                    isRecommendExists.ToString(), isRecommend.ToString(), isHotExists.ToString(), isHot.ToString(),
                    isColorExists.ToString(), isColor.ToString(), where);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ContentDao.GetStlWhereString(siteId, group,
                    groupNot,
                    tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop,
                    isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isTopExists, bool isTop, string where)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlWhereString),
                    siteId.ToString(), group, groupNot, tags, isTopExists.ToString(), isTop.ToString(),
                    where);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ContentDao.GetStlWhereString(siteId, group, groupNot, tags,
                    isTopExists, isTop, where);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetStlSqlStringChecked(string tableName, int siteId, int channelId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlSqlStringChecked),
                    tableName, siteId.ToString(), channelId.ToString(), startNum.ToString(),
                    totalNum.ToString(), orderByString, whereString, EScopeTypeUtils.GetValue(scopeType), groupChannel,
                    groupChannelNot);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
                    retVal = DataProvider.ContentDao.GetStlSqlStringChecked(channelIdList, tableName, siteId, channelId, startNum,
                    totalNum, orderByString, whereString, scopeType, groupChannel, groupChannelNot);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetStlWhereStringBySearch(string group, string groupNot, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlWhereStringBySearch), group, groupNot, isImageExists.ToString(), isImage.ToString(),
                    isVideoExists.ToString(), isVideo.ToString(), isFileExists.ToString(), isFile.ToString(),
                    isTopExists.ToString(), isTop.ToString(), isRecommendExists.ToString(), isRecommend.ToString(),
                    isHotExists.ToString(), isHot.ToString(), isColorExists.ToString(), isColor.ToString(), where);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ContentDao.GetStlWhereStringBySearch(group, groupNot,
                    isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop,
                    isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache),
                       nameof(GetStlSqlStringCheckedBySearch),
                       tableName, startNum.ToString(), totalNum.ToString(), orderByString, whereString);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    retVal = DataProvider.ContentDao.GetStlSqlStringCheckedBySearch(tableName, startNum, totalNum,
                    orderByString, whereString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }
    }
}
