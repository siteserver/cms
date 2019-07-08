using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task UpdateArrangeTaxisAsync(int channelId, string attributeName, bool isDesc);

        Task<bool> SetTaxisToUpAsync(int channelId, int contentId, bool isTop);

        Task<bool> SetTaxisToDownAsync(int channelId, int contentId, bool isTop);

        Task UpdateAsync(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        Task AddDownloadsAsync(int channelId, int contentId);

        Task UpdateRestoreContentsByTrashAsync(int siteId, int channelId);

        Task UpdateIsCheckedAsync(int siteId, int channelId, List<int> contentIdList, int translateChannelId, int userId, bool isChecked, int checkedLevel, string reasons);

        Task SetAutoPageContentToSiteAsync(int siteId);

        Task AddContentGroupListAsync(int contentId, List<string> contentGroupList);

        Task UpdateAsync(int channelId, int contentId, string name, string value);

        Task UpdateTrashContentsAsync(int siteId, int channelId, IList<int> contentIdList);

        Task UpdateTrashContentsByChannelIdAsync(int siteId, int channelId);
    }
}
