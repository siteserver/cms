using System;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Models;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task<int> InsertAsync(Site siteInfo, Channel channelInfo, Content contentInfo)
        {
            var taxis = 0;
            if (contentInfo.SourceId == SourceManager.Preview)
            {
                channelInfo.IsPreviewContentsExists = true;
                await _channelRepository.UpdateExtendAsync(channelInfo);
            }
            else
            {
                taxis = await GetTaxisToInsertAsync(contentInfo.ChannelId, contentInfo.IsTop);
            }
            return await InsertWithTaxisAsync(siteInfo, channelInfo, contentInfo, taxis);
        }

        public async Task<int> InsertPreviewAsync(Site siteInfo, Channel channelInfo, Content contentInfo)
        {
            channelInfo.IsPreviewContentsExists = true;
            await _channelRepository.UpdateExtendAsync(channelInfo);

            contentInfo.SourceId = SourceManager.Preview;
            return await InsertWithTaxisAsync(siteInfo, channelInfo, contentInfo, 0);
        }

        public async Task<int> InsertWithTaxisAsync(Site siteInfo, Channel channelInfo, Content contentInfo, int taxis)
        {
            if (siteInfo.IsAutoPageInTextEditor && !string.IsNullOrEmpty(contentInfo.Body))
            {
                contentInfo.Body = ContentUtility.GetAutoPageContent(contentInfo.Body, siteInfo.AutoPageWordNum);
            }
            contentInfo.Taxis = taxis;
            contentInfo.SiteId = siteInfo.Id;
            contentInfo.ChannelId = channelInfo.Id;

            contentInfo.Id = await _repository.InsertAsync(contentInfo);

            await InsertCacheAsync(siteInfo, channelInfo, contentInfo);

            return contentInfo.Id;
        }
    }
}
