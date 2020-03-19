using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IContentRepository
    {
Task<List<ContentSummary>> Search(Site site, Channel channel, bool isAllContents, string searchType, string searchText, bool isAdvanced, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames);

        Task<(int Total, List<ContentSummary> PageSummaries)> AdvancedSearch(Site site, int page, List<int> channelIds, bool isAllContents, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items, bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames, bool isAdmin, int adminId, bool isUser);

        Task<(int Total, List<ContentSummary> PageSummaries)> CheckSearch(Site site, int page, int? channelId, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items, bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames);

        Task<(int Total, List<ContentSummary> PageSummaries)> RecycleSearch(Site site, int page, int? channelId, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items, bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames);
    }
}
