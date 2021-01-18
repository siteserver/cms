using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<List<ContentSummary>> SearchAsync(Site site, Channel channel, bool isAllContents, string searchType,
            string searchText, bool isAdvanced, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot,
            bool isColor, List<string> groupNames, List<string> tagNames);

        Task<(int Total, List<ContentSummary> PageSummaries)> UserWriteSearchAsync(int userId, Site site, int page, int? channelId, bool isCheckedLevels, List<int> checkedLevels, List<string> groupNames, List<string> tagNames);

        Task<(int Total, List<ContentSummary> PageSummaries)> AdvancedSearchAsync(Site site, int page, List<int> channelIds,
            bool isAllContents, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items,
            bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor,
            List<string> groupNames, List<string> tagNames, bool isAdmin, int adminId, bool isUser);

        Task<(int Total, List<ContentSummary> PageSummaries)> AdvancedSearchAsync(Site site, int page, List<int> channelIds,
            bool isAllContents);

        Task<(int Total, List<ContentSummary> PageSummaries)> CheckSearchAsync(Site site, int page, int? channelId,
            DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items,
            bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor,
            List<string> groupNames, List<string> tagNames);

        Task<(int Total, List<ContentSummary> PageSummaries)> RecycleSearchAsync(Site site, int page, int? channelId,
            DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items,
            bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor,
            List<string> groupNames, List<string> tagNames);
    }
}
