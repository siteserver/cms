using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        private string GetAllListAndCountKey(string tableName, int siteId)
        {
            return Caching.GetAllKey(tableName, "ListAndCount", siteId.ToString());
        }

        private string GetAllEntityKey(string tableName, int siteId)
        {
            return Caching.GetAllKey(tableName, "Entity", siteId.ToString());
        }

        private string GetCountKey(string tableName, int siteId, int channelId)
        {
            return Caching.GetCountKey(tableName, siteId, channelId);
        }

        private string GetEntityKey(string tableName, int contentId)
        {
            return Caching.GetEntityKey(tableName, contentId);
        }

        private string GetListKey(string tableName, bool isAllContents, int siteId, int channelId)
        {
            return Caching.GetListKey(tableName, isAllContents.ToString(), siteId.ToString(), channelId.ToString());
        }

        public async Task CacheAllListAndCountAsync(Site site, List<Channel> channels)
        {
            var repository = GetRepository(site.TableName);
            var allKey = GetAllListAndCountKey(repository.TableName, site.Id);

            var cacheManager = await repository.GetCacheManagerAsync();
            if (!cacheManager.Exists(allKey))
            {
                var allSummaries = await repository.GetAllAsync<ContentSummary>(Q
                    .Select(nameof(Content.ChannelId), nameof(Content.Id))
                    .Where(nameof(Content.SiteId), site.Id)
                    .WhereNot(nameof(Content.SourceId), SourceManager.Preview)
                    .OrderByDesc(nameof(Content.Taxis), nameof(Content.Id))
                );

                foreach (var channel in channels)
                {
                    var listKey = GetListKey(repository.TableName, channel.IsAllContents, site.Id, channel.Id);
                    var summaries = allSummaries.Where(x => x.ChannelId == channel.Id).ToList();
                    cacheManager.Put(listKey, summaries);

                    var countKey = GetCountKey(repository.TableName, site.Id, channel.Id);
                    var count = summaries.Count();
                    cacheManager.Put(countKey, count);
                }

                cacheManager.Put(allKey, true);
            }
        }

        public async Task CacheAllEntityAsync(Site site, List<int> channelIds)
        {
            var repository = GetRepository(site.TableName);
            var allKey = GetAllEntityKey(repository.TableName, site.Id);

            var cacheManager = await repository.GetCacheManagerAsync();
            if (!cacheManager.Exists(allKey))
            {
                foreach (var channelId in channelIds)
                {
                    var allContents = await repository.GetAllAsync(Q
                        .Where(nameof(Content.SiteId), site.Id)
                        .Where(nameof(Content.ChannelId), channelId)
                        .WhereNot(nameof(Content.SourceId), SourceManager.Preview)
                        .Limit(site.PageSize)
                        .OrderByDesc(nameof(Content.Taxis), nameof(Content.Id))
                    );

                    foreach (var content in allContents)
                    {
                        cacheManager.Put(GetEntityKey(repository.TableName, content.Id), content);
                    }
                }

                cacheManager.Put(allKey, true);
            }
        }

        public async Task<int> GetCountAsync(Site site, IChannelSummary channel)
        {
            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
            var repository = GetRepository(tableName);

            return await repository.CountAsync(
                GetQuery(site.Id, channel.Id)
                    .CachingGet(GetCountKey(tableName, site.Id, channel.Id))
            );
        }

        private async Task<Content> GetAsync(string tableName, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

            var repository = GetRepository(tableName);
            return await repository.GetAsync(contentId, Q
                .CachingGet(GetEntityKey(tableName, contentId))
            );
        }

        public async Task<Content> GetAsync(Site site, int channelId, int contentId)
        {
            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelId);
            return await GetAsync(tableName, contentId);
        }

        public async Task<Content> GetAsync(Site site, Channel channel, int contentId)
        {
            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
            return await GetAsync(tableName, contentId);
        }

        public async Task<List<int>> GetContentIdsAsync(Site site, Channel channel)
        {
            var summaries = await GetSummariesAsync(site, channel, false);
            return summaries.Select(x => x.Id).ToList();
        }

        public async Task<List<ContentSummary>> GetSummariesAsync(Site site, Channel channel, bool isAllContents)
        {
            var repository = await GetRepositoryAsync(site, channel);
            var query = Q.Select(nameof(Content.ChannelId), nameof(Content.Id));

            await QueryWhereAsync(query, site, channel, isAllContents);

            if (isAllContents)
            {
                query.OrderBy(ContentAttribute.ChannelId).OrderByDesc(nameof(Content.Taxis), nameof(Content.Id));
            }
            else
            {
                query.OrderByDesc(nameof(Content.Taxis), nameof(Content.Id));
            }

            query.CachingGet(GetListKey(repository.TableName, isAllContents, site.Id, channel.Id));

            return await repository.GetAllAsync<ContentSummary>(query);
        }

        public async Task<List<ContentSummary>> Search(Site site, Channel channel, bool isAllContents, string searchType, string searchText, bool isAdvanced, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames)
        {
            var repository = await GetRepositoryAsync(site, channel);
            var query = Q.Select(nameof(Content.ChannelId), nameof(Content.Id));

            await QueryWhereAsync(query, site, channel, isAllContents);

            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchText))
            {
                query.WhereLike(searchType, $"%{searchText}%");
            }

            if (isAdvanced)
            {
                if (checkedLevels != null && checkedLevels.Count > 0)
                {
                    query.Where(q =>
                    {
                        if (checkedLevels.Contains(site.CheckContentLevel))
                        {
                            q.OrWhere(nameof(Content.Checked), true);
                        }

                        q.OrWhereIn(nameof(Content.CheckedLevel), checkedLevels);
                        return q;
                    });
                }

                if (groupNames != null && groupNames.Count > 0)
                {
                    query.Where(q =>
                    {
                        foreach (var groupName in groupNames)
                        {
                            q.OrWhereLike(nameof(Content.GroupNames), $"%{groupName}%");
                        }
                        return q;
                    });
                }

                if (tagNames != null && tagNames.Count > 0)
                {
                    query.Where(q =>
                    {
                        foreach (var tagName in tagNames)
                        {
                            q.OrWhereLike(nameof(Content.TagNames), $"%{tagName}%");
                        }
                        return q;
                    });
                }

                if (isTop)
                {
                    query.Where(nameof(Content.Top), true);
                }

                if (isRecommend)
                {
                    query.Where(nameof(Content.Recommend), true);
                }

                if (isHot)
                {
                    query.Where(nameof(Content.Hot), true);
                }

                if (isColor)
                {
                    query.Where(nameof(Content.Color), true);
                }
            }

            if (isAllContents)
            {
                query.OrderBy(ContentAttribute.ChannelId).OrderByDesc(nameof(Content.Taxis), nameof(Content.Id));
            }
            else
            {
                query.OrderByDesc(nameof(Content.Taxis), nameof(Content.Id));
            }

            return await repository.GetAllAsync<ContentSummary>(query);
        }
    }
}
