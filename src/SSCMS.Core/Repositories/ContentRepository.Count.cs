using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task<int> GetCountAsync(Site site, IChannelSummary channel)
        {
            var summaries = await GetSummariesAsync(site, channel);
            return summaries.Count;
        }

        public async Task<int> GetCountOfCheckedAsync(Site site, IChannelSummary channel)
        {
            var summaries = await GetSummariesAsync(site, channel);
            return summaries.Where(x => x.Checked).ToList().Count;
        }

        public async Task<int> GetCountOfUnCheckedAsync(Site site, IChannelSummary channel)
        {
            var summaries = await GetSummariesAsync(site, channel);
            return summaries.Where(x => !x.Checked).ToList().Count;
        }

        public async Task<int> GetCountCheckedImageAsync(Site site, Channel channel)
        {
            var repository = await GetRepositoryAsync(site, channel);

            return await repository.CountAsync(GetQuery(site.Id, channel.Id)
                       .WhereTrue(nameof(Content.Checked))
                       .WhereNotNullOrEmpty(nameof(Content.ImageUrl))
                   );
        }

        public async Task<int> GetCountOfCheckedImagesAsync(Site site, IChannelSummary channel)
        {
            var repository = await GetRepositoryAsync(site, channel);

            return await repository.CountAsync(GetQuery(site.Id, channel.Id)
                       .WhereTrue(nameof(Content.Checked))
                       .WhereNotNullOrEmpty(nameof(Content.ImageUrl))
                   );
        }

        public async Task<int> GetCountAsync(string tableName, Query query)
        {
            var repository = await GetRepositoryAsync(tableName);
            return await repository.CountAsync(query);
        }

        public async Task<int> GetCountOfContentAddAsync(string tableName, int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, int adminId, bool? checkedState)
        {
            var channelIdList = await _channelRepository.GetChannelIdsAsync(siteId, channelId, scope);
            return await GetCountOfContentAddAsync(tableName, siteId, channelIdList, begin, end, adminId, checkedState);
        }

        private async Task<int> GetCountOfContentAddAsync(string tableName, int siteId, List<int> channelIdList, DateTime begin, DateTime end, int adminId, bool? checkedState)
        {
            var repository = await GetRepositoryAsync(tableName);

            var query = Q.Where(nameof(Content.SiteId), siteId);
            query.WhereIn(nameof(Content.ChannelId), channelIdList);
            query.WhereBetween(nameof(Content.AddDate), begin, end.AddDays(1));
            if (adminId > 0)
            {
                query.Where(nameof(Content.AdminId), adminId);
            }

            if (checkedState.HasValue)
            {
                query.Where(nameof(Content.Checked), TranslateUtils.ToBool(checkedState.ToString()));
            }

            return await repository.CountAsync(query);
        }
        
        public async Task<int> GetCountCheckingAsync(Site site)
        {
            var channels = await _channelRepository.GetSummariesAsync(site.Id);
            var count = 0;
            foreach (var channel in channels)
            {
                var summaries = await GetSummariesAsync(site, channel);
                count += summaries.Count(summary =>
                {
                    return !summary.Checked && summary.CheckedLevel == 0;
                });
            }

            return count;
        }

        public async Task<int> GetCountCheckingAsync(Site site, List<int> channelIds)
        {
            if (channelIds == null || channelIds.Count == 0)
            {
                return 0;
            }
            else if (channelIds.Contains(site.Id))
            {
                return await GetCountCheckingAsync(site);
            }

            var channels = await _channelRepository.GetSummariesAsync(site.Id);
            var count = 0;
            foreach (var channel in channels)
            {
                var isSelfOrChildren = false;
                if (channelIds.Contains(channel.Id))
                {
                    isSelfOrChildren = true;
                }
                else
                {
                    foreach (var channelId in channelIds)
                    {
                        if (ListUtils.Contains(channel.ParentsPath, channelId))
                        {
                            isSelfOrChildren = true;
                            break;
                        }
                    }
                }
                if (!isSelfOrChildren) continue;

                var summaries = await GetSummariesAsync(site, channel);
                count += summaries.Count(summary =>
                {
                    return !summary.Checked && summary.CheckedLevel == 0;
                });
            }

            return count;
        }
    }
}