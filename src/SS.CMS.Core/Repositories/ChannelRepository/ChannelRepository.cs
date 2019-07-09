using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Enumerations;
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
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly Repository<ChannelInfo> _repository;

        public ChannelRepository(IDistributedCache cache, ISettingsManager settingsManager, IUserRepository userRepository, IChannelGroupRepository channelGroupRepository, ISiteRepository siteRepository)
        {
            _cache = cache;
            _userRepository = userRepository;
            _channelGroupRepository = channelGroupRepository;
            _siteRepository = siteRepository;
            _repository = new Repository<ChannelInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(ChannelInfo channelInfo)
        {
            var parentChannelInfo = await GetChannelInfoAsync(channelInfo.ParentId);

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

                var maxTaxis = await GetMaxTaxisByParentPathAsync(channelInfo.ParentsPath);
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

            await _repository.IncrementAsync(Attr.Taxis, Q
                .Where(Attr.Taxis, ">=", channelInfo.Taxis)
                .Where(Attr.SiteId, channelInfo.SiteId)
            );
            channelInfo.Id = await _repository.InsertAsync(channelInfo);

            if (!string.IsNullOrEmpty(channelInfo.ParentsPath))
            {
                await _repository.IncrementAsync(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(channelInfo.ParentsPath))
                );
            }

            await _repository.UpdateAsync(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, channelInfo.ParentId)
            );

            var topId = await _repository.GetAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, channelInfo.ParentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                await _repository.UpdateAsync(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(Attr.Id, topId)
                );
            }

            await RemoveListCacheAsync(channelInfo.SiteId);
            if (parentChannelInfo != null)
            {
                await RemoveEntityCacheAsync(parentChannelInfo.Id);
            }

            // Permissions.ClearAllCache();

            return channelInfo.Id;
        }

        public async Task UpdateAsync(ChannelInfo channelInfo)
        {
            await _repository.UpdateAsync(channelInfo);

            var nodeInfo = await GetChannelInfoAsync(channelInfo.Id);
            if (nodeInfo.ChannelName != channelInfo.ChannelName || nodeInfo.IndexName != channelInfo.IndexName)
            {
                await RemoveListCacheAsync(channelInfo.SiteId);
            }

            await RemoveEntityCacheAsync(channelInfo.Id);
        }

        public async Task UpdateExtendAsync(ChannelInfo channelInfo)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.ExtendValues, channelInfo.ExtendValues)
                .Where(Attr.Id, channelInfo.Id)
            );

            await RemoveEntityCacheAsync(channelInfo.Id);
        }

        public async Task DeleteAsync(int siteId, int channelId)
        {
            var channelInfo = await GetChannelInfoAsync(channelId);
            if (channelInfo == null) return;

            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            // var tableName = GetTableName(_pluginManager, siteInfo, channelInfo);
            var idList = new List<int>();
            if (channelInfo.ChildrenCount > 0)
            {
                idList = await GetChannelIdListAsync(channelInfo, ScopeType.Descendant, string.Empty, string.Empty, string.Empty);
            }
            idList.Add(channelId);

            foreach (var i in idList)
            {
                var cInfo = await GetChannelInfoAsync(i);
                await cInfo.ContentRepository.UpdateTrashContentsByChannelIdAsync(siteId, i);
            }

            var deletedNum = await _repository.DeleteAsync(Q
                .WhereIn(Attr.Id, idList));

            if (channelInfo.ParentId != 0)
            {
                await _repository.DecrementAsync(Attr.Taxis, Q
                        .Where(Attr.SiteId, channelInfo.SiteId)
                        .Where(Attr.Taxis, ">", channelInfo.Taxis), deletedNum
                );
            }

            await UpdateIsLastNodeAsync(channelInfo.ParentId);
            await UpdateSubtractChildrenCountAsync(channelInfo.ParentsPath, deletedNum);

            if (channelInfo.ParentId == 0)
            {
                await _siteRepository.DeleteAsync(channelInfo.Id);
            }

            await RemoveListCacheAsync(channelInfo.SiteId);
            await RemoveEntityCacheAsync(channelInfo.Id);
        }

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        public async Task<ChannelInfo> GetChannelInfoByLastAddDateAsync(int channelId)
        {
            return await _repository.GetAsync(Q
                .Where(Attr.ParentId, channelId)
                .OrderByDesc(Attr.Id));
        }

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        public async Task<ChannelInfo> GetChannelInfoByTaxisAsync(int channelId)
        {
            return await _repository.GetAsync(Q
                .Where(Attr.ParentId, channelId)
                .OrderBy(Attr.Taxis));
        }

        public async Task<int> GetIdByParentIdAndTaxisAsync(int parentId, int taxis, bool isNextChannel)
        {
            int channelId;
            if (isNextChannel)
            {
                channelId = await _repository.GetAsync<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ParentId, parentId)
                    .Where(Attr.Taxis, ">", taxis)
                    .OrderBy(Attr.Taxis));
            }
            else
            {
                channelId = await _repository.GetAsync<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ParentId, parentId)
                    .Where(Attr.Taxis, "<", taxis)
                    .OrderByDesc(Attr.Taxis));
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



        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        public async Task<string> GetOrderStringInSiteAsync(int channelId)
        {
            var retval = "";
            if (channelId != 0)
            {
                var parentId = await GetParentIdAsync(channelId);
                if (parentId != 0)
                {
                    var orderString = await GetOrderStringInSiteAsync(parentId);
                    retval = orderString + "_" + await GetOrderInSiblingAsync(channelId, parentId);
                }
                else
                {
                    retval = "1";
                }
            }
            return retval;
        }

        public async Task<IEnumerable<string>> GetIndexNameListAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(Attr.IndexName)
                .Where(Attr.SiteId, siteId)
                .OrWhere(Attr.Id, siteId)
                .Distinct());
        }

        public async Task<int> GetCountAsync(int channelId)
        {
            return await _repository.CountAsync(Q
                .Where(Attr.ParentId, channelId));
        }

        public async Task<int> GetSequenceAsync(int siteId, int channelId)
        {
            var channelInfo = await GetChannelInfoAsync(channelId);

            var taxis = channelInfo.Taxis;

            return await _repository.CountAsync(Q
                       .Where(Attr.SiteId, siteId)
                       .Where(Attr.ParentId, channelInfo.ParentId)
                       .Where(Attr.Taxis, ">", taxis)
                   ) + 1;
        }

        public async Task<IEnumerable<int>> GetIdListByTotalNumAsync(int siteId, int channelId, TaxisType taxisType, ScopeType scopeType, string groupChannel, string groupChannelNot, bool? isImage, int totalNum)
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

            return await _repository.GetAllAsync<int>(query.Select(Attr.Id));
        }

        public async Task<IEnumerable<KeyValuePair<int, ChannelInfo>>> GetContainerChannelListAsync(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType, ScopeType scopeType, bool isTotal)
        {
            var query = Q.Select(Container.Channel.Columns).Offset(startNum - 1).Limit(totalNum);
            await QueryWhereGroupAsync(query, siteId, group, groupNot);
            QueryWhereImage(query, isImage);
            QueryOrder(query, taxisType);
            await QueryWhereScopeAsync(query, siteId, channelId, isTotal, scopeType);

            var channelInfoList = await _repository.GetAllAsync<ChannelInfo>(query);
            var list = new List<KeyValuePair<int, ChannelInfo>>();
            var i = 0;
            foreach (var channelInfo in channelInfoList)
            {
                list.Add(new KeyValuePair<int, ChannelInfo>(i++, channelInfo));
            }

            return list;
        }

        public async Task<IEnumerable<string>> GetContentModelPluginIdListAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(Attr.ContentModelPluginId)
                .Distinct());
        }

        public async Task<IEnumerable<int>> GetChannelIdListAsync(TemplateInfo templateInfo)
        {
            var list = new List<int>();

            if (templateInfo.Type == TemplateType.ChannelTemplate)
            {
                if (templateInfo.IsDefault)
                {
                    return await _repository.GetAllAsync<int>(Q
                        .Select(Attr.Id)
                        .Where(Attr.SiteId, templateInfo.SiteId)
                        .OrWhere(Attr.ChannelTemplateId, templateInfo.Id)
                        .OrWhere(Attr.ChannelTemplateId, 0)
                        .OrWhereNull(Attr.ChannelTemplateId));
                }

                return await _repository.GetAllAsync<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.SiteId, templateInfo.SiteId)
                    .Where(Attr.ChannelTemplateId, templateInfo.Id));
            }

            if (templateInfo.Type == TemplateType.ContentTemplate)
            {
                if (templateInfo.IsDefault)
                {
                    return await _repository.GetAllAsync<int>(Q
                        .Select(Attr.Id)
                        .Where(Attr.SiteId, templateInfo.SiteId)
                        .OrWhere(Attr.ContentTemplateId, templateInfo.Id)
                        .OrWhere(Attr.ContentTemplateId, 0)
                        .OrWhereNull(Attr.ContentTemplateId));
                }

                return await _repository.GetAllAsync<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.SiteId, templateInfo.SiteId)
                    .Where(Attr.ContentTemplateId, templateInfo.Id));
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

            var sourceSiteId = await GetSiteIdAsync(sourceId);
            var siteInfo = await _siteRepository.GetSiteInfoAsync(sourceSiteId);
            if (siteInfo == null) return "内容转移";

            var nodeNames = await GetChannelNameNavigationAsync(siteInfo.Id, sourceId);
            if (!string.IsNullOrEmpty(nodeNames))
            {
                return siteInfo.SiteName + "：" + nodeNames;
            }
            return siteInfo.SiteName;
        }

        public async Task<ChannelInfo> GetChannelInfoAsync(int channelId)
        {
            return await GetEntityCacheAsync(channelId);
        }

        public async Task<int> GetSiteIdAsync(int channelId)
        {
            var channelInfo = await GetEntityCacheAsync(channelId);
            if (channelInfo == null) return 0;
            return channelInfo.SiteId;
        }

        public async Task<int> GetChannelIdAsync(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retval = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = await GetChannelIdByIndexNameAsync(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = await GetChannelIdByParentIdAndChannelNameAsync(siteId, retval, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = await GetChannelIdByParentIdAndChannelNameAsync(siteId, siteId, channelName, true);
                }
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }

            return retval;
        }

        public async Task<int> GetChannelIdByIndexNameAsync(int siteId, string indexName)
        {
            if (string.IsNullOrEmpty(indexName)) return 0;

            var cacheInfoList = await GetListCacheAsync(siteId);
            return cacheInfoList.Where(x => x.IndexName == indexName).Select(x => x.Id).FirstOrDefault();
        }

        public async Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var cacheInfoList = await GetListCacheAsync(siteId);

            CacheInfo cacheInfo;

            if (recursive)
            {
                if (siteId == parentId)
                {
                    cacheInfo = cacheInfoList.FirstOrDefault(x => x.ChannelName == channelName);
                }
                else
                {
                    cacheInfo = cacheInfoList.FirstOrDefault(x => (x.ParentId == parentId || x.ParentsIdList.Contains(parentId)) && x.ChannelName == channelName);
                }
            }
            else
            {
                cacheInfo = cacheInfoList.FirstOrDefault(x => x.ParentId == parentId && x.ChannelName == channelName);
            }

            return cacheInfo?.Id ?? 0;
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId)
        {
            var cacheInfoList = await GetListCacheAsync(siteId);
            return cacheInfoList.Select(channelInfo => channelInfo.Id).ToList();
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId, string channelGroup)
        {
            var channelInfoList = new List<ChannelInfo>();
            var cacheInfoList = await GetListCacheAsync(siteId);
            foreach (var cacheInfo in cacheInfoList)
            {
                var channelInfo = await GetChannelInfoAsync(cacheInfo.Id);
                if (channelInfo == null) continue;

                if (string.IsNullOrEmpty(channelInfo.GroupNameCollection)) continue;

                if (StringUtils.Contains(channelInfo.GroupNameCollection, channelGroup))
                {
                    channelInfoList.Add(channelInfo);
                }
            }
            return channelInfoList.OrderBy(c => c.Taxis).Select(channelInfo => channelInfo.Id).ToList();
        }

        public async Task<List<int>> GetChannelIdListAsync(ChannelInfo channelInfo, ScopeType scopeType)
        {
            return await GetChannelIdListAsync(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        }

        public async Task<List<int>> GetChannelIdListAsync(ChannelInfo channelInfo, ScopeType scopeType, string group, string groupNot, string contentModelPluginId)
        {
            if (channelInfo == null) return new List<int>();

            var channelInfoList = new List<ChannelInfo>();

            if (channelInfo.ChildrenCount == 0)
            {
                if (scopeType != ScopeType.Children && scopeType != ScopeType.Descendant)
                {
                    channelInfoList.Add(channelInfo);
                }
            }
            else if (scopeType == ScopeType.Self)
            {
                channelInfoList.Add(channelInfo);
            }
            else
            {
                var cacheInfoList = await GetListCacheAsync(channelInfo.SiteId);

                if (scopeType == ScopeType.All)
                {
                    foreach (var cacheInfo in cacheInfoList)
                    {
                        if (cacheInfo.Id == channelInfo.Id || cacheInfo.ParentId == channelInfo.Id || cacheInfo.ParentsIdList.Contains(channelInfo.Id))
                        {
                            var nodeInfo = await GetChannelInfoAsync(cacheInfo.Id);
                            if (nodeInfo != null)
                            {
                                channelInfoList.Add(nodeInfo);
                            }
                        }
                    }
                }
                else if (scopeType == ScopeType.Children)
                {
                    foreach (var cacheInfo in cacheInfoList)
                    {
                        if (cacheInfo.ParentId == channelInfo.Id)
                        {
                            var nodeInfo = await GetChannelInfoAsync(cacheInfo.Id);
                            if (nodeInfo != null)
                            {
                                channelInfoList.Add(nodeInfo);
                            }
                        }
                    }
                }
                else if (scopeType == ScopeType.Descendant)
                {
                    foreach (var cacheInfo in cacheInfoList)
                    {
                        if (cacheInfo.ParentId == channelInfo.Id || cacheInfo.ParentsIdList.Contains(channelInfo.Id))
                        {
                            var nodeInfo = await GetChannelInfoAsync(cacheInfo.Id);
                            if (nodeInfo != null)
                            {
                                channelInfoList.Add(nodeInfo);
                            }
                        }
                    }
                }
                else if (scopeType == ScopeType.SelfAndChildren)
                {
                    foreach (var cacheInfo in cacheInfoList)
                    {
                        if (cacheInfo.Id == channelInfo.Id || cacheInfo.ParentId == channelInfo.Id)
                        {
                            var nodeInfo = await GetChannelInfoAsync(cacheInfo.Id);
                            if (nodeInfo != null)
                            {
                                channelInfoList.Add(nodeInfo);
                            }
                        }
                    }
                }
            }

            var filteredChannelInfoList = new List<ChannelInfo>();
            foreach (var nodeInfo in channelInfoList)
            {
                if (!string.IsNullOrEmpty(group))
                {
                    if (!StringUtils.In(nodeInfo.GroupNameCollection, group))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(groupNot))
                {
                    if (StringUtils.In(nodeInfo.GroupNameCollection, groupNot))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(contentModelPluginId))
                {
                    if (!StringUtils.EqualsIgnoreCase(nodeInfo.ContentModelPluginId, contentModelPluginId))
                    {
                        continue;
                    }
                }
                filteredChannelInfoList.Add(nodeInfo);
            }

            return filteredChannelInfoList.OrderBy(c => c.Taxis).Select(channelInfoInList => channelInfoInList.Id).ToList();
        }

        public async Task<bool> IsExistsAsync(int channelId)
        {
            var channelInfo = await GetChannelInfoAsync(channelId);
            return channelInfo != null;
        }

        public async Task<int> GetChannelIdByParentsCountAsync(int siteId, int channelId, int parentsCount)
        {
            if (parentsCount == 0) return siteId;
            if (channelId == 0 || channelId == siteId) return siteId;

            var nodeInfo = await GetChannelInfoAsync(channelId);
            if (nodeInfo != null)
            {
                return nodeInfo.ParentsCount == parentsCount ? nodeInfo.Id : await GetChannelIdByParentsCountAsync(siteId, nodeInfo.ParentId, parentsCount);
            }
            return siteId;
        }

        public async Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, int channelId)
        {
            return await GetTableNameAsync(pluginManager, siteInfo, await GetChannelInfoAsync(channelId));
        }

        public async Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            return channelInfo != null ? await GetTableNameAsync(pluginManager, siteInfo, channelInfo.ContentModelPluginId) : string.Empty;
        }

        public async Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, string pluginId)
        {
            var tableName = siteInfo.TableName;

            if (string.IsNullOrEmpty(pluginId)) return tableName;

            var contentTable = await pluginManager.GetContentTableNameAsync(pluginId);
            if (!string.IsNullOrEmpty(contentTable))
            {
                tableName = contentTable;
            }

            return tableName;
        }

        public async Task<bool> IsContentModelPluginAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            if (string.IsNullOrEmpty(channelInfo.ContentModelPluginId)) return false;

            var contentTable = await pluginManager.GetContentTableNameAsync(channelInfo.ContentModelPluginId);
            return !string.IsNullOrEmpty(contentTable);
        }

        public async Task<string> GetParentsPathAsync(int channelId)
        {
            var retval = string.Empty;
            var channelInfo = await GetChannelInfoAsync(channelId);
            if (channelInfo != null)
            {
                retval = channelInfo.ParentsPath;
            }
            return retval;
        }

        public async Task<int> GetTopLevelAsync(int channelId)
        {
            var parentsPath = await GetParentsPathAsync(channelId);
            return string.IsNullOrEmpty(parentsPath) ? 0 : parentsPath.Split(',').Length;
        }

        public async Task<string> GetChannelNameAsync(int channelId)
        {
            var retval = string.Empty;
            var nodeInfo = await GetChannelInfoAsync(channelId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.ChannelName;
            }
            return retval;
        }

        public async Task<string> GetChannelNameNavigationAsync(int siteId, int channelId)
        {
            var nodeNameList = new List<string>();

            if (channelId == 0) channelId = siteId;

            if (channelId == siteId)
            {
                var nodeInfo = await GetChannelInfoAsync(siteId);
                return nodeInfo.ChannelName;
            }
            var parentsPath = await GetParentsPathAsync(channelId);
            var channelIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                channelIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            channelIdList.Add(channelId);
            channelIdList.Remove(siteId);

            foreach (var theChannelId in channelIdList)
            {
                var nodeInfo = await GetChannelInfoAsync(theChannelId);
                if (nodeInfo != null)
                {
                    nodeNameList.Add(nodeInfo.ChannelName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(nodeNameList, " > ");
        }

        public async Task<string> GetContentAttributesOfDisplayAsync(int siteId, int channelId)
        {
            var channelInfo = await GetChannelInfoAsync(channelId);
            if (channelInfo == null) return string.Empty;
            if (siteId != channelId && string.IsNullOrEmpty(channelInfo.ContentAttributesOfDisplay))
            {
                return await GetContentAttributesOfDisplayAsync(siteId, channelInfo.ParentId);
            }
            return channelInfo.ContentAttributesOfDisplay;
        }

        public async Task<bool> IsAncestorOrSelfAsync(int parentId, int childId)
        {
            if (parentId == childId)
            {
                return true;
            }
            var nodeInfo = await GetChannelInfoAsync(childId);
            if (nodeInfo == null)
            {
                return false;
            }
            if (StringUtils.In(nodeInfo.ParentsPath, parentId.ToString()))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsCreatableAsync(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            if (siteInfo == null || channelInfo == null) return false;

            if (!channelInfo.IsChannelCreatable || !string.IsNullOrEmpty(channelInfo.LinkUrl)) return false;

            var isCreatable = false;

            var linkType = ELinkTypeUtils.GetEnumType(channelInfo.LinkType);

            if (linkType == ELinkType.None)
            {
                isCreatable = true;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExists)
            {
                var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                isCreatable = count != 0;
            }
            else if (linkType == ELinkType.LinkToOnlyOneContent)
            {
                var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                isCreatable = count != 1;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
            {
                var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                if (count != 0 && count != 1)
                {
                    isCreatable = true;
                }
            }
            else if (linkType == ELinkType.LinkToFirstContent)
            {
                var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                isCreatable = count < 1;
            }
            else if (linkType == ELinkType.NoLinkIfChannelNotExists)
            {
                isCreatable = channelInfo.ChildrenCount != 0;
            }
            else if (linkType == ELinkType.LinkToLastAddChannel)
            {
                isCreatable = channelInfo.ChildrenCount <= 0;
            }
            else if (linkType == ELinkType.LinkToFirstChannel)
            {
                isCreatable = channelInfo.ChildrenCount <= 0;
            }

            return isCreatable;
        }
    }
}
