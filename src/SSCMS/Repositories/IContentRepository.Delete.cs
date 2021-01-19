using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        Task TrashContentsAsync(Site site, Channel channel, List<int> contentIds, int adminId);

        Task TrashContentAsync(Site site, Channel channel, int contentId, int adminId);

        Task TrashContentsAsync(Site site, int channelId, int adminId);

        Task DeletePreviewAsync(Site site, Channel channel);

        Task DeleteTrashAsync(Site site, int channelId, string tableName, List<int> contentIds, IPluginManager pluginManager);

        Task DeleteTrashAsync(Site site, IPluginManager pluginManager);

        Task RestoreTrashAsync(Site site, int restoreChannelId);

        Task RestoreTrashAsync(Site site, int channelId, string tableName, List<int> contentIds,
            int restoreChannelId);
    }
}
