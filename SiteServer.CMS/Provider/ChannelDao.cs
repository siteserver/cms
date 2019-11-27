using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class ChannelDao : DataProviderBase, IRepository
    {
        private string SqlColumns => $"{nameof(Channel.Id)}, {nameof(Channel.AddDate)}, {nameof(Channel.Taxis)}";

        private readonly Repository<Channel> _repository;

        public ChannelDao()
        {
            _repository = new Repository<Channel>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private async Task InsertChannelAsync(Channel parentChannel, Channel channel)
        {
            if (parentChannel != null)
            {
                channel.SiteId = parentChannel.SiteId;
                if (parentChannel.ParentsPath.Length == 0)
                {
                    channel.ParentsPath = parentChannel.Id.ToString();
                }
                else
                {
                    channel.ParentsPath = parentChannel.ParentsPath + "," + parentChannel.Id;
                }
                channel.ParentsCount = parentChannel.ParentsCount + 1;

                var maxTaxis = await GetMaxTaxisByParentPathAsync(channel.ParentsPath);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentChannel.Taxis;
                }
                channel.Taxis = maxTaxis + 1;
            }
            else
            {
                channel.Taxis = 1;
            }

            channel.ChildrenCount = 0;
            channel.LastNode = true;

            if (channel.SiteId != 0)
            {
                await _repository.IncrementAsync(nameof(Channel.Taxis), Q
                    .Where(nameof(Channel.Taxis), ">=", channel.Taxis)
                    .Where(nameof(Channel.SiteId), channel.SiteId)
                );
            }
            channel.Id = await _repository.InsertAsync(channel);

            if (!string.IsNullOrEmpty(channel.ParentsPath))
            {
                await _repository.IncrementAsync(nameof(Channel.ChildrenCount), Q
                    .WhereIn(nameof(Channel.Id), TranslateUtils.StringCollectionToIntList(channel.ParentsPath))
                );
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.IsLastNode), false.ToString())
                .Where(nameof(Channel.ParentId), channel.ParentId)
            );

            var topId = await _repository.GetAsync<int>(Q
                .Select(nameof(Channel.Id))
                .Where(nameof(Channel.ParentId), channel.ParentId)
                .OrderByDesc(nameof(Channel.Taxis))
            );

            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.IsLastNode), true.ToString())
                .Where(nameof(Channel.Id), topId)
            );

            ChannelManager.RemoveCacheBySiteId(channel.SiteId);
            PermissionsImpl.ClearAllCache();
        }

        private async Task UpdateSubtractChildrenCountAsync(string parentsPath, int subtractNum)
        {
            if (string.IsNullOrEmpty(parentsPath)) return;

            await _repository.DecrementAsync(nameof(Channel.ChildrenCount), Q
                    .WhereIn(nameof(Channel.Id), TranslateUtils.StringCollectionToIntList(parentsPath))
                , subtractNum);
        }

        private async Task TaxisSubtractAsync(int siteId, int selectedId)
        {
            var channelEntity = await ChannelManager.GetChannelAsync(siteId, selectedId);
            if (channelEntity == null || channelEntity.ParentId == 0 || channelEntity.SiteId == 0) return;

            //UpdateWholeTaxisBySiteId(channel.SiteId);

            var lower = await _repository.GetAsync<(int Id, int ChildrenCount, string ParentsPath)?>(Q
                .Select(nameof(Channel.Id), nameof(Channel.ChildrenCount), nameof(Channel.ParentsPath))
                .Where(nameof(Channel.ParentId), channelEntity.ParentId)
                .Where(nameof(Channel.SiteId), channelEntity.SiteId)
                .WhereNot(nameof(Channel.Id), channelEntity.Id)
                .Where(nameof(Channel.Taxis), "<", channelEntity.Taxis)
                .OrderByDesc(nameof(Channel.Taxis))
            );

            if (lower == null) return;

            var lowerPath = lower.Value.ParentsPath == "" ? lower.Value.Id.ToString() : string.Concat(lower.Value.ParentsPath, ",", lower.Value.Id);
            var selectedPath = channelEntity.ParentsPath == "" ? channelEntity.Id.ToString() : string.Concat(channelEntity.ParentsPath, ",", channelEntity.Id);

            await SetTaxisSubtractAsync(selectedId, selectedPath, lower.Value.ChildrenCount + 1);
            await SetTaxisAddAsync(lower.Value.Id, lowerPath, channelEntity.ChildrenCount + 1);

            await UpdateIsLastNodeAsync(channelEntity.ParentId);
        }

        private async Task TaxisAddAsync(int siteId, int selectedId)
        {
            var channelEntity = await ChannelManager.GetChannelAsync(siteId, selectedId);
            if (channelEntity == null || channelEntity.ParentId == 0 || channelEntity.SiteId == 0) return;

            //UpdateWholeTaxisBySiteId(channel.SiteId);

            var higher = await _repository.GetAsync<(int Id, int ChildrenCount, string ParentsPath)?>(Q
                .Select(nameof(Channel.Id), nameof(Channel.ChildrenCount), nameof(Channel.ParentsPath))
                .Where(nameof(Channel.ParentId), channelEntity.ParentId)
                .Where(nameof(Channel.SiteId), channelEntity.SiteId)
                .WhereNot(nameof(Channel.Id), channelEntity.Id)
                .Where(nameof(Channel.Taxis), ">", channelEntity.Taxis)
                .OrderBy(nameof(Channel.Taxis))
            );

            if (higher == null) return;

            var higherPath = higher.Value.ParentsPath == string.Empty ? higher.Value.Id.ToString() : string.Concat(higher.Value.ParentsPath, ",", higher.Value.Id);
            var selectedPath = channelEntity.ParentsPath == string.Empty ? channelEntity.Id.ToString() : String.Concat(channelEntity.ParentsPath, ",", channelEntity.Id);

            await SetTaxisAddAsync(selectedId, selectedPath, higher.Value.ChildrenCount + 1);
            await SetTaxisSubtractAsync(higher.Value.Id, higherPath, channelEntity.ChildrenCount + 1);

            await UpdateIsLastNodeAsync(channelEntity.ParentId);
        }

        private async Task SetTaxisAddAsync(int channelId, string parentsPath, int addNum)
        {
            await _repository.IncrementAsync(nameof(Channel.Taxis), Q
                    .Where(nameof(Channel.Id), channelId)
                    .OrWhere(nameof(Channel.ParentsPath), parentsPath)
                    .OrWhereStarts(nameof(Channel.ParentsPath), $"{parentsPath},")
                , addNum);
        }

        private async Task SetTaxisSubtractAsync(int channelId, string parentsPath, int subtractNum)
        {
            await _repository.DecrementAsync(nameof(Channel.Taxis), Q
                    .Where(nameof(Channel.Id), channelId)
                    .OrWhere(nameof(Channel.ParentsPath), parentsPath)
                    .OrWhereStarts(nameof(Channel.ParentsPath), $"{parentsPath},")
                , subtractNum);
        }

        private async Task UpdateIsLastNodeAsync(int parentId)
        {
            if (parentId <= 0) return;

            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.IsLastNode), false.ToString())
                .Where(nameof(Channel.ParentId), parentId)
            );

            var topId = await _repository.GetAsync<int>(Q
                .Where(nameof(Channel.ParentId), parentId)
                .OrderByDesc(nameof(Channel.Taxis))
            );

            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.IsLastNode), true.ToString())
                .Where(nameof(Channel.Id), topId)
            );
        }

        private async Task<int> GetMaxTaxisByParentPathAsync(string parentPath)
        {
            return await _repository.MaxAsync(nameof(Channel.Taxis), Q
                       .Where(nameof(Channel.ParentsPath), parentPath)
                       .OrWhereStarts(nameof(Channel.ParentsPath), $"{parentPath},")
                   ) ?? 0;
        }

        private async Task<int> GetParentIdAsync(int channelId)
        {
            return await _repository.GetAsync<int>(Q
                .Select(nameof(Channel.ParentId))
                .Where(nameof(Channel.Id), channelId)
            );
        }

        private async Task<int> GetIdByParentIdAndOrderAsync(int parentId, int order)
        {
            var channelId = parentId;

            var list = await _repository.GetAllAsync<int>(Q
                .Select(nameof(Channel.Id))
                .Where(nameof(Channel.ParentId), parentId)
                .OrderBy(nameof(Channel.Taxis))
            );

            var index = 1;
            foreach (var id in list)
            {
                channelId = id;
                if (index == order)
                    break;
                index++;
            }
            return channelId;
        }

        public async Task<int> InsertAsync(int siteId, int parentId, string channelName, string indexName, string contentModelPluginId, List<string> contentRelatedPluginIds, int channelTemplateId, int contentTemplateId)
        {
            if (siteId > 0 && parentId == 0) return 0;

            var defaultChannelTemplateEntity = await TemplateManager.GetDefaultTemplateAsync(siteId, TemplateType.ChannelTemplate);
            var defaultContentTemplateEntity = await TemplateManager.GetDefaultTemplateAsync(siteId, TemplateType.ContentTemplate);

            var channelEntity = new Channel
            {
                ParentId = parentId,
                SiteId = siteId,
                ChannelName = channelName,
                IndexName = indexName,
                ContentModelPluginId = contentModelPluginId,
                ContentRelatedPluginIdList = contentRelatedPluginIds,
                AddDate = DateTime.Now,
                ChannelTemplateId = channelTemplateId > 0 ? channelTemplateId : defaultChannelTemplateEntity.Id,
                ContentTemplateId = contentTemplateId > 0 ? contentTemplateId : defaultContentTemplateEntity.Id
            };

            var parentChannel = await ChannelManager.GetChannelAsync(siteId, parentId);

            await InsertChannelAsync(parentChannel, channelEntity);

            return channelEntity.Id;

        }

        public async Task<int> InsertAsync(Channel channel)
        {
            if (channel.SiteId > 0 && channel.ParentId == 0) return 0;

            var parentChannel = await ChannelManager.GetChannelAsync(channel.SiteId, channel.ParentId);

            await InsertChannelAsync(parentChannel, channel);

            return channel.Id;
        }

        /// <summary>
        /// 添加后台发布节点
        /// </summary>
        public async Task<int> InsertSiteAsync(Channel channel, Site site, string administratorName)
        {
            await InsertChannelAsync(null, channel);

            site.Id = channel.Id;

            await DataProvider.SiteDao.InsertAsync(site);

            var adminEntity = await DataProvider.AdministratorDao.GetByUserNameAsync(administratorName);
            await DataProvider.AdministratorDao.UpdateSiteIdAsync(adminEntity, channel.Id);

            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.SiteId), channel.Id)
                .Where(nameof(Channel.Id), channel.Id)
            );

            await DataProvider.TemplateDao.CreateDefaultTemplateAsync(channel.Id, administratorName);

            return channel.Id;
        }

        public async Task UpdateAsync(Channel channel)
        {
            await _repository.UpdateAsync(channel);

            await ChannelManager.UpdateCacheAsync(channel.SiteId, channel);

            //ChannelManager.RemoveCache(channel.ParentId == 0
            //    ? channel.Id
            //    : channel.SiteId);
        }

        public async Task UpdateChannelTemplateIdAsync(Channel channel)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.ChannelTemplateId), channel.ChannelTemplateId)
                .Where(nameof(Channel.Id), channel.Id)
            );

            await ChannelManager.UpdateCacheAsync(channel.SiteId, channel);
        }

        public async Task UpdateContentTemplateIdAsync(Channel channel)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.ContentTemplateId), channel.ContentTemplateId)
                .Where(nameof(Channel.Id), channel.Id)
            );

            await ChannelManager.UpdateCacheAsync(channel.SiteId, channel);
        }

        public async Task UpdateAdditionalAsync(Channel channel)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.ExtendValues), channel.ToString())
                .Where(nameof(Channel.Id), channel.Id)
            );

            await ChannelManager.UpdateCacheAsync(channel.SiteId, channel);

            //ChannelManager.RemoveCache(channel.ParentId == 0
            //    ? channel.Id
            //    : channel.SiteId);
        }

        /// <summary>
        /// 修改排序
        /// </summary>
        public async Task UpdateTaxisAsync(int siteId, int selectedId, bool isSubtract)
        {
            if (isSubtract)
            {
                await TaxisSubtractAsync(siteId, selectedId);
            }
            else
            {
                await TaxisAddAsync(siteId, selectedId);
            }
            ChannelManager.RemoveCacheBySiteId(siteId);
        }

        public async Task AddGroupNameListAsync(int siteId, int channelId, List<string> groupList)
        {
            var channelEntity = await ChannelManager.GetChannelAsync(siteId, channelId);
            if (channelEntity == null) return;

            foreach (var groupName in groupList)
            {
                if (!channelEntity.GroupNames.Contains(groupName)) channelEntity.GroupNames.Add(groupName);
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.GroupNameCollection), string.Join(",", channelEntity.GroupNames))
                .Where(nameof(Channel.Id), channelId)
            );

            await ChannelManager.UpdateCacheAsync(siteId, channelEntity);
        }

        public async Task DeleteAsync(int siteId, int channelId)
        {
            var channelEntity = await ChannelManager.GetChannelAsync(siteId, channelId);
            if (channelEntity == null) return;

            var site = await DataProvider.SiteDao.GetAsync(siteId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channelEntity);
            var idList = new List<int>();
            if (channelEntity.ChildrenCount > 0)
            {
                idList = await ChannelManager.GetChannelIdListAsync(channelEntity, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
            }
            idList.Add(channelId);

            var deletedNum = await _repository.DeleteAsync(Q
                .Where(nameof(Channel.SiteId), siteId)
                .WhereIn(nameof(Channel.Id), idList)
            );

            if (channelEntity.ParentId != 0)
            {
                await _repository.DecrementAsync(nameof(Channel.Taxis), Q
                    .Where(nameof(Channel.SiteId), channelEntity.SiteId)
                    .Where(nameof(Channel.Taxis), ">", channelEntity.Taxis)
                , deletedNum);
            }

            await UpdateIsLastNodeAsync(channelEntity.ParentId);
            await UpdateSubtractChildrenCountAsync(channelEntity.ParentsPath, deletedNum);

            foreach (var channelIdDeleted in idList)
            {
                await DataProvider.ContentDao.UpdateTrashContentsByChannelIdAsync(site.Id, channelIdDeleted, tableName);
            }
            //DataProvider.ContentDao.DeleteContentsByDeletedChannelIdList(trans, site, idList);

            if (channelEntity.ParentId == 0)
            {
                await DataProvider.SiteDao.DeleteAsync(channelEntity.Id);
            }
            else
            {
                ChannelManager.RemoveCacheBySiteId(channelEntity.SiteId);
            }
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(Channel.SiteId), siteId)
                .OrWhere(nameof(Channel.Id), siteId)
            );
        }

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        public async Task<Channel> GetChannelByLastAddDateAsyncTask(int channelId)
        {
            var channel = await _repository.GetAsync(Q
                .Where(nameof(Channel.ParentId), channelId)
                .OrderByDesc(nameof(Channel.AddDate))
            );
            return channel;
        }

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        public async Task<Channel> GetChannelByTaxisAsync(int channelId)
        {
            var channel = await _repository.GetAsync(Q
                .Where(nameof(Channel.ParentId), channelId)
                .OrderBy(nameof(Channel.Taxis))
            );
            return channel;
        }

        public async Task<int> GetIdByParentIdAndTaxisAsync(int parentId, int taxis, bool isNextChannel)
        {
            var channelId = 0;

            if (isNextChannel)
            {
                channelId = await _repository.GetAsync<int>(Q
                    .Select(nameof(Channel.Id))
                    .Where(nameof(Channel.ParentId), parentId)
                    .Where(nameof(Channel.Taxis), ">", taxis)
                    .OrderBy(nameof(Channel.Taxis))
                );
            }
            else
            {
                channelId = await _repository.GetAsync<int>(Q
                    .Select(nameof(Channel.Id))
                    .Where(nameof(Channel.ParentId), parentId)
                    .Where(nameof(Channel.Taxis), "<", taxis)
                    .OrderByDesc(nameof(Channel.Taxis))
                );
            }

            return channelId;
        }

        public async Task<int> GetIdAsync(int siteId, string orderString)
        {
            if (orderString == "1")
                return siteId;

            var channelId = siteId;

            var orderArr = orderString.Split('_');
            for (var index = 1; index < orderArr.Length; index++)
            {
                var order = int.Parse(orderArr[index]);
                channelId = await GetIdByParentIdAndOrderAsync(channelId, order);
            }
            return channelId;
        }

        public async Task<int> GetSiteIdAsync(int channelId)
        {
            return await _repository.GetAsync<int>(Q
                .Select(nameof(Channel.SiteId))
                .Where(nameof(Channel.Id), channelId)
            );
        }

        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        public async Task<string> GetOrderStringInSiteAsync(int channelId)
        {
            var retVal = "";
            if (channelId != 0)
            {
                var parentId = await GetParentIdAsync(channelId);
                if (parentId != 0)
                {
                    var orderString = await GetOrderStringInSiteAsync(parentId);
                    retVal = orderString + "_" + await GetOrderInSiblingAsync(channelId, parentId);
                }
                else
                {
                    retVal = "1";
                }
            }
            return retVal;
        }

        private async Task<int> GetOrderInSiblingAsync(int channelId, int parentId)
        {
            var idList = (await _repository.GetAllAsync<int>(Q
                .Select(nameof(Channel.Id))
                .Where(nameof(Channel.ParentId), parentId)
                .OrderBy(nameof(Channel.Taxis))
            )).ToList();
            return idList.IndexOf(channelId) + 1;
        }

        public async Task<IEnumerable<string>> GetIndexNameListAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Channel.IndexName))
                .Where(nameof(Channel.SiteId), siteId)
                .Distinct()
            );
        }

        private static string GetGroupWhereString(string group, string groupNot)
        {
            var whereStringBuilder = new StringBuilder();
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
                                $" (siteserver_Channel.GroupNameCollection = '{trimGroup}' OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", "," + trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", "," + trimGroup)}) OR ");
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
                        var trimGroupNot = AttackUtils.FilterSql(theGroupNot.Trim());
                        //whereStringBuilder.Append(
                        //    $" (siteserver_Channel.GroupNameCollection <> '{trimGroupNot}' AND CHARINDEX('{trimGroupNot},',siteserver_Channel.GroupNameCollection) = 0 AND CHARINDEX(',{trimGroupNot},',siteserver_Channel.GroupNameCollection) = 0 AND CHARINDEX(',{trimGroupNot}',siteserver_Channel.GroupNameCollection) = 0) AND ");

                        whereStringBuilder.Append(
                                $" (siteserver_Channel.GroupNameCollection <> '{trimGroupNot}' AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", "," + trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", "," + trimGroupNot)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 4;
                    }
                    whereStringBuilder.Append(") ");
                }
            }
            return whereStringBuilder.ToString();
        }

        public string GetWhereString(string group, string groupNot, bool isImageExists, bool isImage, string where)
        {
            var whereStringBuilder = new StringBuilder();
            if (isImageExists)
            {
                whereStringBuilder.Append(isImage
                    ? " AND siteserver_Channel.ImageUrl <> '' "
                    : " AND siteserver_Channel.ImageUrl = '' ");
            }

            whereStringBuilder.Append(GetGroupWhereString(group, groupNot));

            if (!string.IsNullOrEmpty(where))
            {
                whereStringBuilder.Append($" AND {where} ");
            }

            return whereStringBuilder.ToString();
        }

        public async Task<int> GetCountAsync(int channelId)
        {
            return await _repository.CountAsync(Q.Where(nameof(Channel.ParentId), channelId));
        }

        public async Task<int> GetSequenceAsync(int siteId, int channelId)
        {
            var channelEntity = await ChannelManager.GetChannelAsync(siteId, channelId);
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM siteserver_Channel WHERE SiteId = {siteId} AND ParentId = {channelEntity.ParentId} AND Taxis > (SELECT Taxis FROM siteserver_Channel WHERE Id = {channelId})";

            return DataProvider.DatabaseDao.GetIntResult(sqlString) + 1;
        }

        public async Task<IEnumerable<int>> GetIdListByTotalNumAsync(List<int> channelIdList, int totalNum, string orderByString, string whereString)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return channelIdList;
            }

            string sqlString;
            if (totalNum > 0)
            {
                var where =
                    $"WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString})";
                sqlString = SqlUtils.ToTopSqlString(TableName, "Id",
                    where,
                    orderByString,
                    totalNum);

                //return await _repository.GetAllAsync<int>(Q
                //    .Select(nameof(Channel.Id))
                //    .Limit(totalNum)
                //);
            }
            else
            {
                sqlString = $@"SELECT Id
FROM siteserver_Channel 
WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString} {orderByString}
";
            }

            var list = new List<int>();

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

        public async Task<Dictionary<int, Channel>> GetChannelDictionaryBySiteIdAsync(int siteId)
        {
            var channelList = await _repository.GetAllAsync(Q
                .Where(nameof(Channel.SiteId), siteId)
                .Where(q => q
                    .Where(nameof(Channel.Id), siteId)
                    .OrWhere(nameof(Channel.ParentId), ">", 0)
                )
                .OrderBy(nameof(Channel.Taxis))
            );

            return channelList.ToDictionary(channel => channel.Id);
        }

        public DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

            var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return ExecuteDataset(sqlSelect);
        }

        public DataSet GetStlDataSet(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }

            //var sqlWhereString =
            //    $"WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString})";

            var sqlWhereString =
                $"WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString}";

            //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return ExecuteDataset(sqlSelect);
        }

        public DataSet GetStlDataSetBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

            //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return ExecuteDataset(sqlSelect);
        }

        public async Task<IEnumerable<string>> GetContentModelPluginIdListAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Channel.ContentModelPluginId))
                .Distinct()
            );
        }

        public async Task<IEnumerable<string>> GetAllFilePathBySiteIdAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Channel.FilePath))
                .WhereNotNull(nameof(Channel.FilePath))
                .WhereNot(nameof(Channel.FilePath), string.Empty)
                .Where(nameof(Channel.SiteId), siteId)
            );
        }

        public int GetTemplateUseCount(int siteId, int templateId, TemplateType templateType, bool isDefault)
        {
            var sqlString = string.Empty;

            if (templateType == TemplateType.IndexPageTemplate)
            {
                if (isDefault)
                {
                    return 1;
                }
                return 0;
            }
            if (templateType == TemplateType.FileTemplate)
            {
                return 1;
            }
            if (templateType == TemplateType.ChannelTemplate)
            {
                sqlString = isDefault
                    ? $"SELECT COUNT(*) FROM {TableName} WHERE ({nameof(Channel.ChannelTemplateId)} = {templateId} OR {nameof(Channel.ChannelTemplateId)} = 0) AND {nameof(Channel.SiteId)} = {siteId}"
                    : $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(Channel.ChannelTemplateId)} = {templateId} AND {nameof(Channel.SiteId)} = {siteId}";
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                sqlString = isDefault
                    ? $"SELECT COUNT(*) FROM {TableName} WHERE ({nameof(Channel.ContentTemplateId)} = {templateId} OR {nameof(Channel.ContentTemplateId)} = 0) AND {nameof(Channel.SiteId)} = {siteId}"
                    : $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(Channel.ContentTemplateId)} = {templateId} AND {nameof(Channel.SiteId)} = {siteId}";
            }

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public async Task<IEnumerable<int>> GetChannelIdListAsync(Template template)
        {
            if (template.Type != TemplateType.ChannelTemplate &&
                template.Type != TemplateType.ContentTemplate)
            {
                return new List<int>();
            }

            if (template.Type == TemplateType.ChannelTemplate)
            {
                if (template.Default)
                {
                    return await _repository.GetAllAsync<int>(Q
                        .Select(nameof(Channel.Id))
                        .Where(nameof(Channel.SiteId), template.SiteId)
                        .Where(q => q
                            .Where(nameof(Channel.ChannelTemplateId), template.Id)
                            .OrWhere(nameof(Channel.ChannelTemplateId), 0)
                        )
                    );
                }

                return await _repository.GetAllAsync<int>(Q
                    .Select(nameof(Channel.Id))
                    .Where(nameof(Channel.SiteId), template.SiteId)
                    .Where(nameof(Channel.ChannelTemplateId), template.Id)
                );
            }
            
            if (template.Default)
            {
                return await _repository.GetAllAsync<int>(Q
                    .Select(nameof(Channel.Id))
                    .Where(nameof(Channel.SiteId), template.SiteId)
                    .Where(q => q
                        .Where(nameof(Channel.ContentTemplateId), template.Id)
                        .OrWhere(nameof(Channel.ContentTemplateId), 0)
                    )
                );
            }

            return await _repository.GetAllAsync<int>(Q
                .Select(nameof(Channel.Id))
                .Where(nameof(Channel.SiteId), template.SiteId)
                .Where(nameof(Channel.ContentTemplateId), template.Id)
            );
        }

        /// 更新发布系统下的所有节点的排序号
        //private void UpdateWholeTaxisBySiteId(int siteId)
        //{
        //    if (siteId <= 0) return;
        //    var idList = new List<int>
        //    {
        //        siteId
        //    };
        //    var level = 0;
        //    string selectLevelCmd =
        //        $"SELECT MAX(ParentsCount) FROM siteserver_Channel WHERE (Id = {siteId}) OR (SiteId = {siteId})";
        //    using (var rdr = ExecuteReader(selectLevelCmd))
        //    {
        //        while (rdr.Read())
        //        {
        //            var parentsCount = GetInt(rdr, 0);
        //            level = parentsCount;
        //        }
        //        rdr.Close();
        //    }

        //    for (var i = 0; i < level; i++)
        //    {
        //        var list = new List<int>(idList);
        //        foreach (var savedId in list)
        //        {
        //            var lastChildIdOfSavedId = savedId;
        //            var sqlString =
        //                $"SELECT Id, ChannelName FROM siteserver_Channel WHERE ParentId = {savedId} ORDER BY Taxis, LastNode";
        //            using (var rdr = ExecuteReader(sqlString))
        //            {
        //                while (rdr.Read())
        //                {
        //                    var channelId = GetInt(rdr, 0);
        //                    if (!idList.Contains(channelId))
        //                    {
        //                        var index = idList.IndexOf(lastChildIdOfSavedId);
        //                        idList.Insert(index + 1, channelId);
        //                        lastChildIdOfSavedId = channelId;
        //                    }
        //                }
        //                rdr.Close();
        //            }
        //        }
        //    }

        //    for (var i = 1; i <= idList.Count; i++)
        //    {
        //        var channelId = idList[i - 1];
        //        string updateCmd = $"UPDATE siteserver_Channel SET Taxis = {i} WHERE Id = {channelId}";
        //        ExecuteNonQuery(updateCmd);
        //    }
        //}
    }
}
