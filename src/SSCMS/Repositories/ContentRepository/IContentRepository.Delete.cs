using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IContentRepository
    {
        Task RecycleContentsAsync(Site site, Channel channel, IEnumerable<int> contentIdList, int adminId);

        Task RecycleAllAsync(Site site, int channelId, int adminId);

        Task RecycleDeleteAsync(Site site, int channelId, string tableName, List<int> contentIdList);

        Task RecycleDeleteAllAsync(IPluginManager pluginManager, Site site);

        Task RecycleRestoreAllAsync(IPluginManager pluginManager, Site site, int restoreChannelId);

        Task RecycleRestoreAsync(Site site, int channelId, string tableName, List<int> contentIdList,
            int restoreChannelId);

        Task DeleteAsync(IPluginManager pluginManager, Site site, Channel channel, int contentId);
    }
}
