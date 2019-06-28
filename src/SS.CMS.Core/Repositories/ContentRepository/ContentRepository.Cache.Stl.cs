using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public void StlClearCache()
        {
            _cacheManager.Clear(nameof(ContentRepository));
        }

        public List<int> StlGetContentIdListChecked(ChannelInfo channelInfo, TaxisType taxisType)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(GetContentIdListChecked),
                    channelInfo.Id.ToString(), taxisType.Value);
            var retval = _cacheManager.Get<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<List<int>>(cacheKey);
            if (retval == null)
            {
                retval = GetContentIdListChecked(channelInfo.Id, taxisType);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public List<ContentInfo> StlGetStlDataSourceChecked(List<int> channelIdList, ChannelInfo channelInfo, int startNum, int totalNum, TaxisType taxisType, Query query, NameValueCollection others)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlDataSourceChecked),
                    TranslateUtils.ObjectCollectionToString(channelIdList), channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), taxisType.Value, query.ToString(), TranslateUtils.NameValueCollectionToString(others));
            var retval = _cacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval == null)
            {
                retval = GetStlDataSourceChecked(channelIdList, startNum, totalNum, taxisType, query, others);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public List<KeyValuePair<int, ContentInfo>> StlGetContainerContentListChecked(List<int> channelIdList, ChannelInfo channelInfo, int startNum, int totalNum, string order, Query where, NameValueCollection others)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetContainerContentListChecked),
                    TranslateUtils.ObjectCollectionToString(channelIdList), channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), order, where.ToString(), TranslateUtils.NameValueCollectionToString(others));
            var retval = _cacheManager.Get<List<KeyValuePair<int, ContentInfo>>>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<List<KeyValuePair<int, ContentInfo>>>(cacheKey);
            if (retval == null)
            {
                retval = GetContainerContentListChecked(channelIdList, startNum, totalNum, order, where, others);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public List<KeyValuePair<int, ContentInfo>> StlGetContainerContentListBySqlString(ChannelInfo channelInfo, string sqlString, string order, int totalNum, int pageNum, int currentPageIndex)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetContainerContentListBySqlString), channelInfo.Id.ToString(),
                       sqlString, order, totalNum.ToString(), pageNum.ToString(), currentPageIndex.ToString());
            var retval = _cacheManager.Get<List<KeyValuePair<int, ContentInfo>>>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<List<KeyValuePair<int, ContentInfo>>>(cacheKey);
            if (retval == null)
            {
                retval = GetContainerContentListBySqlString(sqlString, order, totalNum, pageNum,
                currentPageIndex);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public string StlGetValue(ChannelInfo channelInfo, int contentId, string type)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetValue), channelInfo.Id.ToString(), contentId.ToString(), type);
            var retval = _cacheManager.Get<string>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<string>(cacheKey);
            if (retval == null)
            {
                retval = GetValue<string>(contentId, type);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public int StlGetSequence(ChannelInfo channelInfo, int contentId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetSequence),
                    channelInfo.Id.ToString(), contentId.ToString());
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval = GetSequence(channelInfo.Id, contentId);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public int StlGetCountCheckedImage(int siteId, ChannelInfo channelInfo)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetCountCheckedImage),
                    siteId.ToString(), channelInfo.Id.ToString());
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval = GetCountCheckedImage(siteId, channelInfo.Id);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public async Task<int> StlGetCountOfContentAddAsync(int siteId, ChannelInfo channelInfo, ScopeType scope,
            DateTime begin, DateTime end, string userName, bool? checkedState)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetCountOfContentAddAsync),
                    siteId.ToString(), channelInfo.Id.ToString(), scope.Value,
                    DateUtils.GetDateString(begin), DateUtils.GetDateString(end), userName, checkedState.HasValue ? checkedState.ToString() : string.Empty);
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval = await GetCountOfContentAddAsync(siteId, channelInfo.Id, scope,
                begin, end, userName, checkedState);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public int StlGetContentId(ChannelInfo channelInfo, int taxis, bool isNextContent)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetContentId), channelInfo.Id.ToString(), taxis.ToString(), isNextContent.ToString());
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval = GetContentId(channelInfo.Id, taxis, isNextContent);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public int StlGetContentId(ChannelInfo channelInfo, TaxisType taxisType)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetContentId), channelInfo.Id.ToString(), taxisType.Value);
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval = GetContentId(channelInfo.Id, taxisType);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public int StlGetChannelId(ChannelInfo channelInfo, int contentId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetChannelId), channelInfo.Id.ToString(), contentId.ToString());
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval = GetChannelId(contentId);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public Query StlGetStlWhereString(int siteId, string group, string groupNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ChannelInfo channelInfo, bool isRelatedContents, int contentId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlWhereString),
                    siteId.ToString(), group, groupNot,
                    tags, isImage.ToString(), isVideo.ToString(), isFile.ToString(), isTop.ToString(), isRecommend.ToString(), isHot.ToString(), isColor.ToString(), channelInfo.Id.ToString(), isRelatedContents.ToString(), contentId.ToString());
            var retval = _cacheManager.Get<Query>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<Query>(cacheKey);
            if (retval == null)
            {
                retval = GetStlWhereString(siteId, channelInfo, group,
                groupNot,
                tags, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor, isRelatedContents, contentId);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public Query StlGetStlWhereString(int siteId, string group, string groupNot, string tags, bool? isTop, ChannelInfo channelInfo, bool isRelatedContents, int contentId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlWhereString),
                    siteId.ToString(), group, groupNot, tags, isTop.ToString(), channelInfo.Id.ToString(), isRelatedContents.ToString(), contentId.ToString());
            var retval = _cacheManager.Get<Query>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<Query>(cacheKey);
            if (retval == null)
            {
                retval = GetStlWhereString(siteId, channelInfo, group, groupNot, tags, isTop, isRelatedContents, contentId);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public async Task<List<ContentInfo>> StlGetStlSqlStringCheckedAsync(int siteId, ChannelInfo channelInfo, int startNum, int totalNum, string order, Query query, ScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlSqlStringCheckedAsync), siteId.ToString(), channelInfo.Id.ToString(), startNum.ToString(),
                    totalNum.ToString(), order, query.ToString(), scopeType.Value, groupChannel,
                    groupChannelNot);
            var retval = _cacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval == null)
            {
                var channelIdList = await _channelRepository.GetChannelIdListAsync(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
                retval = GetStlSqlStringChecked(channelIdList, siteId, channelInfo.Id, startNum,
                totalNum, order, query, scopeType, groupChannel, groupChannelNot);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public Query StlGetStlWhereStringBySearch(ChannelInfo channelInfo, string group, string groupNot, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository), nameof(StlGetStlWhereStringBySearch), channelInfo.Id.ToString(), group, groupNot, isImage.ToString(),
                    isVideo.ToString(), isFile.ToString(),
                    isTop.ToString(), isRecommend.ToString(),
                    isHot.ToString(), isColor.ToString());
            var retval = _cacheManager.Get<Query>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<Query>(cacheKey);
            if (retval == null)
            {
                retval = GetStlWhereStringBySearch(group, groupNot, isImage, isVideo, isFile, isTop, isRecommend, isHot, isColor);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public List<ContentInfo> StlGetStlSqlStringCheckedBySearch(ChannelInfo channelInfo, int startNum, int totalNum, string order, Query query)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ContentRepository),
                       nameof(StlGetStlSqlStringCheckedBySearch),
                       channelInfo.Id.ToString(), startNum.ToString(), totalNum.ToString(), order, query.ToString());
            var retval = _cacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<List<ContentInfo>>(cacheKey);
            if (retval == null)
            {
                retval = GetStlSqlStringCheckedBySearch(startNum, totalNum, order, query);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }
    }
}
