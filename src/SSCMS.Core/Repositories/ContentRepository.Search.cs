using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task QueryWhereAsync(Query query, Repository<Content> repository, string searchType, string searchText)
        {
            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchText))
            {
                if (StringUtils.EqualsIgnoreCase(nameof(Content.AdminId), searchType))
                {
                    var adminIds = await _administratorRepository.GetAdministratorIdsAsync(searchText);
                    if (adminIds == null || adminIds.Count == 0)
                    {
                        query.Where(nameof(Content.LastEditAdminId), -1);
                    }
                    else
                    {
                        query.WhereIn(nameof(Content.AdminId), adminIds);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(Content.LastEditAdminId), searchType))
                {
                    var adminIds = await _administratorRepository.GetAdministratorIdsAsync(searchText);
                    if (adminIds == null || adminIds.Count == 0)
                    {
                        query.Where(nameof(Content.LastEditAdminId), -1);
                    }
                    else
                    {
                        query.WhereIn(nameof(Content.LastEditAdminId), adminIds);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(Content.UserId), searchType))
                {
                    var userIds = await _userRepository.GetUserIdsAsync(searchText);
                    if (userIds == null || userIds.Count == 0)
                    {
                        query.Where(nameof(Content.UserId), -1);
                    }
                    else
                    {
                        query.WhereIn(nameof(Content.UserId), userIds);
                    }
                }
                else if (repository.TableColumns.Exists(x => StringUtils.EqualsIgnoreCase(x.AttributeName, searchType)))
                {
                    query.WhereLike(searchType, $"%{searchText}%");
                }
                else
                {
                    query.WhereLike(AttrExtendValues, $@"%""{searchType}"":""%{searchText}%""%");
                }
            }
        }

        public async Task<List<ContentSummary>> SearchAsync(Site site, Channel channel, bool isAllContents, string searchType, string searchText, bool isAdvanced, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames)
        {
            var repository = await GetRepositoryAsync(site, channel);
            var query = Q.Select(
              nameof(Content.Id), 
              nameof(Content.ChannelId), 
              nameof(Content.Checked), 
              nameof(Content.CheckedLevel)
            );

            await QueryWhereAsync(query, site, channel.Id, isAllContents);
            await QueryWhereAsync(query, repository, searchType, searchText);

            if (isAdvanced)
            {
                if (checkedLevels != null && checkedLevels.Count > 0)
                {
                    if (!checkedLevels.Contains(site.CheckContentLevel))
                    {
                        query.Where(nameof(Content.Checked), false);
                        query.WhereIn(nameof(Content.CheckedLevel), checkedLevels);
                    }
                    else if (checkedLevels.Count == 1 && checkedLevels.Contains(site.CheckContentLevel))
                    {
                        query.Where(nameof(Content.Checked), true);
                    }
                    else
                    {
                        query.WhereIn(nameof(Content.CheckedLevel), checkedLevels);
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
            var repository = await GetRepositoryAsync(site.TableName);

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

            query.OrderByDesc(nameof(Content.AddDate), nameof(Content.Id));

            var total = await repository.CountAsync(query);
            var pageSummaries = await repository.GetAllAsync<ContentSummary>(query.ForPage(page, site.PageSize));
            return (total, pageSummaries);
        }

        public async Task<(int Total, List<ContentSummary> PageSummaries)> AdvancedSearchAsync(Site site, int page, List<int> channelIds, bool isAllContents, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items, bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames, bool isAdmin, int adminId, bool isUser)
        {
            var repository = await GetRepositoryAsync(site.TableName);

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
                query.Where(nameof(Content.AddDate), ">=", DateUtils.ToString(startDate));
            }
            if (endDate.HasValue)
            {
                query.Where(nameof(Content.AddDate), "<=", DateUtils.ToString(endDate));
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    await QueryWhereAsync(query, repository, item.Key, item.Value);
                }
            }

            if (isCheckedLevels)
            {
                if (checkedLevels != null && checkedLevels.Count > 0)
                {
                    if (!checkedLevels.Contains(site.CheckContentLevel))
                    {
                        query.Where(nameof(Content.Checked), false);
                        query.WhereIn(nameof(Content.CheckedLevel), checkedLevels);
                    }
                    else if (checkedLevels.Count == 1 && checkedLevels.Contains(site.CheckContentLevel))
                    {
                        query.Where(nameof(Content.Checked), true);
                    }
                    else
                    {
                        query.WhereIn(nameof(Content.CheckedLevel), checkedLevels);
                    }
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

        public async Task<(int Total, List<ContentSummary> PageSummaries)> CheckSearchAsync(Site site, int page, List<int> channelIds, bool isAllContents, DateTime? startDate, DateTime? endDate, IEnumerable<KeyValuePair<string, string>> items, bool isCheckedLevels, List<int> checkedLevels, bool isTop, bool isRecommend, bool isHot, bool isColor, List<string> groupNames, List<string> tagNames)
        {
            var repository = await GetRepositoryAsync(site.TableName);

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
                .WhereIn(nameof(Content.ChannelId), idList.Distinct())
                .WhereNullOrFalse(nameof(Content.Checked))
                .OrderByDesc(nameof(Content.AddDate));

            if (startDate.HasValue)
            {
                query.Where(nameof(Content.AddDate), ">=", DateUtils.ToString(startDate));
            }
            if (endDate.HasValue)
            {
                query.Where(nameof(Content.AddDate), "<=", DateUtils.ToString(endDate));
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    await QueryWhereAsync(query, repository, item.Key, item.Value);
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
            var repository = await GetRepositoryAsync(site.TableName);

            var query = Q
                .Select(nameof(Content.ChannelId), nameof(Content.Id))
                .Where(nameof(Content.SiteId), site.Id)
                .WhereNot(nameof(Content.SourceId), SourceManager.Preview)
                .Where(nameof(Content.ChannelId), "<", 0)
                .OrderByDesc(nameof(Content.LastModifiedDate));

            if (channelId.HasValue && channelId != site.Id)
            {
                query.Where(nameof(Content.ChannelId), -channelId.Value);
            }

            if (startDate.HasValue)
            {
                query.Where(nameof(Content.LastModifiedDate), ">=", DateUtils.ToString(startDate));
            }
            if (endDate.HasValue)
            {
                query.Where(nameof(Content.LastModifiedDate), "<=", DateUtils.ToString(endDate));
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    await QueryWhereAsync(query, repository, item.Key, item.Value);
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
