﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        Task ClearAllListCacheAsync(Site site);

        Task CacheAllListAndCountAsync(Site site, List<ChannelSummary> channelSummaries);

        Task CacheAllEntityAsync(Site site, List<ChannelSummary> channelSummaries);

        Task<Content> GetAsync(int siteId, int channelId, int contentId);

        Task<Content> GetAsync(Site site, int channelId, int contentId);

        Task<Content> GetAsync(Site site, Channel channel, int contentId);

        Task<List<int>> GetContentIdsAsync(Site site, Channel channel);

        Task<List<int>> GetContentIdsCheckedAsync(Site site, Channel channel);

        Task<List<int>> GetContentIdsAsync(Site site, Channel channel, bool isPeriods, string dateFrom, string dateTo, bool? checkedState);

        Task<List<int>> GetContentIdsByLinkTypeAsync(Site site, Channel channel, LinkType linkType);

        Task<List<int>> GetChannelIdsCheckedByLastModifiedDateHourAsync(Site site, int hour);

        Task<List<ContentSummary>> GetSummariesAsync(Site site, Channel channel, bool isAllContents);

        Task<List<ContentSummary>> GetSummariesAsync(Site site, IChannelSummary channel);
    }
}
