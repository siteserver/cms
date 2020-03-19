using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS;
using SSCMS.Core.Utils;

namespace SSCMS.Core.Repositories.ContentRepository
{
    public partial class ContentRepository
    {
        private async Task<int> SyncTaxisAsync(Site site, Channel channel, Content content)
        {
            var taxis = content.Taxis;
            var updateHigher = false;

            var repository = GetRepository(site, channel);

            var query = Q
                .Where(nameof(Content.ChannelId), channel.Id)
                .WhereNot(nameof(Content.SourceId), SourceManager.Preview)
                .Where(nameof(Content.Taxis), content.Top ? ">=" : "<", TaxisIsTopStartValue);

            if (channel.DefaultTaxisType == TaxisType.OrderByAddDate)
            {
                if (content.AddDate.HasValue)
                {
                    taxis = (await repository.MaxAsync(nameof(Content.Taxis), query.Clone()
                                 .WhereDate(nameof(Content.AddDate), ">", content.AddDate.Value)
                             ) ?? 0) + 1;
                    updateHigher = true;
                }
            }
            else if (channel.DefaultTaxisType == TaxisType.OrderByAddDateDesc)
            {
                if (content.AddDate.HasValue)
                {
                    taxis = (await repository.MaxAsync(nameof(Content.Taxis), query.Clone()
                                 .WhereDate(nameof(Content.AddDate), "<", content.AddDate.Value)
                             ) ?? 0) + 1;
                    updateHigher = true;
                }
            }
            else if (channel.DefaultTaxisType == TaxisType.OrderByTaxis)
            {
                taxis = 1;
                updateHigher = true;
            }
            else
            {
                if (content.Top == false && content.Taxis >= TaxisIsTopStartValue)
                {
                    taxis = await GetMaxTaxisAsync(site, channel, false) + 1;
                }
                else if (content.Top && content.Taxis < TaxisIsTopStartValue)
                {
                    taxis = await GetMaxTaxisAsync(site, channel, true) + 1;
                }

                if (taxis == 0)
                {
                    taxis = (await repository.MaxAsync(nameof(Content.Taxis), query.Clone()) ?? 0) + 1;
                }
            }

            if (content.Top && taxis < TaxisIsTopStartValue)
            {
                taxis += TaxisIsTopStartValue;
            }

            if (updateHigher)
            {
                await repository.IncrementAsync(nameof(Content.Taxis), query
                    .Where(nameof(Content.Taxis), ">=", taxis)
                );
            }

            return taxis;
        }

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
                content.Taxis = await SyncTaxisAsync(site, channel, content);
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
            if (site.IsAutoPageInTextEditor)
            {
                content.Body = ContentUtility.GetAutoPageBody(content.Body, site.AutoPageWordNum);
            }

            content.Taxis = taxis;

            var tableName = _channelRepository.GetTableName(site, channel);
            if (string.IsNullOrEmpty(tableName)) return 0;

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
