using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.Core;

namespace SS.CMS.Repositories
{
    public partial class ContentRepository
    {
        public async Task<int> InsertAsync(Site site, Channel channel, Content content)
        {
            var taxis = 0;
            if (content.SourceId == SourceManager.Preview)
            {
                channel.IsPreviewContentsExists = true;
                await _channelRepository.UpdateAsync(channel);
            }
            else
            {
                if (content.Top)
                {
                    taxis = await GetMaxTaxisAsync(site, channel, true) + 1;
                }
                else
                {
                    taxis = await GetMaxTaxisAsync(site, channel, false) + 1;
                }
            }
            return await InsertWithTaxisAsync(site, channel, content, taxis);
        }

        public async Task<int> InsertPreviewAsync(Site site, Channel channel, Content content)
        {
            channel.IsPreviewContentsExists = true;
            await _channelRepository.UpdateAsync(channel);

            content.SourceId = SourceManager.Preview;
            return await InsertWithTaxisAsync(site, channel, content, 0);
        }

        public async Task<int> InsertWithTaxisAsync(Site site, Channel channel, Content content, int taxis)
        {
            if (site.IsAutoPageInTextEditor && content.ContainsKey(ContentAttribute.Content))
            {
                content.Set(ContentAttribute.Content, ContentUtility.GetAutoPageContent(content.Get<string>(ContentAttribute.Content), site.AutoPageWordNum));
            }

            content.Taxis = taxis;

            var tableName = await _channelRepository.GetTableNameAsync(site, channel);
            if (string.IsNullOrEmpty(tableName)) return 0;

            content.LastEditDate = DateTime.Now;

            var repository = GetRepository(tableName);
            if (content.SourceId == SourceManager.Preview)
            {
                await repository.InsertAsync(content);
            }
            else
            {
                var cacheKeys = new List<string>
                {
                    GetListKey(tableName, content.SiteId, content.ChannelId),
                    GetCountKey(tableName, content.SiteId, content.ChannelId)
                };
                await repository.InsertAsync(content, Q.CachingRemove(cacheKeys.ToArray()));
            }

            return content.Id;
        }
    }
}
