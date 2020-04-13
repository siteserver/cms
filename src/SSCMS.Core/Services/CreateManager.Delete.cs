using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CreateManager
    {
        public async Task DeleteContentsAsync(Site site, int channelId, IEnumerable<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                await DeleteContentAsync(site, channelId, contentId);
            }
        }

        public async Task DeleteContentAsync(Site site, int channelId, int contentId)
        {
            var filePath = await _pathManager.GetContentPageFilePathAsync(site, channelId, contentId, 0);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public async Task DeleteChannelsAsync(Site site, IEnumerable<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var filePath = await _pathManager.GetChannelPageFilePathAsync(site, channelId, 0);

                FileUtils.DeleteFileIfExists(filePath);

                var channel = await _channelRepository.GetAsync(channelId);
                var contentIdList = await _contentRepository.GetContentIdsAsync(site, channel);
                await DeleteContentsAsync(site, channelId, contentIdList);
            }
        }
    }
}
