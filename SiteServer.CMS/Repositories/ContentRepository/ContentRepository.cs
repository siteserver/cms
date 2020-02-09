using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SqlKata;
using TaxisType = SiteServer.Abstractions.TaxisType;
using ETaxisTypeUtils = SiteServer.Abstractions.ETaxisTypeUtils;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository : DataProviderBase
    {
        public async Task UpdateIsCheckedAsync(Site site, Channel channel, IEnumerable<int> contentIdList, int translateChannelId, int adminId, bool isChecked, int checkedLevel, string reasons)
        {
            if (isChecked)
            {
                checkedLevel = 0;
            }

            var checkDate = DateTime.Now;

            var repository = await GetRepositoryAsync(site, channel);

            foreach (var contentId in contentIdList)
            {
                var content = await GetAsync(site, channel, contentId);

                content.Checked = isChecked;
                content.CheckedLevel = checkedLevel;

                content.CheckAdminId = adminId;
                content.CheckDate = checkDate;
                content.CheckReasons = reasons;

                if (translateChannelId > 0)
                {
                    content.ChannelId = translateChannelId;
                }

                await UpdateAsync(site, channel, content);

                await DataProvider.ContentCheckRepository.InsertAsync(new ContentCheck
                {
                    TableName = repository.TableName,
                    SiteId = site.Id,
                    ChannelId = channel.Id,
                    ContentId = contentId,
                    AdminId = adminId,
                    Checked = isChecked,
                    CheckedLevel = checkedLevel,
                    CheckDate = checkDate,
                    Reasons = reasons
                });
            }
        }

        public async Task SetAutoPageContentToSiteAsync(Site site)
        {
            if (!site.IsAutoPageInTextEditor) return;

            var tableNames = await DataProvider.SiteRepository.GetAllTableNameListAsync();
            foreach (var tableName in tableNames)
            {
                var repository = GetRepository(tableName);

                var list = await repository.GetAllAsync<(int ContentId, string Content)>(
                    GetQuery(site.Id)
                    .Select(ContentAttribute.Id, ContentAttribute.Content)
                );

                foreach (var (contentId, contentValue) in list)
                {
                    var content = ContentUtility.GetAutoPageContent(contentValue, site.AutoPageWordNum);
                    await repository.UpdateAsync(
                        GetQuery(site.Id)
                        .Set(ContentAttribute.Content, content)
                        .Where(ContentAttribute.Id, contentId)
                        .CachingRemove(GetEntityKey(tableName, contentId))
                    );
                }
            }
        }

        public async Task RecycleContentsAsync(Site site, Channel channel, List<int> contentIdList)
        {
            if (contentIdList == null || contentIdList.Count == 0) return;

            var repository = await GetRepositoryAsync(site, channel);

            var cacheKeys = new List<string>
            {
                GetCountKey(repository.TableName, site.Id, channel.Id),
                GetListKey(repository.TableName, channel.IsAllContents, site.Id, channel.Id)
            };
            foreach (var contentId in contentIdList)
            {
                cacheKeys.Add(GetEntityKey(repository.TableName, contentId));
            }

            var referenceSummaries = await GetReferenceIdListAsync(repository.TableName, contentIdList);
            if (referenceSummaries.Count > 0)
            {
                foreach (var referenceSummary in referenceSummaries)
                {
                    await DeleteReferenceContentsAsync(site, referenceSummary);
                }
            }

            await repository.UpdateAsync(
                GetQuery(site.Id)
                    .SetRaw("ChannelId = -ChannelId")
                    .WhereIn(ContentAttribute.Id, contentIdList)
                    .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task RecycleContentsAsync(Site site, Channel channel)
        {
            var contentIds = await GetContentIdsAsync(site, channel);
            await RecycleContentsAsync(site, channel, contentIds);
        }

        public async Task UpdateRestoreContentsByTrashAsync(int siteId, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var repository = GetRepository(tableName);
            await repository.UpdateAsync(
                GetQuery(siteId)
                .SetRaw("ChannelId = -ChannelId")
                .Where(ContentAttribute.ChannelId, "<", 0)
            );
        }

        public async Task DeleteAsync(Site site, Channel channel, int contentId)
        {
            if (site == null || channel == null || contentId <= 0) return;

            var repository = await GetRepositoryAsync(site, channel);

            await repository.DeleteAsync(contentId, Q
                .CachingRemove(GetCountKey(repository.TableName, site.Id, channel.Id))
                .CachingRemove(GetEntityKey(repository.TableName, contentId))
                .CachingRemove(GetListKey(repository.TableName, channel.IsAllContents, site.Id, channel.Id))
            );

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnContentDeleteCompleted(new ContentEventArgs(site.Id, channel.Id, contentId));
                }
                catch (Exception ex)
                {
                    await LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentDeleteCompleted));
                }
            }
        }

        private async Task DeleteReferenceContentsAsync(Site site, ContentSummary summary)
        {
            var channel = await DataProvider.ChannelRepository.GetAsync(summary.ChannelId);
            var repository = await GetRepositoryAsync(site, channel);

            await repository.DeleteAsync(
                GetQuery(site.Id, channel.Id)
                    .Where(ContentAttribute.ReferenceId, ">", 0)
                    .Where(ContentAttribute.Id, summary.Id)
                    .CachingRemove(
                        GetCountKey(repository.TableName, site.Id, channel.Id),
                        GetListKey(repository.TableName, channel.IsAllContents, site.Id, channel.Id),
                        GetEntityKey(repository.TableName, summary.Id)
                    )
            );
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

            var repository = await GetRepositoryAsync(site, channel);
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
                GetListKey(repository.TableName, true, site.Id, channel.Id),
                GetListKey(repository.TableName, false, site.Id, channel.Id)
            );
        }

        public async Task<bool> SetTaxisToUpAsync(Site site, Channel channel, int contentId, bool isTop)
        {
            var repository = await GetRepositoryAsync(site, channel);

            var taxis = await repository.GetAsync<int>(
                GetQuery(site.Id, channel.Id)
                .Select(ContentAttribute.Taxis)
                .Where(ContentAttribute.Id, contentId)
            );

            var result = await repository.GetAsync<Content>(
                GetQuery(site.Id, channel.Id)
                .Select(ContentAttribute.Id, ContentAttribute.Taxis)
                .Where(ContentAttribute.Taxis, ">", taxis)
                .Where(ContentAttribute.Taxis, isTop ? ">" : "<", TaxisIsTopStartValue)
                .OrderBy(ContentAttribute.Taxis));

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
                .Set(ContentAttribute.Taxis, higherTaxis)
                .Where(ContentAttribute.Id, contentId)
            );

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                .Set(ContentAttribute.Taxis, taxis)
                .Where(ContentAttribute.Id, higherId)
            );

            await repository.RemoveCacheAsync(
                GetEntityKey(repository.TableName, contentId),
                GetEntityKey(repository.TableName, higherId),
                GetListKey(repository.TableName, channel.IsAllContents, site.Id, channel.Id)
            );

            return true;
        }

        public async Task<bool> SetTaxisToDownAsync(Site site, Channel channel, int contentId, bool isTop)
        {
            var repository = await GetRepositoryAsync(site, channel);

            var taxis = await repository.GetAsync<int>(
                GetQuery(site.Id, channel.Id)
                .Select(ContentAttribute.Taxis)
                .Where(ContentAttribute.Id, contentId)
            );

            var result = await repository.GetAsync<Content>(
                GetQuery(site.Id, channel.Id)
                .Select(ContentAttribute.Id, ContentAttribute.Taxis)
                .Where(ContentAttribute.Taxis, "<", taxis)
                .Where(ContentAttribute.Taxis, isTop ? ">" : "<", TaxisIsTopStartValue)
                .OrderByDesc(ContentAttribute.Taxis));

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
                .Set(ContentAttribute.Taxis, lowerTaxis)
                .Where(ContentAttribute.Id, contentId)
            );

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                .Set(ContentAttribute.Taxis, taxis)
                .Where(ContentAttribute.Id, lowerId)
            );

            await repository.RemoveCacheAsync(
                GetEntityKey(repository.TableName, contentId),
                GetEntityKey(repository.TableName, lowerId),
                GetListKey(repository.TableName, channel.IsAllContents, site.Id, channel.Id)
            );

            return true;
        }

        public async Task<int> GetMaxTaxisAsync(Site site, Channel channel, bool isTop)
        {
            var repository = await GetRepositoryAsync(site, channel);

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

        public async Task AddContentGroupListAsync(string tableName, int contentId, List<string> contentGroupList)
        {
            var repository = GetRepository(tableName);

            var content = await GetAsync(tableName, contentId);

            if (content != null)
            {
                var list = content.GroupNames;
                foreach (var groupName in contentGroupList)
                {
                    if (!list.Contains(groupName)) list.Add(groupName);
                }

                content.GroupNames = list;
                
                await repository.UpdateAsync(Q
                    .Set(nameof(Content.GroupNames), Utilities.ToString(content.GroupNames))
                    .Where(nameof(Content.Id), contentId)
                    .CachingRemove(GetEntityKey(tableName, contentId))
                );
            }
        }

        public async Task AddDownloadsAsync(string tableName, int channelId, int contentId)
        {
            var repository = GetRepository(tableName);

            await repository.IncrementAsync(ContentAttribute.Downloads, Q
                .Where(ContentAttribute.Id, contentId)
                .CachingRemove(GetEntityKey(tableName, contentId))
            );
        }

        private async Task<List<ContentSummary>> GetReferenceIdListAsync(string tableName, IEnumerable<int> contentIdList)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<ContentSummary>(Q
                .Select(ContentAttribute.Id, ContentAttribute.ChannelId)
                .Where(ContentAttribute.ChannelId, ">", 0)
                .WhereIn(ContentAttribute.ReferenceId, contentIdList)
            );
        }

        public async Task<int> GetFirstContentIdAsync(string tableName, int channelId)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAsync<int>(Q
                .Select(ContentAttribute.Id)
                .Where(nameof(Content.ChannelId), channelId)
                .OrderByDesc(ContentAttribute.Taxis, ContentAttribute.Id)
            );
        }

        public async Task<List<int>> GetContentIdListAsync(string tableName, int channelId)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<int>(Q
                .Select(ContentAttribute.Id)
                .Where(ContentAttribute.ChannelId, channelId)
            );
        }

        public async Task<List<int>> GetContentIdListAsync(string tableName, int channelId, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var repository = GetRepository(tableName);
            var query = Q
                .Select(ContentAttribute.Id)
                .Where(ContentAttribute.ChannelId, channelId)
                .OrderByDesc(ContentAttribute.Taxis, ContentAttribute.Id);

            if (isPeriods)
            {
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    query.WhereDate(ContentAttribute.AddDate, ">=", TranslateUtils.ToDateTime(dateFrom));
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    query.WhereDate(ContentAttribute.AddDate, "<=", TranslateUtils.ToDateTime(dateTo).AddDays(1));
                }
            }

            if (checkedState != ETriState.All)
            {
                query.Where(ContentAttribute.Checked, TranslateUtils.ToBool(ETriStateUtils.GetValue(checkedState)));
            }

            return await repository.GetAllAsync<int>(query);
        }

        public async Task<List<int>> GetContentIdListCheckedByChannelIdAsync(string tableName, int siteId, int channelId)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<int>(
                GetQuery(siteId, channelId)
                .Select(ContentAttribute.Id)
                .Where(ContentAttribute.Checked, true)
            );
        }

        public async Task<List<string>> GetValueListByStartStringAsync(Site site, Channel channel, string name, string startString, int totalNum)
        {
            var repository = await GetRepositoryAsync(site, channel);
            var list = await repository.GetAllAsync<string>(
                GetQuery(site.Id, channel.Id)
                .Select(name)
                .WhereInStr(WebConfigUtils.DatabaseType, name, startString)
                .Distinct()
                .Limit(totalNum)
            );
            return list.ToList();
        }

        public List<int> GetChannelIdListCheckedByLastEditDateHour(string tableName, int siteId, int hour)
        {
            var list = new List<int>();

            var sqlString =
                $"SELECT DISTINCT ChannelId FROM {tableName} WHERE (SiteId = {siteId}) AND (IsChecked = '{true}') AND (LastEditDate BETWEEN {SqlUtils.GetComparableDateTime(DateTime.Now.AddHours(-hour))} AND {SqlUtils.GetComparableNow()})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var channelId = GetInt(rdr, 0);
                    list.Add(channelId);
                }
                rdr.Close();
            }
            return list;
        }

        public DataRowCollection GetDataSetOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = $@"select userName,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
