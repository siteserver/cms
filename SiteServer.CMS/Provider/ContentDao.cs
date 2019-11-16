using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using Dapper;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.DataCache.Content;
using Datory;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
using SqlKata;

namespace SiteServer.CMS.Provider
{
    public class ContentDao : DataProviderBase
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
        private readonly Repository<Content> _repo;

        public ContentDao()
        {
            _db = new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);
            _repo = new Repository<Content>(_db);
        }

        public List<TableColumn> TableColumns => _repo.TableColumns;

        public List<TableColumn> TableColumnsDefault
        {
            get
            {
                var tableColumns = new List<TableColumn>();
                tableColumns.AddRange(TableColumns);
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
        }

        public void Update(string tableName, int channelId, int contentId, string name, string value)
        {
            var sqlString = $"UPDATE {tableName} SET {name} = @{name} WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add($"@{name}", value);
            parameters.Add("@Id", contentId);

            using (var connection = GetConnection())
            {
                connection.Execute(sqlString, parameters);
            }

            ContentManager.RemoveCache(tableName, channelId);
        }

        public async Task UpdateIsCheckedAsync(string tableName, int siteId, int channelId, List<int> contentIdList, int translateChannelId, string userName, bool isChecked, int checkedLevel, string reasons)
        {
            if (isChecked)
            {
                checkedLevel = 0;
            }

            var checkDate = DateTime.Now;

            var repository = new Repository<Content>(_db, tableName);

            foreach (var contentId in contentIdList)
            {
                var settingsXml = await repository.GetAsync<string>(Q
                    .Select(nameof(Content.SettingsXml))
                    .Where(nameof(Content.Id), contentId)
                );

                var attributes = TranslateUtils.JsonDeserialize(settingsXml, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

                attributes[ContentAttribute.CheckUserName] = userName;
                attributes[ContentAttribute.CheckDate] = DateUtils.GetDateAndTimeString(checkDate);
                attributes[ContentAttribute.CheckReasons] = reasons;

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

                await DataProvider.ContentCheckDao.InsertAsync(new ContentCheck
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

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void SetAutoPageContentToSite(Site site)
        {
            if (!site.IsAutoPageInTextEditor) return;

            var sqlString =
                $"SELECT Id, {ContentAttribute.ChannelId}, {nameof(ContentAttribute.Content)} FROM {site.TableName} WHERE SiteId = {site.Id}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
                    var channelId = GetInt(rdr, 1);
                    var content = GetString(rdr, 2);

                    if (!string.IsNullOrEmpty(content))
                    {
                        content = ContentUtility.GetAutoPageContent(content, site.AutoPageWordNum);

                        Update(site.TableName, channelId, contentId, nameof(ContentAttribute.Content), content);
                    }
                }

                rdr.Close();
            }
        }

        public async Task UpdateTrashContentsAsync(int siteId, int channelId, string tableName, List<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                await DeleteReferenceContentsAsync(siteId, channelId, tableName, referenceIdList);
            }

            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                var sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public async Task UpdateTrashContentsByChannelIdAsync(int siteId, int channelId, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdList(tableName, channelId);
            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                await DeleteReferenceContentsAsync(siteId, channelId, tableName, referenceIdList);
            }
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId = {siteId}";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void Delete(string tableName, int siteId, int channelId, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            ExecuteNonQuery($"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id = {contentId}");
        }

        private async Task DeleteReferenceContentsAsync(int siteId, int channelId, string tableName, List<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var deleteNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                await ContentTagUtils.RemoveTagsAsync(siteId, contentIdList);
                
                var sqlString =
                    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND {ContentAttribute.ReferenceId} > 0 AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";

                deleteNum = ExecuteNonQuery(sqlString);
            }

            if (deleteNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void UpdateRestoreContentsByTrash(int siteId, int channelId, string tableName)
        {
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId < 0";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        private void UpdateTaxis(int channelId, int id, int taxis, string tableName)
        {
            var sqlString = $"UPDATE {tableName} SET Taxis = {taxis} WHERE Id = {id}";
            ExecuteNonQuery(sqlString);

            ContentManager.RemoveCache(tableName, channelId);
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

            var repository = new Repository<Content>(_db, tableName);

            var list = await repository.GetAllAsync<(int id, string isTop)>(query);
            var taxis = 1;
            foreach (var (id, isTop) in list)
            {
                await repository.UpdateAsync(Q
                    .Set(nameof(Content.Taxis), taxis++)
                    .Set(nameof(Content.IsTop), isTop)
                    .Where(nameof(Content.Id), id)
                );
            }

            ContentManager.RemoveCache(tableName, channelId);
        }

        public bool SetTaxisToUp(string tableName, int channelId, int contentId, bool isTop)
        {
            //Get Higher Taxis and Id
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id, Taxis",
                isTop
                    ? $"WHERE (Taxis > (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis >= {TaxisIsTopStartValue} AND ChannelId = {channelId})"
                    : $"WHERE (Taxis > (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis < {TaxisIsTopStartValue} AND ChannelId = {channelId})",
                "ORDER BY Taxis", 1);
            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherId = GetInt(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (higherId != 0)
            {
                //Get Taxis Of Selected Id
                var selectedTaxis = GetTaxis(contentId, tableName);

                //Set The Selected Class Taxis To Higher Level
                UpdateTaxis(channelId, contentId, higherTaxis, tableName);
                //Set The Higher Class Taxis To Lower Level
                UpdateTaxis(channelId, higherId, selectedTaxis, tableName);
                return true;
            }
            return false;
        }

        public bool SetTaxisToDown(string tableName, int channelId, int contentId, bool isTop)
        {
            //Get Lower Taxis and Id
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id, Taxis",
                isTop
                    ? $"WHERE (Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis >= {TaxisIsTopStartValue} AND ChannelId = {channelId})"
                    : $"WHERE (Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis < {TaxisIsTopStartValue} AND ChannelId = {channelId})",
                "ORDER BY Taxis DESC", 1);
            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = GetInt(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (lowerId != 0)
            {
                //Get Taxis Of Selected Class
                var selectedTaxis = GetTaxis(contentId, tableName);

                //Set The Selected Class Taxis To Lower Level
                UpdateTaxis(channelId, contentId, lowerTaxis, tableName);
                //Set The Lower Class Taxis To Higher Level
                UpdateTaxis(channelId, lowerId, selectedTaxis, tableName);
                return true;
            }
            return false;
        }

        public int GetMaxTaxis(string tableName, int channelId, bool isTop)
        {
            var maxTaxis = 0;
            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                var sqlString =
                    $"SELECT MAX(Taxis) FROM {tableName} WHERE ChannelId = {channelId} AND Taxis >= {TaxisIsTopStartValue}";

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var rdr = ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            maxTaxis = GetInt(rdr, 0);
                        }
                        rdr.Close();
                    }
                }

                if (maxTaxis < TaxisIsTopStartValue)
                {
                    maxTaxis = TaxisIsTopStartValue;
                }
            }
            else
            {
                var sqlString =
                    $"SELECT MAX(Taxis) FROM {tableName} WHERE ChannelId = {channelId} AND Taxis < {TaxisIsTopStartValue}";
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var rdr = ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            maxTaxis = GetInt(rdr, 0);
                        }
                        rdr.Close();
                    }
                }
            }
            return maxTaxis;
        }

        public Tuple<int, string> GetValue(string tableName, int contentId, string name)
        {
            Tuple<int, string> tuple = null;

            try
            {
                var sqlString = $"SELECT {ContentAttribute.ChannelId}, {name} FROM {tableName} WHERE Id = {contentId}";

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var rdr = ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            var channelId = GetInt(rdr, 0);
                            var value = GetString(rdr, 1);

                            tuple = new Tuple<int, string>(channelId, value);
                        }

                        rdr.Close();
                    }
                }
            }
            catch
            {
                // ignored
            }

            return tuple;
        }

        public void AddContentGroupList(string tableName, int contentId, List<string> contentGroupList)
        {
            var tuple = GetValue(tableName, contentId, ContentAttribute.GroupNameCollection);

            if (tuple != null)
            {
                var list = TranslateUtils.StringCollectionToStringList(tuple.Item2);
                foreach (var groupName in contentGroupList)
                {
                    if (!list.Contains(groupName)) list.Add(groupName);
                }
                Update(tableName, tuple.Item1, contentId, ContentAttribute.GroupNameCollection, TranslateUtils.ObjectCollectionToString(list));
            }
        }

        private List<int> GetReferenceIdList(string tableName, List<int> contentIdList)
        {
            var list = new List<int>();
            var sqlString =
                $"SELECT Id FROM {tableName} WHERE ChannelId > 0 AND ReferenceId IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public int GetTotalHits(string tableName, int siteId)
        {
            return DataProvider.DatabaseDao.GetIntResult($"SELECT SUM(Hits) FROM {tableName} WHERE IsChecked='{true}' AND SiteId = {siteId} AND Hits > 0");
        }

        public int GetFirstContentId(string tableName, int channelId)
        {
            var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} ORDER BY Taxis DESC, Id DESC";
            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<int> GetContentIdList(string tableName, int channelId)
        {
            var list = new List<int>();

            var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId}";
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

        public List<int> GetContentIdList(string tableName, int channelId, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var list = new List<int>();

            var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId}";
            if (isPeriods)
            {
                var dateString = string.Empty;
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    dateString = $" AND AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ";
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    dateString += $" AND AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))} ";
                }
                sqlString += dateString;
            }

            if (checkedState != ETriState.All)
            {
                sqlString += $" AND IsChecked = '{ETriStateUtils.GetValue(checkedState)}'";
            }

            sqlString += " ORDER BY Taxis DESC, Id DESC";

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

        public List<int> GetContentIdListCheckedByChannelId(string tableName, int siteId, int channelId)
        {
            var list = new List<int>();

            var sqlString = $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelId} AND IsChecked = '{true}'";
            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public int GetContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var contentId = 0;
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id", $"WHERE (ChannelId = {channelId} AND Taxis > {taxis} AND IsChecked = 'True')", "ORDER BY Taxis", 1);
            if (isNextContent)
            {
                sqlString = SqlUtils.ToTopSqlString(tableName, "Id",
                $"WHERE (ChannelId = {channelId} AND Taxis < {taxis} AND IsChecked = 'True')", "ORDER BY Taxis DESC", 1);
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    contentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }

        public int GetContentId(string tableName, int channelId, bool isCheckedOnly, string orderByString)
        {
            var contentId = 0;
            var whereString = $"WHERE {ContentAttribute.ChannelId} = {channelId}";
            if (isCheckedOnly)
            {
                whereString += $" AND {nameof(Content.IsChecked)} = '{true.ToString()}'";
            }
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id", whereString, orderByString, 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    contentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }

        public async Task<List<string>> GetValueListByStartStringAsync(string tableName, int channelId, string name, string startString, int totalNum)
        {
            var repository = new Repository<Content>(_db, tableName);
            var list = await repository.GetAllAsync<string>(Q
                .Select(name)
                .Where(nameof(Content.ChannelId), channelId)
                .WhereInStr(WebConfigUtils.DatabaseType, name, startString)
                .Distinct()
                .Limit(totalNum)
            );
            return list.ToList();
        }

        public int GetChannelId(string tableName, int contentId)
        {
            var channelId = 0;
            var sqlString = $"SELECT {ContentAttribute.ChannelId} FROM {tableName} WHERE (Id = {contentId})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    channelId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return channelId;
        }

        public int GetSequence(string tableName, int channelId, int contentId)
        {
            var sqlString =
                $"SELECT COUNT(*) FROM {tableName} WHERE {ContentAttribute.ChannelId} = {channelId} AND {nameof(Content.IsChecked)} = '{true}' AND Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND {ContentAttribute.SourceId} != {SourceManager.Preview}";

            return DataProvider.DatabaseDao.GetIntResult(sqlString) + 1;
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

        public DataSet GetDataSetOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = GetSqlStringOfAdminExcludeRecycle(tableName, siteId, begin, end);

            return ExecuteDataset(sqlString);
        }

        public async Task<int> InsertAsync(string tableName, Site site, Channel channel, Content content)
        {
            var taxis = 0;
            if (content.SourceId == SourceManager.Preview)
            {
                channel.IsPreviewContentsExists = true;
                await DataProvider.ChannelDao.UpdateAdditionalAsync(channel);
            }
            else
            {
                taxis = GetTaxisToInsert(tableName, content.ChannelId, content.Top);
            }
            return await InsertWithTaxisAsync(tableName, site, channel, content, taxis);
        }

        public async Task<int> InsertPreviewAsync(string tableName, Site site, Channel channel, Content content)
        {
            channel.IsPreviewContentsExists = true;
            await DataProvider.ChannelDao.UpdateAdditionalAsync(channel);

            content.SourceId = SourceManager.Preview;
            return await InsertWithTaxisAsync(tableName, site, channel, content, 0);
        }

        public async Task<int> InsertWithTaxisAsync(string tableName, Site site, Channel channel, Content content, int taxis)
        {
            if (string.IsNullOrEmpty(tableName)) return 0;

            if (site.IsAutoPageInTextEditor && content.ContainsKey(ContentAttribute.Content))
            {
                content.Set(ContentAttribute.Content, ContentUtility.GetAutoPageContent(content.Get<string>(ContentAttribute.Content), site.AutoPageWordNum));
            }

            content.Taxis = taxis;

            var contentId = await InsertInnerAsync(tableName, site, channel, content);

            return contentId;
        }

        private async Task<int> InsertInnerAsync(string tableName, Site site, Channel channel, Content content)
        {
            if (string.IsNullOrEmpty(tableName) || content == null) return 0;

            content.LastEditDate = DateTime.Now;

            var repository = new Repository<Content>(_db, tableName);
            content.Id = await repository.InsertAsync(content);

            await ContentManager.InsertCacheAsync(site, channel, content);

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
                content.Taxis = GetMaxTaxis(tableName, content.ChannelId, false) + 1;
            }
            else if (content.Top && content.Taxis < TaxisIsTopStartValue)
            {
                content.Taxis = GetMaxTaxis(tableName, content.ChannelId, true) + 1;
            }

            content.LastEditDate = DateTime.Now;

            var repository = new Repository<Content>(_db, tableName);
            await repository.UpdateAsync(content);

            await ContentManager.UpdateCacheAsync(site, channel, content);
            ContentManager.RemoveCountCache(tableName);

            //TODO: must delete
            //LogUtils.AddSiteLog(content.SiteId, content.ChannelId, content.Id, content.LastEditUserName, "更新内容", content.Body);
        }

        public async Task<int> GetCountOfContentAddAsync(string tableName, int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var channelIdList = await ChannelManager.GetChannelIdListAsync(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return GetCountOfContentAdd(tableName, siteId, channelIdList, begin, end, userName, checkedState);
        }

        public List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString)
        {
            return GetContentIdListChecked(tableName, channelId, orderByFormatString, string.Empty);
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

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public DataSet GetStlDataSourceChecked(List<int> channelIdList, string tableName, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            return GetStlDataSourceChecked(tableName, channelIdList, startNum, totalNum, orderByString, whereString, others);
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

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<ContentCountInfo> GetContentCountInfoList(string tableName)
        {
            List<ContentCountInfo> list;

            var sqlString =
                $@"SELECT {ContentAttribute.SiteId}, {ContentAttribute.ChannelId}, {nameof(Content.IsChecked)}, {ContentAttribute.CheckedLevel}, {ContentAttribute.AdminId}, COUNT(*) AS {nameof(ContentCountInfo.Count)} FROM {tableName} WHERE {ContentAttribute.ChannelId} > 0 AND {ContentAttribute.SourceId} != {SourceManager.Preview} GROUP BY {ContentAttribute.SiteId}, {ContentAttribute.ChannelId}, {nameof(Content.IsChecked)}, {ContentAttribute.CheckedLevel}, {ContentAttribute.AdminId}";

            using (var connection = GetConnection())
            {
                list = connection.Query<ContentCountInfo>(sqlString).ToList();
            }

            return list;
        }

        public async Task<int> GetCountCheckedImageAsync(int siteId, int channelId)
        {
            var tableName = await SiteManager.GetTableNameAsync(siteId);
            var sqlString =
                $"SELECT COUNT(*) FROM {tableName} WHERE {ContentAttribute.ChannelId} = {channelId} AND {ContentAttribute.ImageUrl} != '' AND {nameof(Content.IsChecked)} = '{true}' AND {ContentAttribute.SourceId} != {SourceManager.Preview}";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int selectedId, string tableName)
        {
            var sqlString = $"SELECT Taxis FROM {tableName} WHERE (Id = {selectedId})";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<(int, int)> GetContentIdListByTrash(int siteId, string tableName)
        {
            List<(int, int)> retVal;

            var sqlString =
                $"SELECT Id, ChannelId FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";

            using (var connection = GetConnection())
            {
                retVal = connection.Query<Content>(sqlString).Select(o => (Math.Abs(o.ChannelId), o.Id)).ToList();
            }

            return retVal;
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
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
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
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
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
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
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
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
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
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
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
                var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, totalNum, MinListColumns, whereString, orderByString);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
        }

        private DataSet GetStlDataSourceByStartNum(string tableName, int startNum, int totalNum, string whereString, string orderByString)
        {
            DataSet dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, whereString, orderByString);
                var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, whereString, orderByString, startNum - 1, totalNum);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
        }

        public void AddDownloads(string tableName, int channelId, int contentId)
        {
            var sqlString =
                $"UPDATE {tableName} SET {DataProvider.DatabaseApi.ToPlusSqlString(ContentAttribute.Downloads, 1)} WHERE Id = {contentId}";
            DataProvider.DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlString);

            ContentManager.RemoveCache(tableName, channelId);
        }

        private int GetTaxisToInsert(string tableName, int channelId, bool isTop)
        {
            int taxis;

            if (isTop)
            {
                taxis = GetMaxTaxis(tableName, channelId, true) + 1;
            }
            else
            {
                taxis = GetMaxTaxis(tableName, channelId, false) + 1;
            }

            return taxis;
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

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
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
            var totalCount = 0;
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

            totalCount = DataProvider.DatabaseDao.GetPageTotalCount(tableName, whereString, dbArgs);
            if (totalCount > 0 && parameters.Skip < totalCount)
            {
                var sqlString = DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, whereString, orderString, parameters.Skip, parameters.Top);

                using (var connection = GetConnection())
                {
                    list = connection.Query<Content>(sqlString, dbArgs).ToList();
                }
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

            var likeList = TranslateUtils.StringCollectionToStringList(StringUtils.TrimAndToLower(like));
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

            var totalCount = DataProvider.DatabaseDao.GetPageTotalCount(tableName, whereString, dbArgs);
            if (totalCount > 0 && skip < totalCount)
            {
                var sqlString = DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, whereString, orderString, skip, top);

                using (var connection = GetConnection())
                {
                    retVal = connection.Query<Content>(sqlString, dbArgs).Select(o => (o.ChannelId, o.Id)).ToList();
                }
            }

            return (retVal, totalCount);
        }

        #region Table

        public string GetSelectCommandByHitsAnalysis(string tableName, int siteId)
        {
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var whereString = new StringBuilder();
            whereString.Append($"AND IsChecked = '{true}' AND SiteId = {siteId} AND Hits > 0");
            whereString.Append(orderByString);

            return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public string GetSqlStringOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = $@"select userName,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
SELECT AddUserName as userName, Count(AddUserName) as addCount, 0 as updateCount FROM {tableName} 
INNER JOIN {DataProvider.AdministratorDao.TableName} ON AddUserName = {DataProvider.AdministratorDao.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
GROUP BY AddUserName
Union
SELECT LastEditUserName as userName,0 as addCount, Count(LastEditUserName) as updateCount FROM {tableName} 
INNER JOIN {DataProvider.AdministratorDao.TableName} ON LastEditUserName = {DataProvider.AdministratorDao.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
AND LastEditDate != AddDate
GROUP BY LastEditUserName
) as tmp
group by tmp.userName";


            return sqlString;
        }

        public async Task<string> GetStlWhereStringAsync(int siteId, string group, string groupNot, string tags, bool isTopExists, bool isTop, string where)
        {
            var whereStringBuilder = new StringBuilder();

            if (isTopExists)
            {
                whereStringBuilder.Append($" AND IsTop = '{isTop}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr.Length > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        whereStringBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 3;
                    }
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr.Length > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        //whereStringBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{theGroupNot.Trim()}' AND CHARINDEX('{theGroupNot.Trim()},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{theGroupNot.Trim()},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{theGroupNot.Trim()}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereStringBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{theGroupNot.Trim()}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + theGroupNot.Trim())}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 4;
                    }
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var tagList = ContentTagUtils.ParseTagsString(tags);
                var contentIdList = await DataProvider.ContentTagDao.GetContentIdListByTagCollectionAsync(tagList, siteId);
                if (contentIdList.Count > 0)
                {
                    var inString = TranslateUtils.ToSqlInStringWithoutQuote(contentIdList);
                    whereStringBuilder.Append($" AND (Id IN ({inString}))");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereStringBuilder.Append($" AND ({where}) ");
            }

            return whereStringBuilder.ToString();
        }

        public async Task<string> GetWhereStringByStlSearchAsync(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes, NameValueCollection form)
        {
            var whereBuilder = new StringBuilder();

            Site site = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                site = await SiteManager.GetSiteBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                site = await SiteManager.GetSiteByDirectoryAsync(siteDir);
            }
            if (site == null)
            {
                site = await SiteManager.GetSiteAsync(siteId);
            }

            var channelId = await ChannelManager.GetChannelIdAsync(siteId, siteId, channelIndex, channelName);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);

            if (isAllSites)
            {
                whereBuilder.Append("(SiteId > 0) ");
            }
            else if (!string.IsNullOrEmpty(siteIds))
            {
                whereBuilder.Append($"(SiteId IN ({TranslateUtils.ToSqlInStringWithoutQuote(TranslateUtils.StringCollectionToIntList(siteIds))})) ");
            }
            else
            {
                whereBuilder.Append($"(SiteId = {site.Id}) ");
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                whereBuilder.Append(" AND ");
                var channelIdList = new List<int>();
                foreach (var theChannelId in TranslateUtils.StringCollectionToIntList(channelIds))
                {
                    var theSiteId = await DataProvider.ChannelDao.GetSiteIdAsync(theChannelId);
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

                var theSiteId = await DataProvider.ChannelDao.GetSiteIdAsync(channelId);
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
                typeList = TranslateUtils.StringCollectionToStringList(type);
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

        public string GetStlSqlStringChecked(List<int> channelIdList, string tableName, int siteId, int channelId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
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
                //return DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, sqlWhereString, orderByString);
                return DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, sqlWhereString, orderByString, startNum - 1, totalNum);
            }
            return string.Empty;
        }

        public string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString)
        {
            var sqlWhereString =
                    $"WHERE (ChannelId > 0 AND IsChecked = '{true}' {whereString})";

            if (!string.IsNullOrEmpty(tableName))
            {
                //return DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, TranslateUtils.ObjectCollectionToString(ContentAttribute.AllAttributes.Value), sqlWhereString, orderByString);
                return DataProvider.DatabaseDao.GetPageSqlString(tableName, TranslateUtils.ObjectCollectionToString(ContentAttribute.AllAttributes.Value), sqlWhereString, orderByString, startNum - 1, totalNum);
            }
            return string.Empty;
        }

        public async Task<string> GetStlWhereStringAsync(int siteId, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append($" AND SiteId = {siteId} ");

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {ContentAttribute.ImageUrl} <> '' "
                    : $" AND {ContentAttribute.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {ContentAttribute.VideoUrl} <> '' "
                    : $" AND {ContentAttribute.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {ContentAttribute.FileUrl} <> '' "
                    : $" AND {ContentAttribute.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsColor} = '{isColor}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 3;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 4;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var tagCollection = ContentTagUtils.ParseTagsString(tags);
                var contentIdArrayList = await DataProvider.ContentTagDao.GetContentIdListByTagCollectionAsync(tagCollection, siteId);
                if (contentIdArrayList.Count > 0)
                {
                    whereBuilder.Append(
                        $" AND (ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdArrayList)}))");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereBuilder.Append($" AND ({where}) ");
            }

            return whereBuilder.ToString();
        }

        public string GetStlWhereStringBySearch(string group, string groupNot, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {ContentAttribute.ImageUrl} <> '' "
                    : $" AND {ContentAttribute.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {ContentAttribute.VideoUrl} <> '' "
                    : $" AND {ContentAttribute.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {ContentAttribute.FileUrl} <> '' "
                    : $" AND {ContentAttribute.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsColor} = '{isColor}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{AttackUtils.FilterSql(trimGroup)}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 3;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 4;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereBuilder.Append($" AND ({where}) ");
            }

            return whereBuilder.ToString();
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

            return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
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

            return $"WHERE {string.Join(" AND ", whereList)}";
        }

        private string GetCreateContentTableSqlString(string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder(DataProvider.DatabaseDao.GetCreateTableSqlString(tableName, tableColumns));
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

        public void CreateContentTable(string tableName, List<TableColumn> columnInfoList)
        {
            var isDbExists = DataProvider.DatabaseDao.IsTableExists(tableName);
            if (isDbExists) return;

            var createTableSqlString = GetCreateContentTableSqlString(tableName, columnInfoList);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
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
            }
        }

        #endregion

        //public async Task<string> GetCacheWhereStringAsync(Site site, Channel channel, int adminId, bool isAllContents)
        //{
        //    var whereString = $"WHERE {ContentAttribute.SiteId} = {site.Id} AND {nameof(ContentAttribute.SourceId)} != {SourceManager.Preview}";

        //    if (adminId > 0)
        //    {
        //        whereString += $" AND {nameof(ContentAttribute.AdminId)} = {adminId}";
        //    }

        //    if (isAllContents)
        //    {
        //        var channelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.All);
        //        whereString += $" AND {ContentAttribute.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(channelIdList)}) ";
        //    }
        //    else
        //    {
        //        whereString += $" AND {ContentAttribute.ChannelId} = {channel.Id} ";
        //    }

        //    return whereString;
        //}

        public string GetOrderString(Channel channel, string orderBy, bool isAllContents)
        {
            return isAllContents
                ? ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByIdDesc)
                : ETaxisTypeUtils.GetContentOrderByString(
                    ETaxisTypeUtils.GetEnumType(channel.DefaultTaxisType), orderBy);
        }

        //public async Task<List<Content>> GetContentInfoListAsync(string tableName, string whereString, string orderString, int offset, int limit)
        //{
        //    var list = new List<Content>();

        //    var sqlString = DataProvider.DatabaseDao.GetPageSqlString(tableName, SqlUtils.Asterisk, whereString, orderString, offset, limit);

        //    using (var rdr = ExecuteReader(sqlString))
        //    {
        //        while (rdr.Read())
        //        {
        //            var contentInfo = new Content(rdr);

        //            list.Add(contentInfo);
        //        }
        //        rdr.Close();
        //    }

        //    return list;
        //}

        //public List<(int ChannelId, int ContentId)> GetCacheChannelContentIdList(string tableName, string whereString, string orderString, int offset, int limit)
        //{
        //    var list = new List<(int ChannelId, int ContentId)>();

        //    var sqlString = DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, whereString, orderString, offset, limit);

        //    using (var rdr = ExecuteReader(sqlString))
        //    {
        //        while (rdr.Read())
        //        {
        //            var contentId = GetInt(rdr, 0);
        //            var channelId = GetInt(rdr, 1);

        //            list.Add((channelId, contentId));
        //        }
        //        rdr.Close();
        //    }

        //    return list;
        //}

        public async Task<Content> GetContentAsync(string tableName, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

            var repository = new Repository<Content>(_db, tableName);

            return await repository.GetAsync(contentId);
        }

        // new

        public async Task QueryWhereAsync(Query query, Site site, Channel channel, int adminId, bool isAllContents)
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

        public void QueryOrder(Query query, Channel channel, string orderBy, bool isAllContents)
        {
            QueryOrder(query, isAllContents
                    ? ETaxisType.OrderByIdDesc
                    : ETaxisTypeUtils.GetEnumType(channel.DefaultTaxisType), orderBy);
        }

        public void QueryOrder(Query query, ETaxisType taxisType, string orderByString = null)
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

        public async Task<List<Content>> GetContentListAsync(string tableName, Query query)
        {
            var repository = new Repository<Content>(_db, tableName);
            var contentList = await repository.GetAllAsync(query);
            return contentList.ToList();
        }

        public async Task<IEnumerable<IContentMin>> GetContentMinListAsync(string tableName, Query query)
        {
            var repository = new Repository<Content>(_db, tableName);
            var q = query.Clone();
            q.Select(MinColumns);
            return await repository.GetAllAsync<ContentMin>(q);
        }
    }
}
