using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task UpdateAsync(Content content)
        {
            if (content == null) return;

            var site = await _siteRepository.GetAsync(content.SiteId);
            var channel = await _channelRepository.GetAsync(content.ChannelId);
            await UpdateAsync(site, channel, content);
        }


        public async Task UpdateAsync(Site site, Channel channel, Content content)
        {
            if (content == null) return;

            var repository = GetRepository(site, channel);

            if (site.IsAutoPageInTextEditor)
            {
                content.Body = ContentUtility.GetAutoPageBody(content.Body,
                    site.AutoPageWordNum);
            }

            if (content.Top == false && content.Taxis >= TaxisIsTopStartValue)
            {
                content.Taxis = await GetMaxTaxisAsync(site, channel, false) + 1;
            }
            else if (content.Top && content.Taxis < TaxisIsTopStartValue)
            {
                content.Taxis = await GetMaxTaxisAsync(site, channel, true) + 1;
            }

            await repository.UpdateAsync(content, Q
                .CachingRemove(GetListKey(repository.TableName, content.SiteId, content.ChannelId))
                .CachingRemove(GetEntityKey(repository.TableName, content.Id))
            );

            await _statRepository.AddCountAsync(StatType.ContentEdit, content.SiteId);
        }

        public async Task SetAutoPageContentToSiteAsync(Site site)
        {
            if (!site.IsAutoPageInTextEditor) return;

            var tableNames = await _siteRepository.GetAllTableNamesAsync();
            foreach (var tableName in tableNames)
            {
                var repository = GetRepository(tableName);

                var list = await repository.GetAllAsync<(int ContentId, string Content)>(
                    GetQuery(site.Id)
                    .Select(nameof(Content.Id), nameof(Content.Body))
                );

                foreach (var (contentId, contentValue) in list)
                {
                    var body = ContentUtility.GetAutoPageBody(contentValue, site.AutoPageWordNum);
                    await repository.UpdateAsync(
                        GetQuery(site.Id)
                        .Set(nameof(Content.Body), body)
                        .Where(nameof(Content.Id), contentId)
                        .CachingRemove(GetEntityKey(tableName, contentId))
                    );
                }
            }
        }

        public async Task UpdateArrangeTaxisAsync(Site site, Channel channel, string attributeName, bool isDesc)
        {
            var query = GetQuery(site.Id, channel.Id);
            query.Select(nameof(Content.Id), nameof(Content.Top));
            //由于页面排序是按Taxis的Desc排序的，所以这里sql里面的ASC/DESC取反
            if (isDesc)
            {
                query.OrderBy(attributeName);
            }
            else
            {
                query.OrderByDesc(attributeName);
            }

            var repository = GetRepository(site, channel);
            var list = await repository.GetAllAsync<(int id, bool top)>(query);
            var taxis = 0;
            var topTaxis = TaxisIsTopStartValue;
            foreach (var (id, top) in list)
            {
                if (top)
                {
                    topTaxis++;
                }
                else
                {
                    taxis++;
                }
                await repository.UpdateAsync(Q
                    .Set(nameof(Content.Taxis), top ? topTaxis : taxis)
                    .Set(nameof(Content.Top), top)
                    .Where(nameof(Content.Id), id)
                    .CachingRemove(GetEntityKey(repository.TableName, id))
                );
            }

            await repository.RemoveCacheAsync(
                GetListKey(repository.TableName, site.Id, channel.Id)
            );
        }

        public async Task<bool> SetTaxisToUpAsync(Site site, Channel channel, int contentId, bool isTop)
        {
            var repository = GetRepository(site, channel);

            var taxis = await repository.GetAsync<int>(
                GetQuery(site.Id, channel.Id)
                .Select(nameof(Content.Taxis))
                .Where(nameof(Content.Id), contentId)
            );

            var result = await repository.GetAsync<Content>(
                GetQuery(site.Id, channel.Id)
                .Select(nameof(Content.Id), nameof(Content.Taxis))
                .Where(nameof(Content.Taxis), ">", taxis)
                .Where(nameof(Content.Taxis), isTop ? ">" : "<", TaxisIsTopStartValue)
                .OrderBy(nameof(Content.Taxis)));

            var higherId = 0;
            var higherTaxis = 0;
            if (result != null)
            {
                higherId = result.Id;
                higherTaxis = result.Taxis;
            }

            if (higherId == 0) return false;

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                .Set(nameof(Content.Taxis), higherTaxis)
                .Where(nameof(Content.Id), contentId)
            );

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                .Set(nameof(Content.Taxis), taxis)
                .Where(nameof(Content.Id), higherId)
            );

            await repository.RemoveCacheAsync(
                GetEntityKey(repository.TableName, contentId),
                GetEntityKey(repository.TableName, higherId),
                GetListKey(repository.TableName, site.Id, channel.Id)
            );

            return true;
        }

        public async Task<bool> SetTaxisToDownAsync(Site site, Channel channel, int contentId, bool isTop)
        {
            var repository = GetRepository(site, channel);

            var taxis = await repository.GetAsync<int>(
                GetQuery(site.Id, channel.Id)
                .Select(nameof(Content.Taxis))
                .Where(nameof(Content.Id), contentId)
            );

            var result = await repository.GetAsync<Content>(
                GetQuery(site.Id, channel.Id)
                .Select(nameof(Content.Id), nameof(Content.Taxis))
                .Where(nameof(Content.Taxis), "<", taxis)
                .Where(nameof(Content.Taxis), isTop ? ">" : "<", TaxisIsTopStartValue)
                .OrderByDesc(nameof(Content.Taxis)));

            var lowerId = 0;
            var lowerTaxis = 0;
            if (result != null)
            {
                lowerId = result.Id;
                lowerTaxis = result.Taxis;
            }

            if (lowerId == 0) return false;

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                .Set(nameof(Content.Taxis), lowerTaxis)
                .Where(nameof(Content.Id), contentId)
            );

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                .Set(nameof(Content.Taxis), taxis)
                .Where(nameof(Content.Id), lowerId)
            );

            await repository.RemoveCacheAsync(
                GetEntityKey(repository.TableName, contentId),
                GetEntityKey(repository.TableName, lowerId),
                GetListKey(repository.TableName, site.Id, channel.Id)
            );

            return true;
        }

        public async Task AddDownloadsAsync(string tableName, int channelId, int contentId)
        {
            var repository = GetRepository(tableName);

            await repository.IncrementAsync(nameof(Content.Downloads), Q
                .Where(nameof(Content.Id), contentId)
                .CachingRemove(GetEntityKey(tableName, contentId))
            );
        }
    }
}
