using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ChannelAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelRepository : IChannelRepository
    {
        private readonly ICacheManager _cacheManager;
        private readonly Repository<ChannelInfo> _repository;
        private readonly IUserRepository _userRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly ISiteRepository _siteRepository;

        public ChannelRepository(ISettingsManager settingsManager, ICacheManager cacheManager, IUserRepository userRepository, IChannelGroupRepository channelGroupRepository, ISiteRepository siteRepository)
        {
            _cacheManager = cacheManager;
            _repository = new Repository<ChannelInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _userRepository = userRepository;
            _channelGroupRepository = channelGroupRepository;
            _siteRepository = siteRepository;
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private void InsertChannelInfo(ChannelInfo parentChannelInfo, ChannelInfo channelInfo)
        {
            if (parentChannelInfo != null)
            {
                channelInfo.SiteId = parentChannelInfo.SiteId;
                if (parentChannelInfo.ParentsPath.Length == 0)
                {
                    channelInfo.ParentsPath = parentChannelInfo.Id.ToString();
                }
                else
                {
                    channelInfo.ParentsPath = parentChannelInfo.ParentsPath + "," + parentChannelInfo.Id;
                }
                channelInfo.ParentsCount = parentChannelInfo.ParentsCount + 1;

                var maxTaxis = GetMaxTaxisByParentPath(channelInfo.ParentsPath);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentChannelInfo.Taxis;
                }
                channelInfo.Taxis = maxTaxis + 1;
            }
            else
            {
                channelInfo.Taxis = 1;
            }

            if (channelInfo.SiteId != 0)
            {
                _repository.Increment(Attr.Taxis, Q
                    .Where(Attr.Taxis, ">=", channelInfo.Taxis)
                    .Where(Attr.SiteId, channelInfo.SiteId)
                );
            }
            channelInfo.Id = _repository.Insert(channelInfo);

            if (!string.IsNullOrEmpty(channelInfo.ParentsPath))
            {
                _repository.Increment(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(channelInfo.ParentsPath))
                );
            }

            _repository.Update(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, channelInfo.ParentId)
            );

            var topId = _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, channelInfo.ParentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                _repository.Update(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(Attr.Id, topId)
                );
            }

            RemoveCacheBySiteId(channelInfo.SiteId);
            // Permissions.ClearAllCache();
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (string.IsNullOrEmpty(parentsPath)) return;

            _repository.Decrement(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(parentsPath)),
                subtractNum);
        }

        /// <summary>
        /// 更新发布系统下的所有节点的排序号
        /// </summary>
        /// <param name="siteId"></param>
        // private void UpdateWholeTaxisBySiteId(int siteId)
        // {
        //     if (siteId <= 0) return;
        //     var idList = new List<int>
        //     {
        //         siteId
        //     };
        //     var level = 0;
        //     string selectLevelCmd =
        //         $"SELECT MAX(ParentsCount) FROM siteserver_Channel WHERE (Id = {siteId}) OR (SiteId = {siteId})";
        //     using (var rdr = ExecuteReader(selectLevelCmd))
        //     {
        //         while (rdr.Read())
        //         {
        //             var parentsCount = GetInt(rdr, 0);
        //             level = parentsCount;
        //         }
        //         rdr.Close();
        //     }

        //     for (var i = 0; i < level; i++)
        //     {
        //         var list = new List<int>(idList);
        //         foreach (var savedId in list)
        //         {
        //             var lastChildIdOfSavedId = savedId;
        //             var sqlString =
        //                 $"SELECT Id, ChannelName FROM siteserver_Channel WHERE ParentId = {savedId} ORDER BY Taxis, IsLastNode";
        //             using (var rdr = ExecuteReader(sqlString))
        //             {
        //                 while (rdr.Read())
        //                 {
        //                     var channelId = GetInt(rdr, 0);
        //                     if (!idList.Contains(channelId))
        //                     {
        //                         var index = idList.IndexOf(lastChildIdOfSavedId);
        //                         idList.Insert(index + 1, channelId);
        //                         lastChildIdOfSavedId = channelId;
        //                     }
        //                 }
        //                 rdr.Close();
        //             }
        //         }
        //     }

        //     for (var i = 1; i <= idList.Count; i++)
        //     {
        //         var channelId = idList[i - 1];
        //         string updateCmd = $"UPDATE siteserver_Channel SET Taxis = {i} WHERE Id = {channelId}";
        //         ExecuteNonQuery(updateCmd);
        //     }
        // }

        private async Task TaxisSubtractAsync(int siteId, int selectedId)
        {
            var channelInfo = await GetChannelInfoAsync(siteId, selectedId);
            if (channelInfo == null || channelInfo.ParentId == 0 || channelInfo.SiteId == 0) return;

            var dataInfo = await _repository.GetAsync(Q
                .Where(Attr.ParentId, channelInfo.ParentId)
                .WhereNot(Attr.Id, channelInfo.Id)
                .Where(Attr.Taxis, "<", channelInfo.Taxis)
                .Where(Attr.SiteId, channelInfo.SiteId)
                .OrderByDesc(Attr.Taxis));
            if (dataInfo == null) return;

            var lowerId = dataInfo.Id;
            var lowerChildrenCount = dataInfo.ChildrenCount;
            var lowerParentsPath = dataInfo.ParentsPath;

            var lowerPath = string.IsNullOrEmpty(lowerParentsPath) ? lowerId.ToString() : string.Concat(lowerParentsPath, ",", lowerId);
            var selectedPath = string.IsNullOrEmpty(channelInfo.ParentsPath) ? channelInfo.Id.ToString() : string.Concat(channelInfo.ParentsPath, ",", channelInfo.Id);

            SetTaxisSubtract(selectedId, selectedPath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerId, lowerPath, channelInfo.ChildrenCount + 1);

            UpdateIsLastNode(channelInfo.ParentId);
        }

        private async Task TaxisAddAsync(int siteId, int selectedId)
        {
            var channelInfo = await GetChannelInfoAsync(siteId, selectedId);
            if (channelInfo == null || channelInfo.ParentId == 0 || channelInfo.SiteId == 0) return;

            var dataInfo = _repository.Get(Q
                .Where(Attr.ParentId, channelInfo.ParentId)
                .WhereNot(Attr.Id, channelInfo.Id)
                .Where(Attr.Taxis, ">", channelInfo.Taxis)
                .Where(Attr.SiteId, channelInfo.SiteId)
                .OrderBy(Attr.Taxis));

            if (dataInfo == null) return;

            var higherId = dataInfo.Id;
            var higherChildrenCount = dataInfo.ChildrenCount;
            var higherParentsPath = dataInfo.ParentsPath;

            var higherPath = string.IsNullOrEmpty(higherParentsPath) ? higherId.ToString() : string.Concat(higherParentsPath, ",", higherId);
            var selectedPath = string.IsNullOrEmpty(channelInfo.ParentsPath) ? channelInfo.Id.ToString() : String.Concat(channelInfo.ParentsPath, ",", channelInfo.Id);

            SetTaxisAdd(selectedId, selectedPath, higherChildrenCount + 1);
            SetTaxisSubtract(higherId, higherPath, channelInfo.ChildrenCount + 1);

            UpdateIsLastNode(channelInfo.ParentId);
        }

        private void SetTaxisAdd(int channelId, string parentsPath, int addNum)
        {
            _repository.Increment(Attr.Taxis, Q
                .Where(Attr.Id, channelId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, $"{parentsPath},"), addNum
                );
        }

        private void SetTaxisSubtract(int channelId, string parentsPath, int subtractNum)
        {
            _repository.Decrement(Attr.Taxis, Q
                .Where(Attr.Id, channelId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, $"{parentsPath},"), subtractNum);
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId <= 0) return;

            _repository.Update(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, parentId)
            );

            var topId = _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                _repository.Update(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(Attr.Id, topId)
                );
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            return _repository.Max(Attr.Taxis, Q
                       .Where(Attr.ParentsPath, parentPath)
                       .OrWhereStarts(Attr.ParentsPath, $"{parentPath},")
                   ) ?? 0;
        }

        private int GetParentId(int channelId)
        {
            return _repository.Get<int>(Q
                .Select(Attr.ParentId)
                .Where(Attr.Id, channelId));
        }

        private int GetIdByParentIdAndOrder(int parentId, int order)
        {
            var idList = _repository.GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis)).ToList();

            for (var i = 0; i < idList.Count; i++)
            {
                if (i + 1 == order)
                {
                    return idList[i];
                }
            }

            return parentId;
        }

        public async Task<int> InsertAsync(int siteId, int parentId, string channelName, string indexName, string contentModelPluginId, string contentRelatedPluginIds, int channelTemplateId, int contentTemplateId)
        {
            if (siteId > 0 && parentId == 0) return 0;

            // var defaultChannelTemplateInfo = _templateRepository.GetDefaultTemplateInfo(siteId, TemplateType.ChannelTemplate);
            // var defaultContentTemplateInfo = _templateRepository.GetDefaultTemplateInfo(siteId, TemplateType.ContentTemplate);

            var channelInfo = new ChannelInfo
            {
                ParentId = parentId,
                SiteId = siteId,
                ChannelName = channelName,
                IndexName = indexName,
                ContentModelPluginId = contentModelPluginId,
                ContentRelatedPluginIds = contentRelatedPluginIds,
                ChannelTemplateId = channelTemplateId,
                ContentTemplateId = contentTemplateId
                // ChannelTemplateId = channelTemplateId > 0 ? channelTemplateId : defaultChannelTemplateInfo.Id,
                // ContentTemplateId = contentTemplateId > 0 ? contentTemplateId : defaultContentTemplateInfo.Id
            };

            var parentChannelInfo = await GetChannelInfoAsync(siteId, parentId);

            InsertChannelInfo(parentChannelInfo, channelInfo);

            return channelInfo.Id;
        }

        public async Task<int> InsertAsync(ChannelInfo channelInfo)
        {
            if (channelInfo.SiteId > 0 && channelInfo.ParentId == 0) return 0;

            var parentChannelInfo = await GetChannelInfoAsync(channelInfo.SiteId, channelInfo.ParentId);

            InsertChannelInfo(parentChannelInfo, channelInfo);

            return channelInfo.Id;
        }

        /// <summary>
        /// 添加后台发布节点
        /// </summary>
        public int InsertSiteInfo(ChannelInfo channelInfo, SiteInfo siteInfo, string administratorName)
        {
            InsertChannelInfo(null, channelInfo);

            siteInfo.Id = channelInfo.Id;

            _siteRepository.Insert(siteInfo);

            var adminInfo = _userRepository.GetUserInfoByUserName(administratorName);
            _userRepository.UpdateSiteId(adminInfo, channelInfo.Id);

            _repository.Update(Q
                .Set(Attr.SiteId, channelInfo.Id)
                .Where(Attr.Id, channelInfo.Id)
            );

            // _templateRepository.CreateDefaultTemplateInfo(channelInfo.Id, administratorName);
            return channelInfo.Id;
        }

        public async Task UpdateAsync(ChannelInfo channelInfo)
        {
            _repository.Update(channelInfo);
            await UpdateCacheAsync(channelInfo.SiteId, channelInfo);
        }

        public async Task UpdateChannelTemplateIdAsync(ChannelInfo channelInfo)
        {
            _repository.Update(Q
                .Set(Attr.ChannelTemplateId, channelInfo.ChannelTemplateId)
                .Where(Attr.Id, channelInfo.Id)
            );

            await UpdateCacheAsync(channelInfo.SiteId, channelInfo);
        }

        public async Task UpdateContentTemplateIdAsync(ChannelInfo channelInfo)
        {
            _repository.Update(Q
                .Set(Attr.ContentTemplateId, channelInfo.ContentTemplateId)
                .Where(Attr.Id, channelInfo.Id)
            );

            await UpdateCacheAsync(channelInfo.SiteId, channelInfo);
        }

        public async Task UpdateExtendAsync(ChannelInfo channelInfo)
        {
            _repository.Update(Q
                .Set(Attr.ExtendValues, channelInfo.ExtendValues)
                .Where(Attr.Id, channelInfo.Id)
            );

            await UpdateCacheAsync(channelInfo.SiteId, channelInfo);
        }

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
            RemoveCacheBySiteId(siteId);
        }

        public async Task AddGroupNameListAsync(int siteId, int channelId, List<string> groupList)
        {
            var channelInfo = await GetChannelInfoAsync(siteId, channelId);
            if (channelInfo == null) return;

            var list = TranslateUtils.StringCollectionToStringList(channelInfo.GroupNameCollection);
            foreach (var groupName in groupList)
            {
                if (!list.Contains(groupName)) list.Add(groupName);
            }

            channelInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);

            _repository.Update(Q
                .Set(Attr.GroupNameCollection, channelInfo.GroupNameCollection)
                .Where(Attr.Id, channelId)
            );

            await UpdateCacheAsync(siteId, channelInfo);
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q.Where(Attr.SiteId, siteId).OrWhere(Attr.Id, siteId));
        }

        public async Task DeleteAsync(int siteId, int channelId)
        {
            var channelInfo = await GetChannelInfoAsync(siteId, channelId);
            if (channelInfo == null) return;

            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            // var tableName = GetTableName(_pluginManager, siteInfo, channelInfo);
            var idList = new List<int>();
            if (channelInfo.ChildrenCount > 0)
            {
                idList = await GetChannelIdListAsync(channelInfo, ScopeType.Descendant, string.Empty, string.Empty, string.Empty);
            }
            idList.Add(channelId);

            foreach (var i in idList)
            {
                var cInfo = await GetChannelInfoAsync(siteId, i);
                await cInfo.ContentRepository.UpdateTrashContentsByChannelIdAsync(siteId, i);
            }

            var deletedNum = await _repository.DeleteAsync(Q
                .WhereIn(Attr.Id, idList));

            if (channelInfo.ParentId != 0)
            {
                _repository.Decrement(Attr.Taxis, Q
                        .Where(Attr.SiteId, channelInfo.SiteId)
                        .Where(Attr.Taxis, ">", channelInfo.Taxis), deletedNum
                );
            }

            UpdateIsLastNode(channelInfo.ParentId);
            UpdateSubtractChildrenCount(channelInfo.ParentsPath, deletedNum);

            if (channelInfo.ParentId == 0)
            {
                await _siteRepository.DeleteAsync(channelInfo.Id);
            }
            else
            {
                RemoveCacheBySiteId(channelInfo.SiteId);
            }
        }

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        public ChannelInfo GetChannelInfoByLastAddDate(int channelId)
        {
            return _repository.Get(Q
                .Where(Attr.ParentId, channelId)
                .OrderByDesc(Attr.Id));
        }

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        public ChannelInfo GetChannelInfoByTaxis(int channelId)
        {
            return _repository.Get(Q
                .Where(Attr.ParentId, channelId)
                .OrderBy(Attr.Taxis));
        }

        public int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            int channelId;
            if (isNextChannel)
            {
                channelId = _repository.Get<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ParentId, parentId)
                    .Where(Attr.Taxis, ">", taxis)
                    .OrderBy(Attr.Taxis));
            }
            else
            {
                channelId = _repository.Get<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ParentId, parentId)
                    .Where(Attr.Taxis, "<", taxis)
                    .OrderByDesc(Attr.Taxis));
            }

            return channelId;
        }

        public int GetId(int siteId, string orderString)
        {
            if (orderString == "1")
                return siteId;

            var channelId = siteId;

            var orderArr = orderString.Split('_');
            for (var index = 1; index < orderArr.Length; index++)
            {
                var order = int.Parse(orderArr[index]);
                channelId = GetIdByParentIdAndOrder(channelId, order);
            }
            return channelId;
        }

        public int GetSiteId(int channelId)
        {
            var siteId = _repository.Get<int>(Q
                .Select(Attr.SiteId)
                .Where(Attr.Id, channelId));

            if (siteId == 0)
            {
                siteId = channelId;
            }

            return siteId;
        }

        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        public string GetOrderStringInSite(int channelId)
        {
            var retval = "";
            if (channelId != 0)
            {
                var parentId = GetParentId(channelId);
                if (parentId != 0)
                {
                    var orderString = GetOrderStringInSite(parentId);
                    retval = orderString + "_" + GetOrderInSibling(channelId, parentId);
                }
                else
                {
                    retval = "1";
                }
            }
            return retval;
        }

        private int GetOrderInSibling(int channelId, int parentId)
        {
            var idList = _repository.GetAll<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis)).ToList();

            return idList.IndexOf(channelId) + 1;
        }

        public IList<string> GetIndexNameList(int siteId)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.IndexName)
                .Where(Attr.SiteId, siteId)
                .OrWhere(Attr.Id, siteId)
                .Distinct()).ToList();
        }

        private void QueryOrder(Query query, TaxisType taxisType)
        {
            if (taxisType == TaxisType.OrderById || taxisType == TaxisType.OrderByChannelId)
            {
                query.OrderBy(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByIdDesc || taxisType == TaxisType.OrderByChannelIdDesc)
            {
                query.OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByAddDate || taxisType == TaxisType.OrderByLastModifiedDate)
            {
                query.OrderBy(Attr.CreatedDate);
            }
            else if (taxisType == TaxisType.OrderByAddDateDesc || taxisType == TaxisType.OrderByLastModifiedDateDesc)
            {
                query.OrderByDesc(Attr.CreatedDate);
            }
            else if (taxisType == TaxisType.OrderByTaxis)
            {
                query.OrderBy(Attr.Taxis);
            }
            else if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                query.OrderByDesc(Attr.Taxis);
            }
            else if (taxisType == TaxisType.OrderByRandom)
            {
                query.OrderByRandom(StringUtils.GetGuid());
            }
            else
            {
                query.OrderBy(Attr.Taxis);
            }
        }

        private async Task QueryWhereScopeAsync(Query query, int siteId, int channelId, bool isTotal, ScopeType scopeType)
        {
            if (isTotal)
            {
                query.Where(Attr.SiteId, siteId);
                return;
            }

            var channelInfo = await GetChannelInfoAsync(siteId, channelId);
            if (channelInfo == null) return;

            if (channelInfo.ChildrenCount == 0)
            {
                if (scopeType != ScopeType.Children && scopeType != ScopeType.Descendant)
                {
                    query.Where(Attr.Id, channelInfo.Id);
                }
            }
            else if (scopeType == ScopeType.Self)
            {
                query.Where(Attr.Id, channelInfo.Id);
            }
            else if (scopeType == ScopeType.All)
            {
                if (channelInfo.Id == siteId)
                {
                    query.Where(Attr.SiteId, siteId);
                }
                else
                {
                    var channelIdList = await GetChannelIdListAsync(channelInfo, scopeType);
                    query.WhereIn(Attr.Id, channelIdList);
                }
            }
            else if (scopeType == ScopeType.Descendant)
            {
                if (channelInfo.Id == siteId)
                {
                    query.Where(Attr.SiteId, siteId).WhereNot(Attr.Id, siteId);
                }
                else
                {
                    var channelIdList = await GetChannelIdListAsync(channelInfo, scopeType);
                    query.WhereIn(Attr.Id, channelIdList);
                }
            }
            else if (scopeType == ScopeType.SelfAndChildren || scopeType == ScopeType.Children)
            {
                var channelIdList = await GetChannelIdListAsync(channelInfo, scopeType);
                query.WhereIn(Attr.Id, channelIdList);
            }
        }

        private void QueryWhereImage(Query query, bool? isImage)
        {
            if (isImage.HasValue)
            {
                if (isImage.Value)
                {
                    query.WhereNotNull(Attr.ImageUrl).WhereNot(Attr.ImageUrl, string.Empty);
                }
                else
                {
                    query.Where(q =>
                    {
                        return q.WhereNull(Attr.ImageUrl).OrWhere(Attr.ImageUrl, string.Empty);
                    });
                }
            }
        }

        private async Task QueryWhereGroupAsync(Query query, int siteId, string group, string groupNot)
        {
            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr.Length > 0)
                {
                    var groupQuery = Q.NewQuery();

                    var channelIdList = new List<int>();

                    foreach (var theGroup in groupArr)
                    {
                        var groupName = theGroup.Trim();

                        if (_channelGroupRepository.IsExists(siteId, groupName))
                        {
                            var groupChannelIdList = await GetChannelIdListAsync(siteId, groupName);
                            foreach (int channelId in groupChannelIdList)
                            {
                                if (!channelIdList.Contains(channelId))
                                {
                                    channelIdList.Add(channelId);
                                }
                            }
                        }
                    }
                    if (channelIdList.Count > 0)
                    {
                        query.WhereIn(Attr.Id, channelIdList);
                    }
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr.Length > 0)
                {
                    var groupNotQuery = Q.NewQuery();

                    var channelIdList = new List<int>();

                    foreach (var theGroup in groupNotArr)
                    {
                        var groupName = theGroup.Trim();

                        if (_channelGroupRepository.IsExists(siteId, groupName))
                        {
                            var groupChannelIdList = await GetChannelIdListAsync(siteId, groupName);
                            foreach (int channelId in groupChannelIdList)
                            {
                                if (!channelIdList.Contains(channelId))
                                {
                                    channelIdList.Add(channelId);
                                }
                            }
                        }
                    }
                    if (channelIdList.Count > 0)
                    {
                        query.WhereNotIn(Attr.Id, channelIdList);
                    }
                }
            }
        }

        // public string GetWhereString(string group, string groupNot, bool isImageExists, bool isImage)
        // {
        //     var whereStringBuilder = new StringBuilder();
        //     if (isImageExists)
        //     {
        //         whereStringBuilder.Append(isImage
        //             ? " AND siteserver_Channel.ImageUrl <> '' "
        //             : " AND siteserver_Channel.ImageUrl = '' ");
        //     }

        //     if (!string.IsNullOrEmpty(group))
        //     {
        //         group = group.Trim().Trim(',');
        //         var groupArr = group.Split(',');
        //         if (groupArr.Length > 0)
        //         {
        //             whereStringBuilder.Append(" AND (");
        //             foreach (var theGroup in groupArr)
        //             {
        //                 var trimGroup = theGroup.Trim();

        //                 whereStringBuilder.Append(
        //                         $" (siteserver_Channel.GroupNameCollection = '{trimGroup}' OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", "," + trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", "," + trimGroup)}) OR ");
        //             }
        //             if (groupArr.Length > 0)
        //             {
        //                 whereStringBuilder.Length = whereStringBuilder.Length - 3;
        //             }
        //             whereStringBuilder.Append(") ");
        //         }
        //     }

        //     if (!string.IsNullOrEmpty(groupNot))
        //     {
        //         groupNot = groupNot.Trim().Trim(',');
        //         var groupNotArr = groupNot.Split(',');
        //         if (groupNotArr.Length > 0)
        //         {
        //             whereStringBuilder.Append(" AND (");
        //             foreach (var theGroupNot in groupNotArr)
        //             {
        //                 var trimGroupNot = AttackUtils.FilterSql(theGroupNot.Trim());
        //                 //whereStringBuilder.Append(
        //                 //    $" (siteserver_Channel.GroupNameCollection <> '{trimGroupNot}' AND CHARINDEX('{trimGroupNot},',siteserver_Channel.GroupNameCollection) = 0 AND CHARINDEX(',{trimGroupNot},',siteserver_Channel.GroupNameCollection) = 0 AND CHARINDEX(',{trimGroupNot}',siteserver_Channel.GroupNameCollection) = 0) AND ");

        //                 whereStringBuilder.Append(
        //                         $" (siteserver_Channel.GroupNameCollection <> '{trimGroupNot}' AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", "," + trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", "," + trimGroupNot)}) AND ");
        //             }
        //             if (groupNotArr.Length > 0)
        //             {
        //                 whereStringBuilder.Length = whereStringBuilder.Length - 4;
        //             }
        //             whereStringBuilder.Append(") ");
        //         }
        //     }

        //     return whereStringBuilder.ToString();
        // }

        public int GetCount(int channelId)
        {
            return _repository.Count(Q
                .Where(Attr.ParentId, channelId));
        }

        public async Task<int> GetSequenceAsync(int siteId, int channelId)
        {
            var channelInfo = await GetChannelInfoAsync(siteId, channelId);

            var taxis = _repository.Get<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.Id, channelId));
            return _repository.Count(Q
                       .Where(Attr.SiteId, siteId)
                       .Where(Attr.ParentId, channelInfo.ParentId)
                       .Where(Attr.Taxis, ">", taxis)
                   ) + 1;
        }

        public async Task<IList<int>> GetIdListByTotalNumAsync(int siteId, int channelId, TaxisType taxisType, ScopeType scopeType, string groupChannel, string groupChannelNot, bool? isImage, int totalNum)
        {
            var query = Q.NewQuery();
            await QueryWhereScopeAsync(query, siteId, channelId, false, scopeType);
            await QueryWhereGroupAsync(query, siteId, groupChannel, groupChannelNot);
            QueryWhereImage(query, isImage);
            QueryOrder(query, taxisType);
            if (totalNum > 0)
            {
                query.Limit(totalNum);
            }

            return _repository.GetAll<int>(query.Select(Attr.Id)).ToList();
        }

        private async Task<Dictionary<int, ChannelInfo>> GetChannelInfoDictionaryBySiteIdToCacheAsync(int siteId)
        {
            var channelInfoList = await _repository.GetAllAsync(Q
                .Where(Attr.SiteId, siteId)
                .Where(q => q
                    .Where(Attr.Id, siteId)
                    .OrWhere(Attr.ParentId, ">", 0))
                .OrderBy(Attr.Taxis));

            return channelInfoList.ToDictionary(channelInfo => channelInfo.Id);
        }

        // public List<Container.Channel> GetContainerChannelList(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
        // {
        //     if (channelIdList == null || channelIdList.Count == 0)
        //     {
        //         return null;
        //     }

        //     var sqlWhereString =
        //         $"WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString}";

        //     var sqlString = DatabaseUtils.GetPageSqlString(TableName, Container.Channel.SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

        //     var list = new List<Container.Channel>();
        //     var itemIndex = 0;

        //     using (var rdr = ExecuteReader(sqlString))
        //     {
        //         while (rdr.Read())
        //         {
        //             var i = 0;
        //             list.Add(new Container.Channel
        //             {
        //                 ItemIndex = itemIndex++,
        //                 Id = GetInt(rdr, i++),
        //                 SiteId = GetInt(rdr, i++),
        //                 AddDate = GetDateTime(rdr, i++),
        //                 Taxis = GetInt(rdr, i++)
        //             });
        //         }
        //         rdr.Close();
        //     }

        //     return list;
        // }

        public async Task<IList<KeyValuePair<int, ChannelInfo>>> GetContainerChannelListAsync(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType, ScopeType scopeType, bool isTotal)
        {
            var query = Q.Select(Container.Channel.Columns).Offset(startNum - 1).Limit(totalNum);
            await QueryWhereGroupAsync(query, siteId, group, groupNot);
            QueryWhereImage(query, isImage);
            QueryOrder(query, taxisType);
            await QueryWhereScopeAsync(query, siteId, channelId, isTotal, scopeType);

            var channelInfoList = _repository.GetAll<ChannelInfo>(query).ToList();
            var list = new List<KeyValuePair<int, ChannelInfo>>();
            var i = 0;
            foreach (var channelInfo in channelInfoList)
            {
                list.Add(new KeyValuePair<int, ChannelInfo>(i++, channelInfo));
            }

            return list;
        }

        public IList<string> GetContentModelPluginIdList()
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.ContentModelPluginId)
                .Distinct()).ToList();
        }

        public IList<string> GetAllFilePathBySiteId(int siteId)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.FilePath)
                .Where(Attr.SiteId, siteId)
                .WhereNotNull(Attr.FilePath)
                .WhereNot(Attr.FilePath, string.Empty)).ToList();
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
                if (isDefault)
                {
                    return _repository.Count(Q
                        .Where(Attr.SiteId, siteId)
                        .Where(q => q
                            .Where(Attr.ChannelTemplateId, templateId)
                            .OrWhere(Attr.ChannelTemplateId, 0)
                            .OrWhereNull(Attr.ChannelTemplateId)
                        ));
                }

                return _repository.Count(Q
                    .Where(Attr.SiteId, siteId)
                    .Where(Attr.ChannelTemplateId, templateId));
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                if (isDefault)
                {
                    return _repository.Count(Q
                        .Where(Attr.SiteId, siteId)
                        .Where(q => q.Where(Attr.ContentTemplateId, templateId)
                            .OrWhere(Attr.ContentTemplateId, 0)
                            .OrWhereNull(Attr.ContentTemplateId))
                        );
                }

                return _repository.Count(Q
                    .Where(Attr.SiteId, siteId)
                    .Where(Attr.ContentTemplateId, templateId));
            }

            return 0;
        }

        public IList<int> GetChannelIdList(TemplateInfo templateInfo)
        {
            var list = new List<int>();

            if (templateInfo.Type == TemplateType.ChannelTemplate)
            {
                if (templateInfo.IsDefault)
                {
                    return _repository.GetAll<int>(Q
                        .Select(Attr.Id)
                        .Where(Attr.SiteId, templateInfo.SiteId)
                        .OrWhere(Attr.ChannelTemplateId, templateInfo.Id)
                        .OrWhere(Attr.ChannelTemplateId, 0)
                        .OrWhereNull(Attr.ChannelTemplateId)).ToList();
                }

                return _repository.GetAll<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.SiteId, templateInfo.SiteId)
                    .Where(Attr.ChannelTemplateId, templateInfo.Id)).ToList();
            }

            if (templateInfo.Type == TemplateType.ContentTemplate)
            {
                if (templateInfo.IsDefault)
                {
                    return _repository.GetAll<int>(Q
                        .Select(Attr.Id)
                        .Where(Attr.SiteId, templateInfo.SiteId)
                        .OrWhere(Attr.ContentTemplateId, templateInfo.Id)
                        .OrWhere(Attr.ContentTemplateId, 0)
                        .OrWhereNull(Attr.ContentTemplateId)).ToList();
                }

                return _repository.GetAll<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.SiteId, templateInfo.SiteId)
                    .Where(Attr.ContentTemplateId, templateInfo.Id)).ToList();
            }

            return list;
        }

        public async Task<string> GetSourceNameAsync(int sourceId)
        {
            if (sourceId == SourceManager.Default)
            {
                return "后台录入";
            }
            if (sourceId == SourceManager.User)
            {
                return "用户投稿";
            }
            if (sourceId == SourceManager.Preview)
            {
                return "预览插入";
            }
            if (sourceId <= 0) return string.Empty;

            var sourceSiteId = GetSiteId(sourceId);
            var siteInfo = _siteRepository.GetSiteInfo(sourceSiteId);
            if (siteInfo == null) return "内容转移";

            var nodeNames = await GetChannelNameNavigationAsync(siteInfo.Id, sourceId);
            if (!string.IsNullOrEmpty(nodeNames))
            {
                return siteInfo.SiteName + "：" + nodeNames;
            }
            return siteInfo.SiteName;
        }
    }
}
