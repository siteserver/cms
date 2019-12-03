using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SqlKata;


namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        Task<IEnumerable<Content>> GetSelectCommandByHitsAnalysisAsync(int siteId);

        Task<Query> GetStlWhereStringAsync(int siteId, Channel channelInfo, string group, string groupNot, string tags, bool? isTop, bool isRelatedContents, int contentId);

        Task<Query> GetWhereStringByStlSearchAsync(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes, NameValueCollection form);

        string GetSqlStringByContentGroup(string contentGroupName, int siteId);

        string GetSqlStringByContentTag(string tag, int siteId);

        Task<IEnumerable<Content>> GetStlSqlStringCheckedAsync(List<int> channelIdList, int siteId, int channelId, int startNum, int totalNum, string order, Query query, ScopeType scopeType, string groupChannel, string groupChannelNot);

        Task<IEnumerable<Content>> GetStlSqlStringCheckedBySearchAsync(int startNum, int totalNum, string order, Query query);

        Task<Query> GetStlWhereStringAsync(int siteId, Channel channelInfo, string group, string groupNot, string tags, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor, bool isRelatedContents, int contentId);

        Query GetStlWhereStringBySearch(string group, string groupNot, bool? isImage, bool? isVideo, bool? isFile, bool? isTop, bool? isRecommend, bool? isHot, bool? isColor);

        Query GetCacheWhereString(Site siteInfo, Channel channelInfo, int? onlyAdminId);

        Task<IEnumerable<Content>> GetStlDataSourceCheckedAsync(List<int> channelIdList, int startNum, int totalNum, TaxisType taxisType, Query query, NameValueCollection others);
    }
}
