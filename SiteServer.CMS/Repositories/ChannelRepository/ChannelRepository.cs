using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Datory;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Repositories
{
    public partial class ChannelRepository : DataProviderBase, IRepository
    {
        private string SqlColumns => $"{nameof(Channel.Id)}, {nameof(Channel.AddDate)}, {nameof(Channel.Taxis)}";

        private readonly Repository<Channel> _repository;

        public ChannelRepository()
        {
            _repository = new Repository<Channel>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString), new Redis(WebConfigUtils.RedisConnectionString));
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

                var maxTaxis = await GetMaxTaxisAsync(channel.SiteId, channel.ParentId);
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

            if (channel.SiteId != 0)
            {
                await _repository.IncrementAsync(nameof(Channel.Taxis), Q
                    .Where(nameof(Channel.Taxis), ">=", channel.Taxis)
                    .Where(nameof(Channel.SiteId), channel.SiteId)
                );
            }

            channel.Id = await _repository.InsertAsync(channel, Q
                .CachingRemove(GetListKey(channel.SiteId))
            );

            if (!string.IsNullOrEmpty(channel.ParentsPath))
            {
                await _repository.IncrementAsync(nameof(Channel.ChildrenCount), Q
                    .WhereIn(nameof(Channel.Id), Utilities.GetIntList(channel.ParentsPath))
                );
            }

            PermissionsImpl.ClearAllCache();
        }

        private async Task TaxisSubtractAsync(int siteId, int selectedId)
        {
            var channelEntity = await GetAsync(selectedId);
            if (channelEntity == null || channelEntity.ParentId == 0 || channelEntity.SiteId == 0) return;

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
        }

        private async Task TaxisAddAsync(int siteId, int selectedId)
        {
            var channelEntity = await GetAsync(selectedId);
            if (channelEntity == null || channelEntity.ParentId == 0 || channelEntity.SiteId == 0) return;

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
            var selectedPath = channelEntity.ParentsPath == string.Empty ? channelEntity.Id.ToString() : string.Concat(channelEntity.ParentsPath, ",", channelEntity.Id);

            await SetTaxisAddAsync(selectedId, selectedPath, higher.Value.ChildrenCount + 1);
            await SetTaxisSubtractAsync(higher.Value.Id, higherPath, channelEntity.ChildrenCount + 1);
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

        public async Task<int> InsertAsync(int siteId, int parentId, string channelName, string indexName, string contentModelPluginId, List<string> contentRelatedPluginIds, int channelTemplateId, int contentTemplateId)
        {
            if (siteId > 0 && parentId == 0) return 0;

            var defaultChannelTemplateEntity = await DataProvider.TemplateRepository.GetDefaultTemplateAsync(siteId, TemplateType.ChannelTemplate);
            var defaultContentTemplateEntity = await DataProvider.TemplateRepository.GetDefaultTemplateAsync(siteId, TemplateType.ContentTemplate);

            var channelEntity = new Channel
            {
                ParentId = parentId,
                SiteId = siteId,
                ChannelName = channelName,
                IndexName = indexName,
                ContentModelPluginId = contentModelPluginId,
                ContentRelatedPluginIds = contentRelatedPluginIds,
                AddDate = DateTime.Now,
                ChannelTemplateId = channelTemplateId > 0 ? channelTemplateId : defaultChannelTemplateEntity.Id,
                ContentTemplateId = contentTemplateId > 0 ? contentTemplateId : defaultContentTemplateEntity.Id
            };

            var parentChannel = await GetAsync(parentId);
            if (parentChannel != null)
            {
                if (parentChannel.DefaultTaxisType != TaxisType.OrderByTaxisDesc)
                {
                    channelEntity.DefaultTaxisType = parentChannel.DefaultTaxisType;
                }
            }

            await InsertChannelAsync(parentChannel, channelEntity);

            return channelEntity.Id;

        }

        public async Task<int> InsertAsync(Channel channel)
        {
            if (channel.SiteId > 0 && channel.ParentId == 0) return 0;

            var parentChannel = await GetAsync(channel.ParentId);

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

            await DataProvider.SiteRepository.InsertAsync(site);

            var adminEntity = await DataProvider.AdministratorRepository.GetByUserNameAsync(administratorName);
            await DataProvider.AdministratorRepository.UpdateSiteIdAsync(adminEntity, channel.Id);

            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.SiteId), channel.Id)
                .Where(nameof(Channel.Id), channel.Id)
            );

            await DataProvider.TemplateRepository.CreateDefaultTemplateAsync(site, administratorName);

            return channel.Id;
        }

        public async Task UpdateAsync(Channel channel)
        {
            await _repository.UpdateAsync(channel, Q
                .CachingRemove(GetListKey(channel.SiteId))
                .CachingRemove(GetEntityKey(channel.Id))
            );
        }

        public async Task UpdateChannelTemplateIdAsync(Channel channel)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.ChannelTemplateId), channel.ChannelTemplateId)
                .Where(nameof(Channel.Id), channel.Id)
            );
        }

        public async Task UpdateContentTemplateIdAsync(Channel channel)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.ContentTemplateId), channel.ContentTemplateId)
                .Where(nameof(Channel.Id), channel.Id)
            );
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
        }

        public async Task SetGroupNamesAsync(int siteId, int channelId, List<string> groupNames)
        {
            var channelEntity = await GetAsync(channelId);
            if (channelEntity == null) return;

            channelEntity.GroupNames = groupNames;

            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.GroupNames), Utilities.ToString(channelEntity.GroupNames))
                .Where(nameof(Channel.Id), channelId)
                .CachingRemove(GetEntityKey(channelId))
            );
        }

        public async Task DeleteAsync(int siteId, int channelId)
        {
            var channelEntity = await GetAsync(channelId);
            if (channelEntity == null) return;

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            var idList = new List<int>();
            if (channelEntity.ChildrenCount > 0)
            {
                idList = await GetChannelIdsAsync(channelEntity, EScopeType.Descendant);
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

            if (!string.IsNullOrEmpty(channelEntity.ParentsPath))
            {
                await _repository.DecrementAsync(nameof(Channel.ChildrenCount), Q
                        .WhereIn(nameof(Channel.Id), Utilities.GetIntList(channelEntity.ParentsPath))
                    , deletedNum);
            }

            foreach (var channelIdDeleted in idList)
            {
                var channelDeleted = await DataProvider.ChannelRepository.GetAsync(channelIdDeleted);
                await DataProvider.ContentRepository.RecycleContentsAsync(site, channelDeleted);
            }
            //DataProvider.ContentRepository.DeleteContentsByDeletedChannelIdList(trans, site, idList);

            if (channelEntity.ParentId == 0)
            {
                await DataProvider.SiteRepository.DeleteAsync(channelEntity.Id);
            }
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(Channel.SiteId), siteId)
                .OrWhere(nameof(Channel.Id), siteId)
            );
        }

        public async Task<bool> IsFilePathExistsAsync(int siteId, string filePath)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(Channel.SiteId), siteId)
                .Where(nameof(Channel.FilePath), filePath)
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

        public async Task<List<int>> GetIdListByTotalNumAsync(List<int> channelIdList, int totalNum, string orderByString, string whereString)
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

            using (var connection = _repository.Database.GetConnection())
            {
                using var rdr = await connection.ExecuteReaderAsync(sqlString);
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

            var sqlSelect = DataProvider.DatabaseRepository.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

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

            //var sqlSelect = DataProvider.DatabaseRepository.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = DataProvider.DatabaseRepository.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return ExecuteDataset(sqlSelect);
        }

        public DataSet GetStlDataSetBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

            //var sqlSelect = DataProvider.DatabaseRepository.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = DataProvider.DatabaseRepository.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return ExecuteDataset(sqlSelect);
        }

        public async Task<List<string>> GetAllFilePathBySiteIdAsync(int siteId)
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

            return DataProvider.DatabaseRepository.GetIntResult(sqlString);
        }

        public async Task<List<int>> GetChannelIdListAsync(Template template)
        {
            if (template.TemplateType != TemplateType.ChannelTemplate &&
                template.TemplateType != TemplateType.ContentTemplate)
            {
                return new List<int>();
            }

            if (template.TemplateType == TemplateType.ChannelTemplate)
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

        public async Task<List<int>> GetChannelIdListByTemplateIdAsync(int siteId, bool isChannelTemplate, int templateId)
        {
            var name = isChannelTemplate ? nameof(Channel.ChannelTemplateId) : nameof(Channel.ContentTemplateId);
            return await _repository.GetAllAsync<int>(Q
                .Select(nameof(Channel.Id))
                .Where(name, templateId)
            );
        }
    }
}