SELECT AddUserName as userName, Count(AddUserName) as addCount, 0 as updateCount FROM {tableName} 
INNER JOIN {DataProvider.AdministratorRepository.TableName} ON AddUserName = {DataProvider.AdministratorRepository.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
GROUP BY AddUserName
Union
SELECT LastEditUserName as userName,0 as addCount, Count(LastEditUserName) as updateCount FROM {tableName} 
INNER JOIN {DataProvider.AdministratorRepository.TableName} ON LastEditUserName = {DataProvider.AdministratorRepository.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
AND LastEditDate != AddDate
GROUP BY LastEditUserName
) as tmp
group by tmp.userName";

            var ds = ExecuteDataset(sqlString);

            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows;
                }
            }

            return null;
        }

        public async Task UpdateAsync(Site site, Channel channel, Content content)
        {
            if (content == null) return;

            if (site.IsAutoPageInTextEditor &&
                content.ContainsKey(ContentAttribute.Content))
            {
                content.Set(ContentAttribute.Content,
                    ContentUtility.GetAutoPageContent(content.Get<string>(ContentAttribute.Content),
                        site.AutoPageWordNum));
            }

            //出现IsTop与Taxis不同步情况
            if (content.Top == false && content.Taxis >= TaxisIsTopStartValue)
            {
                content.Taxis = await GetMaxTaxisAsync(site, channel, false) + 1;
            }
            else if (content.Top && content.Taxis < TaxisIsTopStartValue)
            {
                content.Taxis = await GetMaxTaxisAsync(site, channel, true) + 1;
            }

            content.LastEditDate = DateTime.Now;

            var repository = await GetRepositoryAsync(site, channel);
            await repository.UpdateAsync(content, Q
                .CachingRemove(GetListKey(repository.TableName, channel.IsAllContents, content.SiteId, content.ChannelId))
                .CachingRemove(GetEntityKey(repository.TableName, content.Id))
            );
        }

        public async Task UpdateAsync(Site site, Channel channel, int contentId, string name, string value)
        {
            var repository = await GetRepositoryAsync(site, channel);

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                    .Set(name, value)
                    .Where(ContentAttribute.Id, contentId)
                    .CachingRemove(GetEntityKey(repository.TableName, contentId))
                    .CachingRemove(GetListKey(repository.TableName, channel.IsAllContents, site.Id, channel.Id))
            );
        }

        public async Task<int> GetCountOfContentUpdateAsync(string tableName, int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName)
        {
            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channelInfo, scope);
            return GetCountOfContentUpdate(tableName, siteId, channelIdList, begin, end, userName);
        }

        private int GetCountOfContentUpdate(string tableName, int siteId, List<int> channelIdList, DateTime begin, DateTime end, string userName)
        {
            string sqlString;
            if (string.IsNullOrEmpty(userName))
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate)"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate)";
            }
            else
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate) AND (AddUserName = '{userName}')"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate) AND (AddUserName = '{userName}')";
            }

            return DataProvider.DatabaseRepository.GetIntResult(sqlString);
        }

        public List<int> GetIdListBySameTitle(string tableName, int channelId, string title)
        {
            var list = new List<int>();
            var sql = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} AND Title = '{title}'";
            using (var rdr = ExecuteReader(sql))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public int GetCount(string tableName, string whereString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = whereString.Replace("WHERE ", string.Empty).Replace("where ", string.Empty);
            }
            whereString = string.IsNullOrEmpty(whereString) ? string.Empty : $"WHERE {whereString}";

            var sqlString = $"SELECT COUNT(*) FROM {tableName} {whereString}";

            return DataProvider.DatabaseRepository.GetIntResult(sqlString);
        }

        public async Task<List<(int, int)>> GetContentIdListByTrashAsync(int siteId, string tableName)
        {
            var repository = GetRepository(tableName);
            var list = await repository.GetAllAsync<(int Id, int ChannelId)>(
                GetQuery(siteId)
                    .Select(nameof(Content.Id), nameof(Content.ChannelId))
                    .Where(nameof(Content.ChannelId), "<", 0)
            );

            return list.Select(o => (Math.Abs(o.ChannelId), o.Id)).ToList();
        }

        private async Task<DataSet> GetStlDataSourceCheckedAsync(string tableName, List<int> channelIdList, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }
            var sqlWhereString = channelIdList.Count == 1 ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})";

            if (others != null && others.Count > 0)
            {
                var columnNameList = await TableColumnManager.GetTableColumnNameListAsync(tableName);

                foreach (var attributeName in others.AllKeys)
                {
                    if (StringUtils.ContainsIgnoreCase(columnNameList, attributeName))
                    {
                        var value = others.Get(attributeName);
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.Trim();
                            if (StringUtils.StartsWithIgnoreCase(value, "not:"))
                            {
                                value = value.Substring("not:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} <> '{value}')";
                                }
                                else
                                {
                                    var collection = Utilities.GetStringList(value);
                                    foreach (var val in collection)
                                    {
                                        sqlWhereString += $" AND ({attributeName} <> '{val}')";
                                    }
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "contains:"))
                            {
                                value = value.Substring("contains:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '%{value}%')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = Utilities.GetStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '%{val}%' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "start:"))
                            {
                                value = value.Substring("start:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '{value}%')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = Utilities.GetStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '{val}%' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "end:"))
                            {
                                value = value.Substring("end:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '%{value}')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = Utilities.GetStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '%{val}' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else
                            {
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} = '{value}')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = Utilities.GetStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} = '{val}' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                        }
                    }
                }
            }

            return startNum <= 1 ? GetStlDataSourceByContentNumAndWhereString(tableName, totalNum, sqlWhereString, orderByString) : GetStlDataSourceByStartNum(tableName, startNum, totalNum, sqlWhereString, orderByString);
        }

        private DataSet GetStlDataSourceByContentNumAndWhereString(string tableName, int totalNum, string whereString, string orderByString)
        {
            DataSet dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlSelect = DataProvider.DatabaseRepository.GetSelectSqlString(tableName, totalNum, MinListColumns, whereString, orderByString);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
        }

        private DataSet GetStlDataSourceByStartNum(string tableName, int startNum, int totalNum, string whereString, string orderByString)
        {
            DataSet dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                //var sqlSelect = DataProvider.DatabaseRepository.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, whereString, orderByString);
                var sqlSelect = DataProvider.DatabaseRepository.GetPageSqlString(tableName, MinListColumns, whereString, orderByString, startNum - 1, totalNum);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
        }

        public async Task<int> GetCountOfContentAddAsync(string tableName, int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            string sqlString;

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channelInfo, scope, string.Empty, string.Empty, string.Empty);

            if (string.IsNullOrEmpty(userName))
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))})"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))})";
            }
            else
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (AddUserName = '{userName}')"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (AddUserName = '{userName}')";
            }

            if (checkedState != ETriState.All)
            {
                sqlString += $" AND {ContentAttribute.Checked} = {ETriStateUtils.GetValue(checkedState).ToLower()}";
            }

            return DataProvider.DatabaseRepository.GetIntResult(sqlString);
        }

        private List<int> GetContentIdListChecked(string tableName, int channelId, int totalNum, string orderByFormatString, string whereString)
        {
            var channelIdList = new List<int>
            {
                channelId
            };
            return GetContentIdListChecked(tableName, channelIdList, totalNum, orderByFormatString, whereString);
        }

        private List<int> GetContentIdListChecked(string tableName, List<int> channelIdList, int totalNum, string orderString, string whereString)
        {
            var list = new List<int>();

            if (channelIdList == null || channelIdList.Count == 0)
            {
                return list;
            }

            string sqlString;

            if (totalNum > 0)
            {
                sqlString = SqlUtils.ToTopSqlString(tableName, "Id",
                    channelIdList.Count == 1
                        ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})"
                        : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})", orderString,
                    totalNum);
            }
            else
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT Id FROM {tableName} WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString}) {orderString}"
                    : $"SELECT Id FROM {tableName} WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString}) {orderString}";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
                    list.Add(contentId);
                }
                rdr.Close();
            }
            return list;
        }

        public List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString)
        {
            return GetContentIdListChecked(tableName, channelId, orderByFormatString, string.Empty);
        }

        private List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString, string whereString)
        {
            return GetContentIdListChecked(tableName, channelId, 0, orderByFormatString, whereString);
        }

        public async Task<List<ContentSummary>> GetChannelContentIdListAsync(string tableName, Query query)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<ContentSummary>(query);
        }

        public async Task<int> GetTotalCountAsync(string tableName, Query query)
        {
            var repository = GetRepository(tableName);
            return await repository.CountAsync(query);
        }

        public async Task<string> GetWhereStringByStlSearchAsync(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes, NameValueCollection form)
        {
            var whereBuilder = new StringBuilder();

            Site site = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                site = await DataProvider.SiteRepository.GetSiteBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                site = await DataProvider.SiteRepository.GetSiteByDirectoryAsync(siteDir);
            }
            if (site == null)
            {
                site = await DataProvider.SiteRepository.GetAsync(siteId);
            }

            var channelId = await DataProvider.ChannelRepository.GetChannelIdAsync(siteId, siteId, channelIndex, channelName);
            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);

            if (isAllSites)
            {
                whereBuilder.Append("(SiteId > 0) ");
            }
            else if (!string.IsNullOrEmpty(siteIds))
            {
                whereBuilder.Append($"(SiteId IN ({TranslateUtils.ToSqlInStringWithoutQuote(Utilities.GetIntList(siteIds))})) ");
            }
            else
            {
                whereBuilder.Append($"(SiteId = {site.Id}) ");
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                whereBuilder.Append(" AND ");
                var channelIdList = new List<int>();
                foreach (var theChannelId in Utilities.GetIntList(channelIds))
                {
                    var theChannel = await DataProvider.ChannelRepository.GetAsync(theChannelId);
                    channelIdList.AddRange(
                        await DataProvider.ChannelRepository.GetChannelIdsAsync(theChannel,
                            EScopeType.All));
                }
                whereBuilder.Append(channelIdList.Count == 1
                    ? $"(ChannelId = {channelIdList[0]}) "
                    : $"(ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");
            }
            else if (channelId != siteId)
            {
                whereBuilder.Append(" AND ");

                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channelInfo, EScopeType.All);

                whereBuilder.Append(channelIdList.Count == 1
                    ? $"(ChannelId = {channelIdList[0]}) "
                    : $"(ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");
            }

            var typeList = new List<string>();
            if (string.IsNullOrEmpty(type))
            {
                typeList.Add(ContentAttribute.Title);
            }
            else
            {
                typeList = Utilities.GetStringList(type);
            }

            if (!string.IsNullOrEmpty(word))
            {
                whereBuilder.Append(" AND (");
                foreach (var attributeName in typeList)
                {
                    whereBuilder.Append($"[{attributeName}] LIKE '%{AttackUtils.FilterSql(word)}%' OR ");
                }
                whereBuilder.Length = whereBuilder.Length - 3;
                whereBuilder.Append(")");
            }

            if (string.IsNullOrEmpty(dateAttribute))
            {
                dateAttribute = ContentAttribute.AddDate;
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" {dateAttribute} >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" {dateAttribute} <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))} ");
            }
            if (!string.IsNullOrEmpty(since))
            {
                var sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
                whereBuilder.Append($" AND {dateAttribute} BETWEEN {SqlUtils.GetComparableDateTime(sinceDate)} AND {SqlUtils.GetComparableNow()} ");
            }

            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelInfo);
            //var styleInfoList = RelatedIdentities.GetTableStyleInfoList(site, channel.Id);

            foreach (string key in form.Keys)
            {
                if (excludeAttributes.Contains(key.ToLower())) continue;
                if (string.IsNullOrEmpty(form[key])) continue;

                var value = StringUtils.Trim(form[key]);
                if (string.IsNullOrEmpty(value)) continue;

                var columnInfo = await TableColumnManager.GetTableColumnInfoAsync(tableName, key);

                if (columnInfo != null && (columnInfo.DataType == DataType.VarChar || columnInfo.DataType == DataType.Text))
                {
                    whereBuilder.Append(" AND ");
                    whereBuilder.Append($"({key} LIKE '%{value}%')");
                }
                //else
                //{
                //    foreach (var tableStyleInfo in styleInfoList)
                //    {
                //        if (StringUtils.EqualsIgnoreCase(tableStyleInfo.AttributeName, key))
                //        {
                //            whereBuilder.Append(" AND ");
                //            whereBuilder.Append($"({ContentAttribute.SettingsXml} LIKE '%{key}={value}%')");
                //            break;
                //        }
                //    }
                //}
            }

            return whereBuilder.ToString();
        }

        public async Task<string> GetSqlStringAsync(string tableName, int siteId, int channelId, bool isSystemAdministrator, List<int> owningChannelIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState, bool isTrashContent)
        {
            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channelInfo,
                isSearchChildren ? EScopeType.All : EScopeType.Self, string.Empty, string.Empty, channelInfo.ContentModelPluginId);

            var list = new List<int>();
            if (isSystemAdministrator)
            {
                list = channelIdList;
            }
            else
            {
                foreach (var theChannelId in channelIdList)
                {
                    if (owningChannelIdList.Contains(theChannelId))
                    {
                        list.Add(theChannelId);
                    }
                }
            }

            return await GetSqlStringByConditionAsync(tableName, siteId, list, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent);
        }

        public string GetSqlStringByContentGroup(string tableName, string contentGroupName, int siteId)
        {
            contentGroupName = AttackUtils.FilterSql(contentGroupName);
            var sqlString =
                $"SELECT * FROM {tableName} WHERE SiteId = {siteId} AND ChannelId > 0 AND (GroupNameCollection LIKE '{contentGroupName},%' OR GroupNameCollection LIKE '%,{contentGroupName}' OR GroupNameCollection  LIKE '%,{contentGroupName},%'  OR GroupNameCollection='{contentGroupName}')";
            return sqlString;
        }

        public string GetSqlStringByContentTag(string tableName, string tag, int siteId)
        {
            tag = AttackUtils.FilterSql(tag);

            var sqlString =
                $"SELECT * FROM {tableName} WHERE SiteId = {siteId} AND ChannelId > 0 AND (Tags LIKE '{tag} %' OR Tags LIKE '% {tag}' OR Tags  LIKE '% {tag} %'  OR Tags='{tag}')";
            return sqlString;
        }

        public async Task<string> GetStlSqlStringCheckedAsync(string tableName, int siteId, int channelId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            string sqlWhereString;

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);

            if (siteId == channelId && scopeType == EScopeType.All && string.IsNullOrEmpty(groupChannel) && string.IsNullOrEmpty(groupChannelNot))
            {
                sqlWhereString =
                    $"WHERE (SiteId = {siteId} AND ChannelId > 0 AND IsChecked = '{true}' {whereString})";
            }
            else
            {
                if (channelIdList == null || channelIdList.Count == 0)
                {
                    return string.Empty;
                }
                sqlWhereString = channelIdList.Count == 1 ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})";
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                //return DataProvider.DatabaseRepository.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, sqlWhereString, orderByString);
                return DataProvider.DatabaseRepository.GetPageSqlString(tableName, MinListColumns, sqlWhereString, orderByString, startNum - 1, totalNum);
            }
            return string.Empty;
        }

        private async Task<string> GetSqlStringByConditionAsync(string tableName, int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isTrashContent)
        {
            return await GetSqlStringByConditionAsync(tableName, siteId, channelIdList, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent, false, 0);
        }

        private async Task<string> GetSqlStringByConditionAsync(string tableName, int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isTrashContent, bool isWritingOnly, int adminIdOnly)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }

            var orderByString = ETaxisTypeUtils.GetContentOrderByString(TaxisType.OrderByTaxisDesc);

            var dateString = string.Empty;
            if (!string.IsNullOrEmpty(dateFrom))
            {
                dateString = $" AND AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ";
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                dateString += $" AND AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))} ";
            }
            var whereString = new StringBuilder($"WHERE {nameof(ContentAttribute.SourceId)} != {SourceManager.Preview} AND ");

            if (isTrashContent)
            {
                for (var i = 0; i < channelIdList.Count; i++)
                {
                    var theChannelId = channelIdList[i];
                    channelIdList[i] = -theChannelId;
                }
            }

            whereString.Append(channelIdList.Count == 1
                ? $"SiteId = {siteId} AND (ChannelId = {channelIdList[0]}) "
                : $"SiteId = {siteId} AND (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");

            if (StringUtils.EqualsIgnoreCase(searchType, nameof(Content.Top)) || StringUtils.EqualsIgnoreCase(searchType, nameof(Content.Recommend)) || StringUtils.EqualsIgnoreCase(searchType, nameof(Content.Color)) || StringUtils.EqualsIgnoreCase(searchType, nameof(Content.Hot)))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereString.Append($"AND ({ContentAttribute.Title} LIKE '%{keyword}%') ");
                }
                whereString.Append($" AND {searchType} = '{true}'");
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                var columnNameList = await TableColumnManager.GetTableColumnNameListAsync(tableName);

                if (StringUtils.ContainsIgnoreCase(columnNameList, searchType))
                {
                    whereString.Append($"AND ({searchType} LIKE '%{keyword}%') ");
                }
            }

            whereString.Append(dateString);

            if (checkedState == ETriState.True)
            {
                whereString.Append("AND IsChecked='True' ");
            }
            else if (checkedState == ETriState.False)
            {
                whereString.Append("AND IsChecked='False' ");
            }

            if (adminIdOnly > 0)
            {
                whereString.Append($" AND {ContentAttribute.AdminId} = {adminIdOnly} ");
            }
            if (isWritingOnly)
            {
                whereString.Append($" AND {ContentAttribute.UserId} > 0 ");
            }

            whereString.Append(" ").Append(orderByString);

            return DataProvider.DatabaseRepository.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public async Task<string> GetPagerWhereSqlStringAsync(Site site, Channel channel, string searchType, string keyword, string dateFrom, string dateTo, int checkLevel, bool isCheckOnly, bool isSelfOnly, bool isTrashOnly, bool isWritingOnly, bool isSuperAdmin, IList<int> owningChannelIdList, List<string> allAttributeNameList)
        {
            var isAllChannels = false;
            var searchChannelIdList = new List<int>();

            if (isSelfOnly)
            {
                searchChannelIdList = new List<int>
                {
                    channel.Id
                };
            }
            else
            {
                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channel, EScopeType.All, string.Empty, string.Empty, channel.ContentModelPluginId);

                if (isSuperAdmin)
                {
                    if (channel.Id == site.Id)
                    {
                        isAllChannels = true;
                    }

                    searchChannelIdList = channelIdList;
                }
                else
                {
                    foreach (var theChannelId in channelIdList)
                    {
                        if (owningChannelIdList.Contains(theChannelId))
                        {
                            searchChannelIdList.Add(theChannelId);
                        }
                    }
                }
            }
            if (isTrashOnly)
            {
                searchChannelIdList = searchChannelIdList.Select(i => -i).ToList();
            }

            var whereList = new List<string>
            {
                $"{nameof(ContentAttribute.SiteId)} = {site.Id}",
                $"{nameof(ContentAttribute.SourceId)} != {SourceManager.Preview}"
            };

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereList.Add($"{nameof(ContentAttribute.AddDate)} >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereList.Add($"{nameof(ContentAttribute.AddDate)} <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))}");
            }

            if (isAllChannels)
            {
                whereList.Add(isTrashOnly
                    ? $"{nameof(ContentAttribute.ChannelId)} < 0"
                    : $"{nameof(ContentAttribute.ChannelId)} > 0");
            }
            else if (searchChannelIdList.Count == 0)
            {
                whereList.Add($"{nameof(ContentAttribute.ChannelId)} = 0");
            }
            else if (searchChannelIdList.Count == 1)
            {
                whereList.Add(isTrashOnly
                    ? $"{nameof(ContentAttribute.ChannelId)} = -{channel.Id}"
                    : $"{nameof(ContentAttribute.ChannelId)} = {channel.Id}");
            }
            else
            {
                whereList.Add($"{nameof(ContentAttribute.ChannelId)} IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchChannelIdList)})");
            }

            if (StringUtils.EqualsIgnoreCase(searchType, nameof(Content.Top)) || StringUtils.EqualsIgnoreCase(searchType, nameof(Content.Recommend)) || StringUtils.EqualsIgnoreCase(searchType, nameof(Content.Color)) || StringUtils.EqualsIgnoreCase(searchType, nameof(Content.Hot)))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereList.Add($"{ContentAttribute.Title} LIKE '%{keyword}%'");
                }
                whereList.Add($"{searchType} = '{true}'");
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                if (StringUtils.ContainsIgnoreCase(allAttributeNameList, searchType))
                {
                    whereList.Add($"{searchType} LIKE '%{keyword}%'");
                }
                //whereList.Add(allLowerAttributeNameList.Contains(searchType.ToLower())
                //    ? $"{searchType} LIKE '%{keyword}%'"
                //    : $"{nameof(ContentAttribute.SettingsXml)} LIKE '%{searchType}={keyword}%'");
            }

            if (isCheckOnly)
            {
                whereList.Add(checkLevel == CheckManager.LevelInt.All
                    ? $"{nameof(Content.Checked)} = false"
                    : $"{nameof(Content.Checked)} = false AND {nameof(ContentAttribute.CheckedLevel)} = {checkLevel}");
            }
            else
            {
                if (checkLevel != CheckManager.LevelInt.All)
                {
                    whereList.Add(checkLevel == site.CheckContentLevel
                        ? $"{nameof(Content.Checked)} = true"
                        : $"{nameof(Content.Checked)} = false AND {nameof(ContentAttribute.CheckedLevel)} = {checkLevel}");
                }
            }

            if (isWritingOnly)
            {
                whereList.Add($"{nameof(ContentAttribute.UserId)} > 0");
            }

            return $"WHERE {Utilities.ToString(whereList, " AND ")}";
        }

        public async Task CreateContentTableAsync(string tableName, List<TableColumn> columnInfoList)
        {
            var isDbExists = await WebConfigUtils.Database.IsTableExistsAsync(tableName);
            if (isDbExists) return;

            await WebConfigUtils.Database.CreateTableAsync(tableName, columnInfoList);
            await WebConfigUtils.Database.CreateIndexAsync(tableName, $"IX_{tableName}",
                $"{nameof(Content.Top)} DESC", $"{ContentAttribute.Taxis} DESC", $"{ContentAttribute.Id} DESC");
            await WebConfigUtils.Database.CreateIndexAsync(tableName, $"IX_{tableName}_Taxis",
                $"{ContentAttribute.Taxis} DESC");
        }

        private async Task QueryWhereAsync(Query query, Site site, Channel channel, bool isAllContents)
        {
            query.Where(nameof(Content.SiteId), site.Id);
            query.WhereNot(nameof(Content.SourceId), SourceManager.Preview);

            if (isAllContents)
            {
                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channel, EScopeType.All);
                query.WhereIn(nameof(Content.ChannelId), channelIdList);
            }
            else
            {
                query.Where(nameof(Content.ChannelId), channel.Id);
            }
        }
    }
}
