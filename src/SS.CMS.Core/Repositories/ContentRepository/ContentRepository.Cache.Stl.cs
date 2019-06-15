using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using SqlKata;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Models;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private readonly object StlLockObject = new object();

        public void StlClearCache()
        {
            StlCacheManager.Clear(nameof(ContentRepository));
        }

        public List<int> StlGetContentIdListChecked(ChannelInfo channelInfo, TaxisType taxisType)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(GetContentIdListChecked),
                    channelInfo.Id.ToString(), taxisType.Value);
            var retval = StlCacheManager.Get<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = GetContentIdListChecked(channelInfo.Id, taxisType);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public List<ContentInfo> StlGetStlDataSourceChecked(List<int> channelIdList, ChannelInfo channelInfo, int startNum, int totalNum, TaxisType taxisType, Query query, NameValueCollection others)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlDataSourceChecked),
                    TranslateUtils.ObjectCollectionToString(channelIdList), channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), taxisType.Value, query.ToString(), TranslateUtils.NameValueCollectionToString(others));
            var retval = StlCacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<List<ContentInfo>>(cacheKey);
                if (retval == null)
                {
                    retval = GetStlDataSourceChecked(channelIdList, startNum, totalNum, taxisType, query, others);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public List<KeyValuePair<int, ContentInfo>> StlGetContainerContentListChecked(List<int> channelIdList, ChannelInfo channelInfo, int startNum, int totalNum, string order, Query where, NameValueCollection others)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetContainerContentListChecked),
                    TranslateUtils.ObjectCollectionToString(channelIdList), channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), order, where.ToString(), TranslateUtils.NameValueCollectionToString(others));
            var retval = StlCacheManager.Get<List<KeyValuePair<int, ContentInfo>>>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<List<KeyValuePair<int, ContentInfo>>>(cacheKey);
                if (retval == null)
                {
                    retval = GetContainerContentListChecked(channelIdList, startNum, totalNum, order, where, others);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public List<KeyValuePair<int, ContentInfo>> StlGetContainerContentListBySqlString(ChannelInfo channelInfo, string sqlString, string order, int totalNum, int pageNum, int currentPageIndex)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetContainerContentListBySqlString), channelInfo.Id.ToString(),
                       sqlString, order, totalNum.ToString(), pageNum.ToString(), currentPageIndex.ToString());
            var retval = StlCacheManager.Get<List<KeyValuePair<int, ContentInfo>>>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<List<KeyValuePair<int, ContentInfo>>>(cacheKey);
                if (retval == null)
                {
                    retval = GetContainerContentListBySqlString(sqlString, order, totalNum, pageNum,
                    currentPageIndex);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public string StlGetValue(ChannelInfo channelInfo, int contentId, string type)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetValue), channelInfo.Id.ToString(), contentId.ToString(), type);
            var retval = StlCacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<string>(cacheKey);
                if (retval == null)
                {
                    retval = GetValue<string>(contentId, type);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public int StlGetSequence(ChannelInfo channelInfo, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetSequence),
                    channelInfo.Id.ToString(), contentId.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = GetSequence(channelInfo.Id, contentId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public int StlGetCountCheckedImage(int siteId, ChannelInfo channelInfo)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetCountCheckedImage),
                    siteId.ToString(), channelInfo.Id.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = GetCountCheckedImage(siteId, channelInfo.Id);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public int StlGetCountOfContentAdd(int siteId, ChannelInfo channelInfo, ScopeType scope,
            DateTime begin, DateTime end, string userName, bool? checkedState)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetCountOfContentAdd),
                    siteId.ToString(), channelInfo.Id.ToString(), scope.Value,
                    DateUtils.GetDateString(begin), DateUtils.GetDateString(end), userName, checkedState.HasValue ? checkedState.ToString() : string.Empty);
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = GetCountOfContentAdd(siteId, channelInfo.Id, scope,
                    begin, end, userName, checkedState);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public int StlGetContentId(ChannelInfo channelInfo, int taxis, bool isNextContent)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetContentId), channelInfo.Id.ToString(), taxis.ToString(), isNextContent.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = GetContentId(channelInfo.Id, taxis, isNextContent);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public int StlGetContentId(ChannelInfo channelInfo, TaxisType taxisType)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetContentId), channelInfo.Id.ToString(), taxisType.Value);
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = GetContentId(channelInfo.Id, taxisType);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public int StlGetChannelId(ChannelInfo channelInfo, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetChannelId), channelInfo.Id.ToString(), contentId.ToString());
            var retval = StlCacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = GetChannelId(contentId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public Query StlGetStlWhereString(int siteId, string group, string groupNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ChannelInfo channelInfo, bool isRelatedContents, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlWhereString),
                    siteId.ToString(), group, groupNot,
                    tags, isImage.ToString(), isVideo.ToString(), isFile.ToString(), isTop.ToString(), isRecommend.ToString(), isHot.ToString(), isColor.ToString(), channelInfo.Id.ToString(), isRelatedContents.ToString(), contentId.ToString());
            var retval = StlCacheManager.Get<Query>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<Query>(cacheKey);
                if (retval == null)
                {
                    retval = GetStlWhereString(siteId, channelInfo, group,
                    groupNot,
                    tags, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor, isRelatedContents, contentId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public Query StlGetStlWhereString(int siteId, string group, string groupNot, string tags, bool? isTop, ChannelInfo channelInfo, bool isRelatedContents, int contentId)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlWhereString),
                    siteId.ToString(), group, groupNot, tags, isTop.ToString(), channelInfo.Id.ToString(), isRelatedContents.ToString(), contentId.ToString());
            var retval = StlCacheManager.Get<Query>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<Query>(cacheKey);
                if (retval == null)
                {
                    retval = GetStlWhereString(siteId, channelInfo, group, groupNot, tags, isTop, isRelatedContents, contentId);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public List<ContentInfo> StlGetStlSqlStringChecked(int siteId, ChannelInfo channelInfo, int startNum, int totalNum, string order, Query query, ScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlSqlStringChecked), siteId.ToString(), channelInfo.Id.ToString(), startNum.ToString(),
                    totalNum.ToString(), order, query.ToString(), scopeType.Value, groupChannel,
                    groupChannelNot);
            var retval = StlCacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<List<ContentInfo>>(cacheKey);
                if (retval == null)
                {
                    var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
                    retval = GetStlSqlStringChecked(channelIdList, siteId, channelInfo.Id, startNum,
                    totalNum, order, query, scopeType, groupChannel, groupChannelNot);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public Query StlGetStlWhereStringBySearch(ChannelInfo channelInfo, string group, string groupNot, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlWhereStringBySearch), channelInfo.Id.ToString(), group, groupNot, isImage.ToString(),
                    isVideo.ToString(), isFile.ToString(),
                    isTop.ToString(), isRecommend.ToString(),
                    isHot.ToString(), isColor.ToString());
            var retval = StlCacheManager.Get<Query>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<Query>(cacheKey);
                if (retval == null)
                {
                    retval = GetStlWhereStringBySearch(group, groupNot, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public List<ContentInfo> StlGetStlSqlStringCheckedBySearch(ChannelInfo channelInfo, int startNum, int totalNum, string order, Query query)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(ContentRepository),
                       nameof(StlGetStlSqlStringCheckedBySearch),
                       channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), order, query.ToString());
            var retval = StlCacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval != null) return retval;

            lock (StlLockObject)
            {
                retval = StlCacheManager.Get<List<ContentInfo>>(cacheKey);
                if (retval == null)
                {
                    retval = GetStlSqlStringCheckedBySearch(startNum, totalNum, order, query);
                    StlCacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
