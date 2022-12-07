using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService
    {
        private async Task CreateAsync(ScheduledTask task)
        {
            if (task.CreateSiteIds == null || task.CreateSiteIds.Count == 0) return;

            foreach (var siteId in task.CreateSiteIds)
            {
                if (task.CreateType == CreateType.Index)
                {
                    await _createManager.ExecuteAsync(siteId, CreateType.Index);
                }
                else if (task.CreateType == CreateType.Channel)
                {
                    var channelIds = await _databaseManager.ChannelRepository.GetChannelIdsAsync(siteId);
                    foreach (var channelId in channelIds)
                    {
                        await _createManager.ExecuteAsync(siteId, CreateType.Channel, siteId, channelId);
                    }
                }
                else if (task.CreateType == CreateType.All)
                {
                    await _createManager.ExecuteAsync(siteId, CreateType.All);
                }
            }
        }
    }
}
