using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService
    {
        private async Task PublishAsync(ScheduledTask task)
        {
            if (task.PublishSiteId == 0 || task.PublishChannelId == 0 || task.PublishContentId == 0) return;

            var site = await _databaseManager.SiteRepository.GetAsync(task.PublishSiteId);
            var channel = await _databaseManager.ChannelRepository.GetAsync(task.PublishChannelId);
            var content = await _databaseManager.ContentRepository.GetAsync(site, channel,  task.PublishContentId);

            content.Checked = true;
            content.CheckedLevel = 0;

            await _databaseManager.ContentRepository.UpdateAsync(site, channel, content);

            await _createManager.ExecuteAsync(site.Id, CreateType.Content, channel.Id, content.Id);
            
            var channelIdList = ListUtils.GetIntList(channel.CreateChannelIdsIfContentChanged);
            if (channel.IsCreateChannelIfContentChanged && !channelIdList.Contains(channel.Id))
            {
                channelIdList.Add(channel.Id);
            }
            if (!channelIdList.Contains(site.Id))
            {
                channelIdList.Add(site.Id);
            }
            foreach (var theChannelId in channelIdList)
            {
                await _createManager.ExecuteAsync(site.Id, CreateType.Channel, theChannelId);
            }

            await _databaseManager.ScheduledTaskRepository.DeleteAsync(task.Id);
        }
    }
}
