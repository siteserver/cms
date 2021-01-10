using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Datory;
using SqlKata;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task<int> GetMaxTaxisAsync(Site site, Channel channel, bool isTop)
        {
            var repository = GetRepository(site, channel);

            var maxTaxis = 0;
            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                var max = await repository.MaxAsync(nameof(Content.Taxis),
                    GetQuery(site.Id, channel.Id)
                        .Where(nameof(Content.Taxis), ">", TaxisIsTopStartValue)
                );
                if (max.HasValue)
                {
                    maxTaxis = max.Value;
                }

                if (maxTaxis < TaxisIsTopStartValue)
                {
                    maxTaxis = TaxisIsTopStartValue;
                }
            }
            else
            {
                var max = await repository.MaxAsync(nameof(Content.Taxis),
                    GetQuery(site.Id, channel.Id)
                    .Where(nameof(Content.Taxis), "<", TaxisIsTopStartValue)
                );
                if (max.HasValue)
                {
                    maxTaxis = max.Value;
                }
            }
            return maxTaxis;
        }

        private async Task<List<ContentSummary>> GetReferenceIdListAsync(string tableName, IEnumerable<int> contentIdList)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<ContentSummary>(Q
                .Select(nameof(Content.Id), nameof(Content.ChannelId))
                .Where(nameof(Content.ChannelId), ">", 0)
                .WhereIn(nameof(Content.ReferenceId), contentIdList)
            );
        }

        public async Task<int> GetFirstContentIdAsync(Site site, IChannelSummary channel)
        {
            var repository = GetRepository(site, channel);
            return await repository.GetAsync<int>(Q
                .Select(nameof(Content.Id))
                .Where(nameof(Content.ChannelId), channel.Id)
                .OrderByDesc(nameof(Content.Taxis), nameof(Content.Id))
            );
        }

        public List<(int AdminId, int AddCount, int UpdateCount)> GetDataSetOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = $@"select adminId,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
