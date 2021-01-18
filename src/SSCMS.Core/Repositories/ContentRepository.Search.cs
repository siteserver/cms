using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task<List<ContentSummary>> SearchAsync(Site site, Channel channel, bool isAllContents, string searchType, string searchText, bool isAdvanced, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames)
        {
            var repository = GetRepository(site, channel);
            var query = Q.Select(nameof(Content.ChannelId), nameof(Content.Id));

            await QueryWhereAsync(query, site, channel.Id, isAllContents);

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
                query.OrderBy(nameof(Content.ChannelId)).OrderByDesc(nameof(Content.Taxis), nameof(Content.Id));
            }
            else
            {
                query.OrderByDesc(nameof(Content.Taxis), nameof(Content.Id));
            }

            return await repository.GetAllAsync<ContentSummary>(query);
        }

        public async Task<(int Total, List<ContentSummary> PageSummaries)> UserWriteSearchAsync(int userId, Site site, int page, int? channelId, bool isCheckedLevels, List<int> checkedLevels, List<string> groupNames, List<string> tagNames)
        {
            var repository = GetRepository(site.TableName);

            var query = Q
                .Select(nameof(Content.ChannelId), nameof(Content.Id))
                .Where(nameof(Content.SiteId), site.Id)
                .Where(nameof(Content.ChannelId), ">", 0)
                .Where(nameof(Content.UserId), userId)
                .WhereNot(nameof(Content.SourceId), SourceManager.Preview);

            if (channelId > 0)
            {
                query.Where(nameof(Content.ChannelId), channelId);
            }

            if (isCheckedLevels)
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

            query.OrderByDesc(nameof(Content.LastEditAdminId), nameof(Content.AddDate), nameof(Content.Id));

            var total = await repository.CountAsync(query);
            var pageSummaries = await repository.GetAllAsync<ContentSummary>(query.ForPage(page, site.PageSize));
            return (total, pageSummaries);
        }

        public async Task<(int Total, List<ContentSummary> PageSummaries)> AdvancedSearchAsync(Site site, int page, List<int> channelIds, bool isAllContents, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items, bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames, bool isAdmin, int adminId, bool isUser)
        {
            var repository = GetRepository(site.TableName);

            var idList = new List<int>(channelIds);
            if (isAllContents)
            {
                foreach (var channelId in channelIds)
                {
                    idList.AddRange(await _channelRepository.GetChannelIdsAsync(site.Id, channelId, ScopeType.All));
                }
            }

            var query = Q
                .Select(nameof(Content.ChannelId), nameof(Content.Id))
                .Where(nameof(Content.SiteId), site.Id)
                .WhereNot(nameof(Content.SourceId), SourceManager.Preview)
                .WhereIn(nameof(Content.ChannelId), idList.Distinct());

            if (startDate.HasValue)
            {
                query.WhereDate(nameof(Content.AddDate), ">", startDate.Value);
            }
            if (endDate.HasValue)
            {
                query.WhereDate(nameof(Content.AddDate), "<", endDate.Value);
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    {
                        query.WhereLike(item.Key, $"%{item.Value}%");
                    }
                }
            }

            if (isCheckedLevels)
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

            if (isAdmin)
            {
                query.Where(nameof(Content.AdminId), adminId);
            }

            if (isUser)
            {
                query.Where(nameof(Content.UserId), ">", 0);
            }

            if (isAllContents)
            {
                query.OrderBy(nameof(Content.ChannelId)).OrderByDesc(nameof(Content.Taxis), nameof(Content.Id));
            }
            else
            {
                query.OrderByDesc(nameof(Content.Taxis), nameof(Content.Id));
            }

            var total = await repository.CountAsync(query);
            var pageSummaries = await repository.GetAllAsync<ContentSummary>(query.ForPage(page, site.PageSize));
            return (total, pageSummaries);
        }

        public async Task<(int Total, List<ContentSummary> PageSummaries)> AdvancedSearchAsync(Site site, int page, List<int> channelIds, bool isAllContents)
        {
            return await AdvancedSearchAsync(site, page, channelIds, isAllContents, null, null, null, false, null, false, false,
                false, false, null, null, false, 0, false);
        }

        public async Task<(int Total, List<ContentSummary> PageSummaries)> CheckSearchAsync(Site site, int page, int? channelId, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items, bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames)
        {
            var repository = GetRepository(site.TableName);

            var query = Q
                .Select(nameof(Content.ChannelId), nameof(Content.Id))
                .Where(nameof(Content.SiteId), site.Id)
                .WhereNot(nameof(Content.SourceId), SourceManager.Preview)
                .Where(nameof(Content.ChannelId), ">", 0)
                .WhereNullOrFalse(nameof(Content.Checked))
                .OrderByDesc(nameof(Content.AddDate));

            if (channelId.HasValue)
            {
                query.Where(nameof(Content.ChannelId), channelId.Value);
            }

            if (startDate.HasValue)
            {
                query.WhereDate(nameof(Content.AddDate), ">", startDate.Value);
            }
            if (endDate.HasValue)
            {
                query.WhereDate(nameof(Content.AddDate), "<", endDate.Value);
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    {
                        query.WhereLike(item.Key, $"%{item.Value}%");
                    }
                }
            }

            if (isCheckedLevels)
            {
                if (checkedLevels != null && checkedLevels.Count > 0)
                {
                    query.Where(q =>
                    {
                        q.OrWhereIn(nameof(Content.CheckedLevel), checkedLevels);
                        return q;
                    });
                }
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

            var total = await repository.CountAsync(query);
            var pageSummaries = await repository.GetAllAsync<ContentSummary>(query.ForPage(page, site.PageSize));
            return (total, pageSummaries);
        }

        public async Task<(int Total, List<ContentSummary> PageSummaries)> RecycleSearchAsync(Site site, int page, int? channelId, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items, bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames)
        {
            var repository = GetRepository(site.TableName);

            var query = Q
                .Select(nameof(Content.ChannelId), nameof(Content.Id))
                .Where(nameof(Content.SiteId), site.Id)
                .WhereNot(nameof(Content.SourceId), SourceManager.Preview)
                .Where(nameof(Content.ChannelId), "<", 0)
                .OrderByDesc(nameof(Content.LastModifiedDate));

            if (channelId.HasValue)
            {
                query.Where(nameof(Content.ChannelId), -channelId.Value);
            }

            if (startDate.HasValue)
            {
                query.WhereDate(nameof(Content.LastModifiedDate), ">", startDate.Value);
            }
            if (endDate.HasValue)
            {
                query.WhereDate(nameof(Content.LastModifiedDate), "<", endDate.Value);
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(item.Value))
                    {
                        query.WhereLike(item.Key, $"%{item.Value}%");
                    }
                }
            }

            if (isCheckedLevels)
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

            var total = await repository.CountAsync(query);
            var pageSummaries = await repository.GetAllAsync<ContentSummary>(query.ForPage(page, site.PageSize));
            return (total, pageSummaries);
        }
    }
}
