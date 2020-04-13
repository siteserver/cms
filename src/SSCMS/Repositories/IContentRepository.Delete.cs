using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        Task RecycleContentsAsync(Site site, Channel channel, IEnumerable<int> contentIdList, int adminId);

        Task RecycleAllAsync(Site site, int channelId, int adminId);

        Task RecycleDeleteAsync(Site site, int channelId, string tableName, List<int> contentIdList);

        Task RecycleDeleteAllAsync(IOldPluginManager pluginManager, Site site);

        Task RecycleRestoreAllAsync(IOldPluginManager pluginManager, Site site, int restoreChannelId);

        Task RecycleRestoreAsync(Site site, int channelId, string tableName, List<int> contentIdList,
            int restoreChannelId);

        Task DeleteAsync(IOldPluginManager pluginManager, Site site, Channel channel, int contentId);
    }
}