SELECT AdminId as adminId, Count(AdminId) as addCount, 0 as updateCount FROM {tableName} 
INNER JOIN {_administratorRepository.TableName} ON AdminId = {_administratorRepository.TableName}.Id 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastModifiedDate BETWEEN {GetComparableDate(begin)} AND {GetComparableDate(end.AddDays(1))}
GROUP BY AdminId
Union
SELECT LastEditAdminId as lastEditAdminId,0 as addCount, Count(LastEditAdminId) as updateCount FROM {tableName} 
INNER JOIN {_administratorRepository.TableName} ON LastEditAdminId = {_administratorRepository.TableName}.Id 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastModifiedDate BETWEEN {GetComparableDate(begin)} AND {GetComparableDate(end.AddDays(1))}
AND LastModifiedDate != AddDate
GROUP BY LastEditAdminId
) as tmp
group by tmp.adminId";

            var list = new List<(int AdminId, int AddCount, int UpdateCount)>();

            var repository = GetRepository(tableName);
            using (var connection = repository.Database.GetConnection())
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var adminId = rdr.IsDBNull(0) ? 0 : rdr.GetInt32(0);
                        var addCount = rdr.IsDBNull(1) ? 0 : rdr.GetInt32(1);
                        var updateCount = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2);

                        list.Add((adminId, addCount, updateCount));
                    }
                }
            }

            return list;
        }

        public async Task<int> GetCountOfContentUpdateAsync(string tableName, int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, int adminId)
        {
            var channelIdList = await _channelRepository.GetChannelIdsAsync(siteId, channelId, scope);
            return await GetCountOfContentUpdateAsync(tableName, siteId, channelIdList, begin, end, adminId);
        }

        private async Task<int> GetCountOfContentUpdateAsync(string tableName, int siteId, List<int> channelIdList, DateTime begin, DateTime end, int adminId)
        {
            var repository = GetRepository(tableName);
            var query = Q.Where(nameof(Content.SiteId), siteId);
            query.WhereIn(nameof(Content.ChannelId), channelIdList);
            query.WhereBetween(nameof(Content.LastModifiedDate), begin, end.AddDays(1));
            query.WhereRaw($"{nameof(Content.LastModifiedDate)} != {nameof(Content.AddDate)}");
            if (adminId > 0)
            {
                query.Where(nameof(Content.AdminId), adminId);
            }

            return await repository.CountAsync(query);
        }

        public async Task<List<int>> GetContentIdsBySameTitleAsync(Site site, Channel channel, string title)
        {
            var repository = GetRepository(site, channel);

            return await repository.GetAllAsync<int>(GetQuery(site.Id, channel.Id)
                .Select(nameof(Content.Id))
                .Where(nameof(Content.Title), title)
            );
        }

        public async Task<int> GetCountOfContentAddAsync(string tableName, int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, int adminId, bool? checkedState)
        {
            var channelIdList = await _channelRepository.GetChannelIdsAsync(siteId, channelId, scope);
            return await GetCountOfContentAddAsync(tableName, siteId, channelIdList, begin, end, adminId, checkedState);
        }

        private async Task<int> GetCountOfContentAddAsync(string tableName, int siteId, List<int> channelIdList, DateTime begin, DateTime end, int adminId, bool? checkedState)
        {
            var repository = GetRepository(tableName);

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

        public async Task<List<ContentSummary>> GetSummariesAsync(string tableName, Query query)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<ContentSummary>(query);
        }

        public async Task<int> GetCountAsync(string tableName, Query query)
        {
            var repository = GetRepository(tableName);
            return await repository.CountAsync(query);
        }

        public async Task<Query> GetQueryByStlSearchAsync(IDatabaseManager databaseManager, bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes, NameValueCollection form)
        {
            var query = Q.NewQuery();

            Site site = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                site = await _siteRepository.GetSiteBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                site = await _siteRepository.GetSiteByDirectoryAsync(siteDir);
            }
            if (site == null)
            {
                site = await _siteRepository.GetAsync(siteId);
            }

            var channelId = await _channelRepository.GetChannelIdAsync(siteId, siteId, channelIndex, channelName);
            var channel = await _channelRepository.GetAsync(channelId);

            if (isAllSites)
            {
                query.Where(nameof(Content.SiteId), ">", 0);
            }
            else if (!string.IsNullOrEmpty(siteIds))
            {
                query.WhereIn(nameof(Content.SiteId), ListUtils.GetIntList(siteIds));
            }
            else
            {
                query.Where(nameof(Content.SiteId), site.Id);
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                var channelIdList = new List<int>();
                foreach (var theChannelId in ListUtils.GetIntList(channelIds))
                {
                    var theChannel = await _channelRepository.GetAsync(theChannelId);
                    channelIdList.AddRange(
                        await _channelRepository.GetChannelIdsAsync(theChannel.SiteId, theChannel.Id, ScopeType.All));
                }

                if (channelIdList.Count == 1)
                {
                    query.Where(nameof(Content.ChannelId), channelIdList[0]);
                }
                else
                {
                    query.WhereIn(nameof(Content.ChannelId), channelIdList);
                }
            }
            else if (channelId != siteId)
            {
                var channelIdList = await _channelRepository.GetChannelIdsAsync(siteId, channelId, ScopeType.All);

                if (channelIdList.Count == 1)
                {
                    query.Where(nameof(Content.ChannelId), channelIdList[0]);
                }
                else
                {
                    query.WhereIn(nameof(Content.ChannelId), channelIdList);
                }
            }

            var typeList = new List<string>();
            if (string.IsNullOrEmpty(type))
            {
                typeList.Add(nameof(Content.Title));
            }
            else
            {
                typeList = ListUtils.GetStringList(type);
            }

            var tableName = _channelRepository.GetTableName(site, channel);
            var columns = await databaseManager.GetTableColumnInfoListAsync(tableName, excludeAttributes);

            if (!string.IsNullOrEmpty(word))
            {
                query.Where( q =>
                    {
                        foreach (var attributeName in typeList)
                        {
                            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ListInfo.Tags)) || StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.TagNames)))
                            {
                                q
                                    //.OrWhere(nameof(Content.TagNames), word)
                                    .OrWhereLike(nameof(Content.TagNames), $"%{word}%");
                                //.OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $",{word}")
                                //.OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $",{word},")
                                //.OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $"{word},");
                            }
                            else
                            {
                                var column = columns.FirstOrDefault(x =>
                                    StringUtils.EqualsIgnoreCase(x.AttributeName, attributeName));

                                if (column != null && (column.DataType == DataType.VarChar || column.DataType == DataType.Text))
                                {
                                    q.OrWhereLike(column.AttributeName, $"%{word}%");
                                }
                                //else
                                //{
                                //    q.OrWhereLike(AttrExtendValues, $"%{attributeName}={word}%");
                                //}
                            }
                        }

                        return q;
                    }
                );
            }

            if (string.IsNullOrEmpty(dateAttribute))
            {
                dateAttribute = nameof(Content.AddDate);
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                query.WhereDate(dateAttribute, ">=", dateFrom);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                query.WhereDate(dateAttribute, "<=", dateTo);
            }
            if (!string.IsNullOrEmpty(since))
            {
                var sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));

                query.WhereBetween(dateAttribute, sinceDate, DateTime.Now);
            }

            
            //var styleInfoList = RelatedIdentities.GetTableStyleInfoList(site, channel.Id);

            foreach (string attributeName in form.Keys)
            {
                if (ListUtils.ContainsIgnoreCase(excludeAttributes, attributeName)) continue;
                if (string.IsNullOrEmpty(form[attributeName])) continue;

                var value = StringUtils.Trim(form[attributeName]);
                if (string.IsNullOrEmpty(value)) continue;

                if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ListInfo.Tags)) || StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.TagNames)))
                {
                    query.Where(q => q
                        .Where(nameof(Content.TagNames), word)
                        .OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $",{word}")
                        .OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $",{word},")
                        .OrWhereInStr(Database.DatabaseType, nameof(Content.TagNames), $"{word},"));
                }
                else
                {
                    var column = columns.FirstOrDefault(x =>
                        StringUtils.EqualsIgnoreCase(x.AttributeName, attributeName));

                    if (column != null && (column.DataType == DataType.VarChar || column.DataType == DataType.Text))
                    {
                        query.WhereLike(column.AttributeName, $"%{value}%");
                    }
                    //else
                    //{
                    //    query.WhereLike(AttrExtendValues, $"%{attributeName}={value}%");
                    //}
                }
            }

            return query;
        }

        public async Task<string> GetNewContentTableNameAsync()
        {
            var name = DateTime.Now.ToString("yyyyMMdd");

            var i = 1;
            do
            {
                var tableName = $"siteserver_{name}_{i++}";
                if (!await _settingsManager.Database.IsTableExistsAsync(tableName))
                {
                    return tableName;
                }
            } while (true);
        }

        public async Task<string> CreateNewContentTableAsync()
        {
            var tableName = await GetNewContentTableNameAsync();
            var repository = new Repository<Content>(_settingsManager.Database);
            await CreateContentTableAsync(tableName, repository.TableColumns);
            return tableName;
        }

        public async Task CreateContentTableAsync(string tableName, List<TableColumn> columnInfoList)
        {
            var isDbExists = await _settingsManager.Database.IsTableExistsAsync(tableName);
            if (isDbExists) return;

            await _settingsManager.Database.CreateTableAsync(tableName, columnInfoList);
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.Taxis)}", $"{nameof(Content.Taxis)} DESC");
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.SiteId)}", $"{nameof(Content.SiteId)} DESC");
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.ChannelId)}", $"{nameof(Content.ChannelId)} DESC");
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.AddDate)}", $"{nameof(Content.AddDate)} DESC");
            await _settingsManager.Database.CreateIndexAsync(tableName, $"IX_{tableName}_{nameof(Content.SourceId)}", $"{nameof(Content.SourceId)} DESC");
        }

        private async Task QueryWhereAsync(Query query, Site site, int channelId, bool isAllContents)
        {
            query.Where(nameof(Content.SiteId), site.Id);
            query.WhereNot(nameof(Content.SourceId), SourceManager.Preview);

            if (isAllContents)
            {
                var channelIdList = await _channelRepository.GetChannelIdsAsync(site.Id, channelId, ScopeType.All);
                query.WhereIn(nameof(Content.ChannelId), channelIdList);
            }
            else
            {
                query.Where(nameof(Content.ChannelId), channelId);
            }
        }
    }
}
