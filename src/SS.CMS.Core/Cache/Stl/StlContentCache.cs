using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Models;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Cache.Stl
{
    public static class StlContentCache
    {
        private static readonly object LockObject = new object();

        public static void ClearCache()
        {
            StlCacheManager.Clear(nameof(StlContentCache));
        }

        public static List<int> GetContentIdListChecked(ChannelInfo channelInfo, string orderByFormatString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetContentIdListChecked),
                    channelInfo.Id.ToString(), orderByFormatString);
            var retval = StlCacheManager.Get<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetContentIdListChecked(channelInfo.Id, orderByFormatString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSourceChecked(List<int> channelIdList, ChannelInfo channelInfo, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlDataSourceChecked),
                    TranslateUtils.ObjectCollectionToString(channelIdList), channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), orderByString, whereString, TranslateUtils.NameValueCollectionToString(others));
            var retval = StlCacheManager.Get<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetStlDataSourceChecked(channelIdList, startNum, totalNum, orderByString, whereString, others);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<Container.Content> GetContainerContentListChecked(List<int> channelIdList, ChannelInfo channelInfo, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetContainerContentListChecked),
                    TranslateUtils.ObjectCollectionToString(channelIdList), channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), orderByString, whereString, TranslateUtils.NameValueCollectionToString(others));
            var retval = StlCacheManager.Get<List<Container.Content>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<List<Container.Content>>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetContainerContentListChecked(channelIdList, startNum, totalNum, orderByString, whereString, others);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<Container.Content> GetContainerContentListBySqlString(ChannelInfo channelInfo, string sqlString, string orderByString, int totalNum, int pageNum, int currentPageIndex)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetContainerContentListBySqlString), channelInfo.Id.ToString(),
                       sqlString, orderByString, totalNum.ToString(), pageNum.ToString(), currentPageIndex.ToString());
            var retval = StlCacheManager.Get<List<Container.Content>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<List<Container.Content>>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetContainerContentListBySqlString(sqlString, orderByString, totalNum, pageNum,
                    currentPageIndex);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetValue(ChannelInfo channelInfo, int contentId, string type)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetValue), channelInfo.Id.ToString(), contentId.ToString(), type);
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetValue<string>(contentId, type);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetSequence(ChannelInfo channelInfo, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetSequence),
                    channelInfo.Id.ToString(), contentId.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = channelInfo.ContentDao.GetSequence(channelInfo.Id, contentId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetCountCheckedImage(int siteId, ChannelInfo channelInfo)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetCountCheckedImage),
                    siteId.ToString(), channelInfo.Id.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = channelInfo.ContentDao.GetCountCheckedImage(siteId, channelInfo.Id);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetCountOfContentAdd(int siteId, ChannelInfo channelInfo, EScopeType scope,
            DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetCountOfContentAdd),
                    siteId.ToString(), channelInfo.Id.ToString(), EScopeTypeUtils.GetValue(scope),
                    DateUtils.GetDateString(begin), DateUtils.GetDateString(end), userName, ETriStateUtils.GetValue(checkedState));
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = channelInfo.ContentDao.GetCountOfContentAdd(siteId, channelInfo.Id, scope,
                    begin, end, userName, checkedState);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetContentId(ChannelInfo channelInfo, int taxis, bool isNextContent)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetContentId), channelInfo.Id.ToString(), taxis.ToString(), isNextContent.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = channelInfo.ContentDao.GetContentId(channelInfo.Id, taxis, isNextContent);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetContentId(ChannelInfo channelInfo, string orderByString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetContentId), channelInfo.Id.ToString(), orderByString);
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = channelInfo.ContentDao.GetContentId(channelInfo.Id, orderByString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetChannelId(ChannelInfo channelInfo, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetChannelId), channelInfo.Id.ToString(), contentId.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = channelInfo.ContentDao.GetChannelId(contentId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, ChannelInfo channelInfo, bool isRelatedContents, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlWhereString),
                    siteId.ToString(), group, groupNot,
                    tags, isImageExists.ToString(), isImage.ToString(), isVideoExists.ToString(), isVideo.ToString(),
                    isFileExists.ToString(), isFile.ToString(), isTopExists.ToString(), isTop.ToString(),
                    isRecommendExists.ToString(), isRecommend.ToString(), isHotExists.ToString(), isHot.ToString(),
                    isColorExists.ToString(), isColor.ToString(), channelInfo.Id.ToString(), isRelatedContents.ToString(), contentId.ToString());
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetStlWhereString(siteId, channelInfo, group,
                    groupNot,
                    tags, isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop,
                    isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, isRelatedContents, contentId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isTopExists, bool isTop, ChannelInfo channelInfo, bool isRelatedContents, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlWhereString),
                    siteId.ToString(), group, groupNot, tags, isTopExists.ToString(), isTop.ToString(), channelInfo.Id.ToString(), isRelatedContents.ToString(), contentId.ToString());
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetStlWhereString(siteId, channelInfo, group, groupNot, tags,
                    isTopExists, isTop, isRelatedContents, contentId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlSqlStringChecked(int siteId, ChannelInfo channelInfo, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlSqlStringChecked), siteId.ToString(), channelInfo.Id.ToString(), startNum.ToString(),
                    totalNum.ToString(), orderByString, whereString, EScopeTypeUtils.GetValue(scopeType), groupChannel,
                    groupChannelNot);
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
                    retval = channelInfo.ContentDao.GetStlSqlStringChecked(channelIdList, siteId, channelInfo.Id, startNum,
                    totalNum, orderByString, whereString, scopeType, groupChannel, groupChannelNot);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlWhereStringBySearch(ChannelInfo channelInfo, string group, string groupNot, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache), nameof(GetStlWhereStringBySearch), channelInfo.Id.ToString(), group, groupNot, isImageExists.ToString(), isImage.ToString(),
                    isVideoExists.ToString(), isVideo.ToString(), isFileExists.ToString(), isFile.ToString(),
                    isTopExists.ToString(), isTop.ToString(), isRecommendExists.ToString(), isRecommend.ToString(),
                    isHotExists.ToString(), isHot.ToString(), isColorExists.ToString(), isColor.ToString(), where);
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetStlWhereStringBySearch(group, groupNot,
                    isImageExists, isImage, isVideoExists, isVideo, isFileExists, isFile, isTopExists, isTop,
                    isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetStlSqlStringCheckedBySearch(ChannelInfo channelInfo, int startNum, int totalNum, string orderByString, string whereString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlContentCache),
                       nameof(GetStlSqlStringCheckedBySearch),
                       channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), orderByString, whereString);
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = channelInfo.ContentDao.GetStlSqlStringCheckedBySearch(startNum, totalNum,
                    orderByString, whereString);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
