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
using ETaxisType = SiteServer.Abstractions.ETaxisType;
using ETaxisTypeUtils = SiteServer.Abstractions.ETaxisTypeUtils;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository : DataProviderBase
    {
        private const int TaxisIsTopStartValue = 2000000000;

        private static string MinListColumns { get; } = $"{nameof(IContentMin.Id)}, {nameof(IContentMin.ChannelId)}, {nameof(IContentMin.IsTop)}, {nameof(IContentMin.AddDate)}, {nameof(IContentMin.LastEditDate)}, {nameof(IContentMin.Taxis)}, {nameof(IContentMin.Hits)}, {nameof(IContentMin.HitsByDay)}, {nameof(IContentMin.HitsByWeek)}, {nameof(IContentMin.HitsByMonth)}";

        private static string[] MinColumns { get; } = {
            nameof(IContentMin.Id),
            nameof(IContentMin.ChannelId),
            nameof(IContentMin.IsTop),
            nameof(IContentMin.AddDate),
            nameof(IContentMin.LastEditDate),
            nameof(IContentMin.Taxis),
            nameof(IContentMin.Hits),
            nameof(IContentMin.HitsByDay),
            nameof(IContentMin.HitsByWeek),
            nameof(IContentMin.HitsByMonth)
        };

        public static string GetContentTableName(int siteId)
        {
            return $"siteserver_Content_{siteId}";
        }

        private readonly IDatabase _db;
        private readonly IDistributedCache _cache;

        public ContentRepository()
        {
            _db = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
            _cache = CacheManager.Cache;
        }

        public List<TableColumn> GetTableColumns(string tableName)
        {
            var repository = GetRepository(tableName);
            return repository.TableColumns;
        }

        public List<TableColumn> GetDefaultTableColumns(string tableName)
        {
            var tableColumns = new List<TableColumn>();
            tableColumns.AddRange(GetTableColumns(tableName));
            tableColumns.Add(new TableColumn
            {
                AttributeName = ContentAttribute.SubTitle,
                DataType = DataType.VarChar,
                DataLength = 255
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = ContentAttribute.ImageUrl,
                DataType = DataType.VarChar,
                DataLength = 200
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = ContentAttribute.VideoUrl,
                DataType = DataType.VarChar,
                DataLength = 200
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = ContentAttribute.FileUrl,
                DataType = DataType.VarChar,
                DataLength = 200
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = nameof(ContentAttribute.Content),
                DataType = DataType.Text
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = nameof(ContentAttribute.Summary),
                DataType = DataType.Text
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = nameof(ContentAttribute.Author),
                DataType = DataType.VarChar,
                DataLength = 255
            });
            tableColumns.Add(new TableColumn
            {
                AttributeName = nameof(ContentAttribute.Source),
                DataType = DataType.VarChar,
                DataLength = 255
            });

            return tableColumns;
        }

        public async Task UpdateIsCheckedAsync(string tableName, int siteId, int channelId, IEnumerable<int> contentIdList, int translateChannelId, string userName, bool isChecked, int checkedLevel, string reasons)
        {
            if (isChecked)
            {
                checkedLevel = 0;
            }

            var checkDate = DateTime.Now;

            var repository = GetRepository(tableName);

            foreach (var contentId in contentIdList)
            {
                var settingsXml = await repository.GetAsync<string>(Q
                    .Select(nameof(Content.SettingsXml))
                    .Where(nameof(Content.Id), contentId)
                );

                var attributes = TranslateUtils.JsonDeserialize(settingsXml, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

                attributes[nameof(Content.CheckUserName)] = userName;
                attributes[nameof(Content.CheckDate)] = DateUtils.GetDateAndTimeString(checkDate);
                attributes[nameof(Content.CheckReasons)] = reasons;

                settingsXml = TranslateUtils.JsonSerialize(attributes);

                if (translateChannelId > 0)
                {
                    await repository.UpdateAsync(Q
                        .Set(nameof(Content.IsChecked), isChecked.ToString())
                        .Set(nameof(Content.CheckedLevel), checkedLevel)
                        .Set(nameof(Content.SettingsXml), settingsXml)
                        .Set(nameof(Content.ChannelId), translateChannelId)
                        .Where(nameof(Content.Id), contentId)
                    );
                }
                else
                {
                    await repository.UpdateAsync(Q
                        .Set(nameof(Content.IsChecked), isChecked.ToString())
                        .Set(nameof(Content.CheckedLevel), checkedLevel)
                        .Set(nameof(Content.SettingsXml), settingsXml)
                        .Where(nameof(Content.Id), contentId)
                    );
                }

                await DataProvider.ContentCheckRepository.InsertAsync(new ContentCheck
                {
                    TableName = tableName,
                    SiteId = siteId,
                    ChannelId = channelId,
                    ContentId = contentId,
                    UserName = userName,
                    Checked = isChecked,
                    CheckedLevel = checkedLevel,
                    CheckDate = checkDate,
                    Reasons = reasons
                });
            }

            foreach (var contentId in contentIdList)
            {
                await RemoveEntityCacheAsync(repository, contentId);
            }
        }

        public async Task SetAutoPageContentToSiteAsync(Site site)
        {
            if (!site.IsAutoPageInTextEditor) return;

            var tableNames = await DataProvider.SiteRepository.GetAllTableNameListAsync();
            foreach (var tableName in tableNames)
            {
                var repository = GetRepository(tableName);

                var list = await repository.GetAllAsync<(int ContentId, string Content)>(Q
                    .Select(ContentAttribute.Id, ContentAttribute.Content)
                    .Where(ContentAttribute.SiteId, site.Id)
                );

                foreach (var (contentId, contentValue) in list)
                {
                    var content = ContentUtility.GetAutoPageContent(contentValue, site.AutoPageWordNum);
                    await repository.UpdateAsync(Q
                        .Set(ContentAttribute.Content, content)
                        .Where(ContentAttribute.Id, contentId)
                    );

                    await RemoveEntityCacheAsync(repository, contentId);
                }
            }
        }

        public async Task UpdateTrashContentsAsync(int siteId, int channelId, string tableName, IEnumerable<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var repository = GetRepository(tableName);
            foreach (var contentId in contentIdList)
            {
                await RemoveEntityCacheAsync(repository, contentId);
            }

            var referenceIdList = await GetReferenceIdListAsync(tableName, contentIdList);
            if (referenceIdList.Any())
            {
                await DeleteReferenceContentsAsync(siteId, channelId, tableName, referenceIdList);
            }

            if (!string.IsNullOrEmpty(tableName) && contentIdList.Any())
            {
                await repository.UpdateAsync(Q
                    .SetRaw("ChannelId = -ChannelId")
                    .Where(ContentAttribute.SiteId, siteId)
                    .WhereIn(ContentAttribute.Id, contentIdList)
                );
            }
        }

        public async Task UpdateTrashContentsByChannelIdAsync(int siteId, int channelId, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = await GetContentIdListAsync(tableName, channelId);
            var repository = GetRepository(tableName);
            foreach (var contentId in contentIdList)
            {
                await RemoveEntityCacheAsync(repository, contentId);
            }

            var referenceIdList = await GetReferenceIdListAsync(tableName, contentIdList);
            if (referenceIdList.Any())
            {
                await DeleteReferenceContentsAsync(siteId, channelId, tableName, referenceIdList);
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                await repository.UpdateAsync(Q
                    .SetRaw("ChannelId = -ChannelId")
                    .Where(ContentAttribute.SiteId, siteId)
                    .Where(ContentAttribute.ChannelId, channelId)
                );
            }
        }

        public async Task UpdateRestoreContentsByTrashAsync(int siteId, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var repository = GetRepository(tableName);
            await repository.UpdateAsync(Q
                .SetRaw("ChannelId = -ChannelId")
                .Where(ContentAttribute.SiteId, siteId)
                .Where(ContentAttribute.ChannelId, "<", 0)
            );
        }

        public async Task DeleteAsync(string tableName, int siteId, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return;

            var repository = GetRepository(tableName);
            await RemoveEntityCacheAsync(repository, contentId);

            await ContentTagUtils.RemoveTagsAsync(siteId, contentId);

            await repository.DeleteAsync(contentId);
        }

        private async Task DeleteReferenceContentsAsync(int siteId, int channelId, string tableName, IEnumerable<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName) || contentIdList == null) return;

            var repository = GetRepository(tableName);
            foreach (var contentId in contentIdList)
            {
                await RemoveEntityCacheAsync(repository, contentId);
            }

            await ContentTagUtils.RemoveTagsAsync(siteId, contentIdList);

            await repository.DeleteAsync(Q
                .Where(ContentAttribute.SiteId, siteId)
                .Where(ContentAttribute.ReferenceId, ">", 0)
                .WhereIn(ContentAttribute.Id, contentIdList)
            );
        }

        public async Task UpdateArrangeTaxisAsync(string tableName, int channelId, string attributeName, bool isDesc)
        {
            var query = Q
                .Where(nameof(Content.ChannelId), channelId)
                .OrWhere(nameof(Content.ChannelId), -channelId);
            //由于页面排序是按Taxis的Desc排序的，所以这里sql里面的ASC/DESC取反
            if (isDesc)
            {
                query.OrderBy(attributeName);
            }
            else
            {
                query.OrderByDesc(attributeName);
            }

            var repository = GetRepository(tableName);

            var list = await repository.GetAllAsync<(int id, string isTop)>(query);
            var taxis = 1;
            foreach (var (id, isTop) in list)
            {
                await repository.UpdateAsync(Q
                    .Set(nameof(Content.Taxis), taxis++)
                    .Set(nameof(Content.IsTop), isTop)
                    .Where(nameof(Content.Id), id)
                );

                await RemoveEntityCacheAsync(repository, id);
            }
        }

        public async Task<bool> SetTaxisToUpAsync(string tableName, int channelId, int contentId, bool isTop)
        {
            var repository = GetRepository(tableName);

            var taxis = await repository.GetAsync<int>(Q
                .Select(ContentAttribute.Taxis)
                .Where(ContentAttribute.Id, contentId)
            );

            var result = await repository.GetAsync<(int HigherId, int HigherTaxis)?>(Q
                .Select(ContentAttribute.Id, ContentAttribute.Taxis)
                .Where(ContentAttribute.ChannelId, channelId)
                .Where(ContentAttribute.Taxis, ">", taxis)
                .Where(ContentAttribute.Taxis, isTop ? ">=" : "<", TaxisIsTopStartValue)
                .OrderBy(ContentAttribute.Taxis));

            var higherId = 0;
            var higherTaxis = 0;
            if (result != null)
            {
                higherId = result.Value.HigherId;
                higherTaxis = result.Value.HigherTaxis;
            }

            if (higherId == 0) return false;

            await RemoveEntityCacheAsync(repository, contentId);
            await RemoveEntityCacheAsync(repository, higherId);

            await repository.UpdateAsync(Q
                .Set(ContentAttribute.Taxis, higherTaxis)
                .Where(ContentAttribute.Id, contentId)
            );

            await repository.UpdateAsync(Q
                .Set(ContentAttribute.Taxis, taxis)
                .Where(ContentAttribute.Id, higherId)
            );

            return true;
        }

        public async Task<bool> SetTaxisToDownAsync(string tableName, int channelId, int contentId, bool isTop)
        {
            var repository = GetRepository(tableName);

            var taxis = await repository.GetAsync<int>(Q
                .Select(ContentAttribute.Taxis)
                .Where(ContentAttribute.Id, contentId)
            );

            var result = await repository.GetAsync<(int LowerId, int LowerTaxis)?>(Q
                .Select(ContentAttribute.Id, ContentAttribute.Taxis)
                .Where(ContentAttribute.ChannelId, channelId)
                .Where(ContentAttribute.Taxis, "<", taxis)
                .Where(ContentAttribute.Taxis, isTop ? ">=" : "<", TaxisIsTopStartValue)
                .OrderByDesc(ContentAttribute.Taxis));

            var lowerId = 0;
            var lowerTaxis = 0;
            if (result != null)
            {
                lowerId = result.Value.LowerId;
                lowerTaxis = result.Value.LowerTaxis;
            }

            if (lowerId == 0) return false;

            await RemoveEntityCacheAsync(repository, contentId);
            await RemoveEntityCacheAsync(repository, lowerId);

            await repository.UpdateAsync(Q
                .Set(ContentAttribute.Taxis, lowerTaxis)
                .Where(ContentAttribute.Id, contentId)
            );

            await repository.UpdateAsync(Q
                .Set(ContentAttribute.Taxis, taxis)
                .Where(ContentAttribute.Id, lowerId)
            );

            return true;
        }

        public async Task<int> GetMaxTaxisAsync(string tableName, int channelId, bool isTop)
        {
            var repository = GetRepository(tableName);

            var maxTaxis = 0;
            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                var max = await repository.MaxAsync(nameof(Content.Taxis), Q
                    .Where(nameof(Content.ChannelId), channelId)
                    .Where(nameof(Content.Taxis), ">=", TaxisIsTopStartValue)
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
                var max = await repository.MaxAsync(nameof(Content.Taxis), Q
                    .Where(nameof(Content.ChannelId), channelId)
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
                    .Set(ContentAttribute.GroupNameCollection, content.GroupNameCollection)
                    .Where(nameof(Content.Id), contentId)
                );

                await RemoveEntityCacheAsync(repository, contentId);
            }
        }

        public async Task AddDownloadsAsync(string tableName, int channelId, int contentId)
        {
            var repository = GetRepository(tableName);
            await RemoveEntityCacheAsync(repository, contentId);

            await repository.IncrementAsync(ContentAttribute.Downloads, Q
                .Where(ContentAttribute.Id, contentId)
            );
        }

        private async Task<IEnumerable<int>> GetReferenceIdListAsync(string tableName, IEnumerable<int> contentIdList)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<int>(Q
                .Select(ContentAttribute.Id)
                .Where(ContentAttribute.ChannelId, ">", 0)
                .WhereIn(ContentAttribute.ReferenceId, contentIdList)
            );
        }

        public async Task<int> GetTotalHitsAsync(string tableName, int siteId)
        {
            var repository = GetRepository(tableName);
            return await repository.SumAsync(ContentAttribute.Hits, Q
                .Where(nameof(Content.IsChecked), true.ToString())
                .Where(ContentAttribute.SiteId, siteId)
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

        public async Task<IEnumerable<int>> GetContentIdListAsync(string tableName, int channelId)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<int>(Q
                .Select(ContentAttribute.Id)
                .Where(ContentAttribute.ChannelId, channelId)
            );
        }

        public async Task<IEnumerable<int>> GetContentIdListAsync(string tableName, int channelId, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
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
                query.Where(nameof(Content.IsChecked), ETriStateUtils.GetValue(checkedState));
            }

            return await repository.GetAllAsync<int>(query);
        }

        public async Task<IEnumerable<int>> GetContentIdListCheckedByChannelIdAsync(string tableName, int siteId, int channelId)
        {
            var repository = GetRepository(tableName);
            return await repository.GetAllAsync<int>(Q
                .Select(ContentAttribute.Id)
                .Where(ContentAttribute.SiteId, siteId)
                .Where(ContentAttribute.ChannelId, channelId)
                .Where(nameof(Content.IsChecked), true.ToString())
            );
        }

        public async Task<List<string>> GetValueListByStartStringAsync(string tableName, int channelId, string name, string startString, int totalNum)
        {
            var repository = GetRepository(tableName);
            var list = await repository.GetAllAsync<string>(Q
                .Select(name)
                .Where(nameof(Content.ChannelId), channelId)
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

            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

            //出现IsTop与Taxis不同步情况
            if (content.Top == false && content.Taxis >= TaxisIsTopStartValue)
            {
                content.Taxis = await GetMaxTaxisAsync(tableName, content.ChannelId, false) + 1;
            }
            else if (content.Top && content.Taxis < TaxisIsTopStartValue)
            {
                content.Taxis = await GetMaxTaxisAsync(tableName, content.ChannelId, true) + 1;
            }

            content.LastEditDate = DateTime.Now;

            var repository = GetRepository(tableName);
            await repository.UpdateAsync(content);
        }

        public async Task UpdateAsync(string tableName, int contentId, string name, string value)
        {
            var repository = GetRepository(tableName);
            await RemoveEntityCacheAsync(repository, contentId);

            await repository.UpdateAsync(Q.Set(name, value).Where(ContentAttribute.Id, contentId));
        }

        public async Task<int> GetCountOfContentUpdateAsync(string tableName, int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName)
        {
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo, scope, string.Empty, string.Empty, string.Empty);
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
            var list = await repository.GetAllAsync<(int Id, int ChannelId)>(Q
                .Select(nameof(Content.Id), nameof(Content.ChannelId))
                .Where(nameof(Content.SiteId), siteId)
                .Where(nameof(Content.ChannelId), "<", 0)
            );

            return list.Select(o => (Math.Abs(o.ChannelId), o.Id)).ToList();
        }

        private DataSet GetStlDataSourceChecked(string tableName, List<int> channelIdList, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }
            var sqlWhereString = channelIdList.Count == 1 ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})";

            if (others != null && others.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

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
                                    var collection = StringUtils.GetStringList(value);
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
                                    var collection = StringUtils.GetStringList(value);
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
                                    var collection = StringUtils.GetStringList(value);
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
                                    var collection = StringUtils.GetStringList(value);
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
                                    var collection = StringUtils.GetStringList(value);
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

        private int GetCountOfContentAdd(string tableName, int siteId, List<int> channelIdList, DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            string sqlString;
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
                sqlString += $" AND {nameof(Content.IsChecked)} = '{ETriStateUtils.GetValue(checkedState)}'";
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

        private List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString, string whereString)
        {
            return GetContentIdListChecked(tableName, channelId, 0, orderByFormatString, whereString);
        }

        public async Task<(List<Tuple<int, int>> Tuples, int TotalCount)> ApiGetContentIdListBySiteIdAsync(string tableName, int siteId, ApiContentsParameters parameters)
        {
            var list = new List<Content>();

            var whereString = $"WHERE {ContentAttribute.SiteId} = {siteId} AND {ContentAttribute.ChannelId} > 0 AND {nameof(Content.IsChecked)} = '{true}'";
            if (parameters.ChannelIds.Count > 0)
            {
                whereString += $" AND {ContentAttribute.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(parameters.ChannelIds)})";
            }
            if (!string.IsNullOrEmpty(parameters.ChannelGroup))
            {
                var channelIdList = await ChannelManager.GetChannelIdListAsync(siteId, parameters.ChannelGroup);
                if (channelIdList.Count > 0)
                {
                    whereString += $" AND {ContentAttribute.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(channelIdList)})";
                }
            }
            if (!string.IsNullOrEmpty(parameters.ContentGroup))
            {
                var contentGroup = parameters.ContentGroup;
                whereString += $" AND ({ContentAttribute.GroupNameCollection} = '{AttackUtils.FilterSql(contentGroup)}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, contentGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + contentGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + contentGroup)})";
            }
            if (!string.IsNullOrEmpty(parameters.Tag))
            {
                var tag = parameters.Tag;
                whereString += $" AND ({ContentAttribute.Tags} = '{AttackUtils.FilterSql(tag)}' OR {SqlUtils.GetInStr(ContentAttribute.Tags, tag + ",")} OR {SqlUtils.GetInStr(ContentAttribute.Tags, "," + tag + ",")} OR {SqlUtils.GetInStr(ContentAttribute.Tags, "," + tag)})";
            }

            var channelInfo = await ChannelManager.GetChannelAsync(siteId, siteId);
            var orderString = GetOrderString(channelInfo, AttackUtils.FilterSql(parameters.OrderBy), false);
            var dbArgs = new Dictionary<string, object>();

            if (parameters.QueryString != null && parameters.QueryString.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

                foreach (string attributeName in parameters.QueryString)
                {
                    if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

                    var value = parameters.QueryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.IsChecked)) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsTop))
                    {
                        whereString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Id) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckedLevel))
                    {
                        whereString += $" AND {attributeName} = {TranslateUtils.ToInt(value)}";
                    }
                    else if (parameters.Likes.Contains(attributeName))
                    {
                        whereString += $" AND {attributeName} LIKE '%{AttackUtils.FilterSql(value)}%'";
                    }
                    else
                    {
                        whereString += $" AND {attributeName} = @{attributeName}";
                        dbArgs.Add(attributeName, value);
                    }
                }
            }

            var totalCount = DataProvider.DatabaseRepository.GetPageTotalCount(tableName, whereString, dbArgs);
            if (totalCount > 0 && parameters.Skip < totalCount)
            {
                var sqlString = DataProvider.DatabaseRepository.GetPageSqlString(tableName, MinListColumns, whereString, orderString, parameters.Skip, parameters.Top);

                using var connection = _db.GetConnection();
                list = connection.Query<Content>(sqlString, dbArgs).ToList();
            }

            var tupleList = new List<Tuple<int, int>>();
            foreach (var contentInfo in list)
            {
                tupleList.Add(new Tuple<int, int>(contentInfo.ChannelId, contentInfo.Id));
            }

            return (tupleList, totalCount);
        }

        public async Task<(IList<(int, int)> List, int TotalCount)> ApiGetContentIdListByChannelIdAsync(string tableName, int siteId, int channelId, int top, int skip, string like, string orderBy, NameValueCollection queryString)
        {
            var retVal = new List<(int, int)>();

            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo, EScopeType.All, string.Empty, string.Empty, string.Empty);
            var whereString = $"WHERE {ContentAttribute.SiteId} = {siteId} AND {ContentAttribute.ChannelId} IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND {nameof(Content.IsChecked)} = '{true}'";

            var likeList = StringUtils.GetStringList(StringUtils.TrimAndToLower(like));
            var orderString = GetOrderString(channelInfo, AttackUtils.FilterSql(orderBy), false);
            var dbArgs = new Dictionary<string, object>();

            if (queryString != null && queryString.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

                foreach (string attributeName in queryString)
                {
                    if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

                    var value = queryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.IsChecked)) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsTop))
                    {
                        whereString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Id) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckedLevel))
                    {
                        whereString += $" AND {attributeName} = {TranslateUtils.ToInt(value)}";
                    }
                    else if (likeList.Contains(attributeName))
                    {
                        whereString += $" AND {attributeName} LIKE '%{AttackUtils.FilterSql(value)}%'";
                    }
                    else
                    {
                        whereString += $" AND {attributeName} = @{attributeName}";
                        dbArgs.Add(attributeName, value);
                    }
                }
            }

            var totalCount = DataProvider.DatabaseRepository.GetPageTotalCount(tableName, whereString, dbArgs);
            if (totalCount > 0 && skip < totalCount)
            {
                var sqlString = DataProvider.DatabaseRepository.GetPageSqlString(tableName, MinListColumns, whereString, orderString, skip, top);

                using var connection = _db.GetConnection();
                retVal = connection.Query<Content>(sqlString, dbArgs).Select(o => (o.ChannelId, o.Id)).ToList();
            }

            return (retVal, totalCount);
        }

        public string GetSelectCommandByHitsAnalysis(string tableName, int siteId)
        {
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var whereString = new StringBuilder();
            whereString.Append($"AND IsChecked = '{true}' AND SiteId = {siteId} AND Hits > 0");
            whereString.Append(orderByString);

            return DataProvider.DatabaseRepository.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
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

            var channelId = await ChannelManager.GetChannelIdAsync(siteId, siteId, channelIndex, channelName);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);

            if (isAllSites)
            {
                whereBuilder.Append("(SiteId > 0) ");
            }
            else if (!string.IsNullOrEmpty(siteIds))
            {
                whereBuilder.Append($"(SiteId IN ({TranslateUtils.ToSqlInStringWithoutQuote(StringUtils.GetIntList(siteIds))})) ");
            }
            else
            {
                whereBuilder.Append($"(SiteId = {site.Id}) ");
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                whereBuilder.Append(" AND ");
                var channelIdList = new List<int>();
                foreach (var theChannelId in StringUtils.GetIntList(channelIds))
                {
                    var theSiteId = await DataProvider.ChannelRepository.GetSiteIdAsync(theChannelId);
                    channelIdList.AddRange(
                        await ChannelManager.GetChannelIdListAsync(await ChannelManager.GetChannelAsync(theSiteId, theChannelId),
                            EScopeType.All, string.Empty, string.Empty, string.Empty));
                }
                whereBuilder.Append(channelIdList.Count == 1
                    ? $"(ChannelId = {channelIdList[0]}) "
                    : $"(ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");
            }
            else if (channelId != siteId)
            {
                whereBuilder.Append(" AND ");

                var theSiteId = await DataProvider.ChannelRepository.GetSiteIdAsync(channelId);
                var channelIdList = await ChannelManager.GetChannelIdListAsync(await ChannelManager.GetChannelAsync(theSiteId, channelId),
                            EScopeType.All, string.Empty, string.Empty, string.Empty);

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
                typeList = StringUtils.GetStringList(type);
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

            var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);
            //var styleInfoList = RelatedIdentities.GetTableStyleInfoList(site, channel.Id);

            foreach (string key in form.Keys)
            {
                if (excludeAttributes.Contains(key.ToLower())) continue;
                if (string.IsNullOrEmpty(form[key])) continue;

                var value = StringUtils.Trim(form[key]);
                if (string.IsNullOrEmpty(value)) continue;

                var columnInfo = TableColumnManager.GetTableColumnInfo(tableName, key);

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
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo,
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

            return GetSqlStringByCondition(tableName, siteId, list, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent);
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

        private string GetStlSqlStringChecked(List<int> channelIdList, string tableName, int siteId, int channelId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            string sqlWhereString;

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

        private string GetSqlStringByCondition(string tableName, int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isTrashContent)
        {
            return GetSqlStringByCondition(tableName, siteId, channelIdList, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent, false, string.Empty);
        }

        private string GetSqlStringByCondition(string tableName, int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isTrashContent, bool isWritingOnly, string userNameOnly)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }

            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

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

            if (StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsTop) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsHot))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereString.Append($"AND ({ContentAttribute.Title} LIKE '%{keyword}%') ");
                }
                whereString.Append($" AND {searchType} = '{true}'");
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

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

            if (!string.IsNullOrEmpty(userNameOnly))
            {
                whereString.Append($" AND {ContentAttribute.AddUserName} = '{userNameOnly}' ");
            }
            if (isWritingOnly)
            {
                whereString.Append($" AND {ContentAttribute.UserId} > 0 ");
            }

            whereString.Append(" ").Append(orderByString);

            return DataProvider.DatabaseRepository.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public async Task<string> GetPagerWhereSqlStringAsync(Site site, Channel channel, string searchType, string keyword, string dateFrom, string dateTo, int checkLevel, bool isCheckOnly, bool isSelfOnly, bool isTrashOnly, bool isWritingOnly, int adminId, bool isSuperAdmin, IList<int> owningChannelIdList, List<string> allAttributeNameList)
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
                var channelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.All, string.Empty, string.Empty, channel.ContentModelPluginId);

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

            if (StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsTop) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsHot))
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
                    ? $"{nameof(Content.IsChecked)} = '{false}'"
                    : $"{nameof(Content.IsChecked)} = '{false}' AND {nameof(ContentAttribute.CheckedLevel)} = {checkLevel}");
            }
            else
            {
                if (checkLevel != CheckManager.LevelInt.All)
                {
                    whereList.Add(checkLevel == site.CheckContentLevel
                        ? $"{nameof(Content.IsChecked)} = '{true}'"
                        : $"{nameof(Content.IsChecked)} = '{false}' AND {nameof(ContentAttribute.CheckedLevel)} = {checkLevel}");
                }
            }

            if (adminId > 0)
            {
                whereList.Add($"{nameof(ContentAttribute.AdminId)} = {adminId}");
            }

            if (isWritingOnly)
            {
                whereList.Add($"{nameof(ContentAttribute.UserId)} > 0");
            }

            return $"WHERE {StringUtils.Join(whereList, " AND ")}";
        }

        private string GetCreateContentTableSqlString(string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder(DataProvider.DatabaseRepository.GetCreateTableSqlString(tableName, tableColumns));
            sqlBuilder.AppendLine().Append("GO");

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                sqlBuilder.Append($@"
CREATE INDEX `IX_{tableName}` ON `{tableName}`(`{ContentAttribute.IsTop}` DESC, `{ContentAttribute.Taxis}` DESC, `{ContentAttribute.Id}` DESC)
GO
CREATE INDEX `IX_{tableName}_Taxis` ON `{tableName}`(`{ContentAttribute.Taxis}` DESC)
GO");
            }
            else
            {
                sqlBuilder.Append($@"
CREATE INDEX IX_{tableName} ON {tableName}({ContentAttribute.IsTop} DESC, {ContentAttribute.Taxis} DESC, {ContentAttribute.Id} DESC)
GO
CREATE INDEX IX_{tableName}_Taxis ON {tableName}({ContentAttribute.Taxis} DESC)
GO");
            }

            return sqlBuilder.ToString();
        }

        public async Task CreateContentTableAsync(string tableName, List<TableColumn> columnInfoList)
        {
            var isDbExists = await WebConfigUtils.Database.IsTableExistsAsync(tableName);
            if (isDbExists) return;

            var createTableSqlString = GetCreateContentTableSqlString(tableName, columnInfoList);

            using var conn = _db.GetConnection();
            conn.Open();
            using var trans = conn.BeginTransaction();
            try
            {
                var reader = new System.IO.StringReader(createTableSqlString);
                string sql;
                while (null != (sql = SqlUtils.ReadNextStatementFromStream(reader)))
                {
                    ExecuteNonQuery(trans, sql.Trim());
                }

                TableColumnManager.ClearCache();

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        private string GetOrderString(Channel channel, string orderBy, bool isAllContents)
        {
            return isAllContents
                ? ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc)
                : ETaxisTypeUtils.GetContentOrderByString(
                    ETaxisTypeUtils.GetEnumType(channel.DefaultTaxisType), orderBy);
        }

        private async Task QueryWhereAsync(Query query, Site site, Channel channel, int adminId, bool isAllContents)
        {
            query.Where(nameof(Content.SiteId), site.Id);
            query.WhereNot(nameof(Content.SourceId), SourceManager.Preview);


            if (adminId > 0)
            {
                query.Where(nameof(Content.AdminId), adminId);
            }

            if (isAllContents)
            {
                var channelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.All);
                query.WhereIn(nameof(Content.ChannelId), channelIdList);
            }
            else
            {
                query.Where(nameof(Content.ChannelId), channel.Id);
            }
        }

        private void QueryOrder(Query query, Channel channel, string orderBy, bool isAllContents)
        {
            QueryOrder(query, isAllContents
                    ? ETaxisType.OrderByIdDesc
                    : ETaxisTypeUtils.GetEnumType(channel.DefaultTaxisType), orderBy);
        }

        private void QueryOrder(Query query, ETaxisType taxisType, string orderByString = null)
        {
            if (!string.IsNullOrEmpty(orderByString))
            {
                if (orderByString.Trim().ToUpper().StartsWith("ORDER BY "))
                {
                    orderByString = orderByString.Substring("ORDER BY ".Length);
                }

                query.OrderByRaw(orderByString);
            }

            if (taxisType == ETaxisType.OrderById)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderBy(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByIdDesc)
            {
                query.OrderByDesc(nameof(Content.IsTop), nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByChannelId)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderBy(nameof(Content.ChannelId)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByChannelIdDesc)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderByDesc(nameof(Content.ChannelId)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByAddDate)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderBy(nameof(Content.AddDate)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByAddDateDesc)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderByDesc(nameof(Content.AddDate)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByLastEditDate)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderBy(nameof(Content.LastEditDate)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderByDesc(nameof(Content.LastEditDate)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByTaxis)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderBy(nameof(Content.Taxis)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByTaxisDesc)
            {
                query.OrderByDesc(nameof(Content.IsTop)).OrderByDesc(nameof(Content.Taxis)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByHits)
            {
                query.OrderByDesc(nameof(Content.Hits)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByHitsByDay)
            {
                query.OrderByDesc(nameof(Content.HitsByDay)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByHitsByWeek)
            {
                query.OrderByDesc(nameof(Content.HitsByWeek)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByHitsByMonth)
            {
                query.OrderByDesc(nameof(Content.HitsByMonth)).OrderByDesc(nameof(Content.Id));
            }
            else if (taxisType == ETaxisType.OrderByRandom)
            {
                query.OrderByRandom(StringUtils.Guid());
            }
        }

        private async Task<IEnumerable<Content>> GetContentListAsync(Repository<Content> repository, Query query)
        {
            return await repository.GetAllAsync(query);
        }

        private async Task<IEnumerable<IContentMin>> GetContentMinListAsync(string tableName, Query query)
        {
            var repository = GetRepository(tableName);
            var q = query.Clone();
            q.Select(MinColumns);
            return await repository.GetAllAsync<ContentMin>(q);
        }
    }
}
