using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        void UpdateArrangeTaxis(int channelId, string attributeName, bool isDesc);

        bool SetTaxisToUp(int channelId, int contentId, bool isTop);

        bool SetTaxisToDown(int channelId, int contentId, bool isTop);

        void Update(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        void AddDownloads(int channelId, int contentId);

        void UpdateRestoreContentsByTrash(int siteId, int channelId);

        void UpdateIsChecked(int siteId, int channelId, List<int> contentIdList, int translateChannelId, string userName, bool isChecked, int checkedLevel, string reasons);

        void SetAutoPageContentToSite(int siteId);

        void AddContentGroupList(int contentId, List<string> contentGroupList);

        void Update(int channelId, int contentId, string name, string value);

        void UpdateTrashContents(int siteId, int channelId, IList<int> contentIdList);

        void UpdateTrashContentsByChannelId(int siteId, int channelId);
    }
}
