using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Datory;
using Microsoft.Extensions.Caching.Distributed;
using SiteServer.Abstractions;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SqlKata;
using ETaxisType = SiteServer.Abstractions.TaxisType;
using ETaxisTypeUtils = SiteServer.Abstractions.ETaxisTypeUtils;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        public async Task<int> InsertAsync(Site site, Channel channel, Content content)
        {
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
            var taxis = 0;
            if (content.SourceId == SourceManager.Preview)
            {
                channel.IsPreviewContentsExists = true;
                await DataProvider.ChannelRepository.UpdateAdditionalAsync(channel);
            }
            else
            {
                if (content.Top)
                {
                    taxis = await GetMaxTaxisAsync(tableName, content.ChannelId, true) + 1;
                }
                else
                {
                    taxis = await GetMaxTaxisAsync(tableName, content.ChannelId, false) + 1;
                }
            }
            return await InsertWithTaxisAsync(site, channel, content, taxis);
        }

        public async Task<int> InsertPreviewAsync(Site site, Channel channel, Content content)
        {
            channel.IsPreviewContentsExists = true;
            await DataProvider.ChannelRepository.UpdateAdditionalAsync(channel);

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

            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
            if (string.IsNullOrEmpty(tableName)) return 0;

            content.LastEditDate = DateTime.Now;

            var repository = GetRepository(tableName);
            content.Id = await repository.InsertAsync(content);

            return content.Id;
        }
    }
}
