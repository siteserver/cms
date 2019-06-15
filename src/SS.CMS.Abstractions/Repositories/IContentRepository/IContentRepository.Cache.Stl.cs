using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SqlKata;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository
    {
        void StlClearCache();

        List<int> StlGetContentIdListChecked(ChannelInfo channelInfo, TaxisType taxisType);

        List<ContentInfo> StlGetStlDataSourceChecked(List<int> channelIdList, ChannelInfo channelInfo, int startNum, int totalNum, TaxisType taxisType, Query query, NameValueCollection others);

        List<KeyValuePair<int, ContentInfo>> StlGetContainerContentListChecked(List<int> channelIdList, ChannelInfo channelInfo, int startNum, int totalNum, string order, Query where, NameValueCollection others);

        List<KeyValuePair<int, ContentInfo>> StlGetContainerContentListBySqlString(ChannelInfo channelInfo, string sqlString, string order, int totalNum, int pageNum, int currentPageIndex);

        string StlGetValue(ChannelInfo channelInfo, int contentId, string type);

        int StlGetSequence(ChannelInfo channelInfo, int contentId);

        int StlGetCountCheckedImage(int siteId, ChannelInfo channelInfo);

        int StlGetCountOfContentAdd(int siteId, ChannelInfo channelInfo, ScopeType scope,
            DateTime begin, DateTime end, string userName, bool? checkedState);

        int StlGetContentId(ChannelInfo channelInfo, int taxis, bool isNextContent);

        int StlGetContentId(ChannelInfo channelInfo, TaxisType taxisType);

        int StlGetChannelId(ChannelInfo channelInfo, int contentId);

        Query StlGetStlWhereString(int siteId, string group, string groupNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, ChannelInfo channelInfo, bool isRelatedContents, int contentId);

        Query StlGetStlWhereString(int siteId, string group, string groupNot, string tags, bool? isTop, ChannelInfo channelInfo, bool isRelatedContents, int contentId);

        List<ContentInfo> StlGetStlSqlStringChecked(int siteId, ChannelInfo channelInfo, int startNum, int totalNum, string order, Query query, ScopeType scopeType, string groupChannel, string groupChannelNot);

        Query StlGetStlWhereStringBySearch(ChannelInfo channelInfo, string group, string groupNot, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor);

        List<ContentInfo> StlGetStlSqlStringCheckedBySearch(ChannelInfo channelInfo, int startNum, int totalNum, string order, Query query);
    }
}
