using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Enums;
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
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        private readonly Repository<Channel> _repository;

        public ChannelRepository(IDistributedCache cache, ISettingsManager settingsManager, IDatabaseRepository databaseRepository, IContentCheckRepository contentCheckRepository, IUserRepository userRepository, ISiteRepository siteRepository, IChannelGroupRepository channelGroupRepository, ITagRepository tagRepository, IErrorLogRepository errorLogRepository)
        {
            _cache = cache;
            _settingsManager = settingsManager;
            _databaseRepository = databaseRepository;
            _contentCheckRepository = contentCheckRepository;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
            _channelGroupRepository = channelGroupRepository;
            _tagRepository = tagRepository;
            _errorLogRepository = errorLogRepository;

            _repository = new Repository<Channel>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(Channel channel)
        {
            var parentChannel = await GetChannelAsync(channel.ParentId);

            if (parentChannel != null)
            {
                channel.SiteId = parentChannel.SiteId;
                if (string.IsNullOrEmpty(parentChannel.ParentsPath))
                {
                    channel.ParentsPath = parentChannel.Id.ToString();
                }
                else
                {
                    channel.ParentsPath = $"{parentChannel.ParentsPath},{parentChannel.Id}";
                }

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

            await _repository.IncrementAsync(Attr.Taxis, Q
                .Where(Attr.Taxis, ">=", channel.Taxis)
                .Where(Attr.SiteId, channel.SiteId)
            );
            channel.Id = await _repository.InsertAsync(channel);

            await RemoveListCacheAsync(channel.SiteId);
            if (parentChannel != null)
            {
                await RemoveEntityCacheAsync(parentChannel.Id);
            }

            // Permissions.ClearAllCache();

            return channel.Id;
        }

        public async Task UpdateAsync(Channel channel)
        {
            await _repository.UpdateAsync(channel);

            var node = await GetChannelAsync(channel.Id);
            if (node.ChannelName != channel.ChannelName || node.IndexName != channel.IndexName)
            {
                await RemoveListCacheAsync(channel.SiteId);
            }

            await RemoveEntityCacheAsync(channel.Id);
        }

        public async Task UpdateExtendAsync(Channel channel)
        {
            await _repository.UpdateAsync(Q
                .Set(Attr.ExtendValues, channel.ExtendValues)
                .Where(Attr.Id, channel.Id)
            );

            await RemoveEntityCacheAsync(channel.Id);
        }

        public async Task<Channel> DeleteAsync(int siteId, int channelId)
        {
            var channel = await GetChannelAsync(channelId);
            if (channel == null) return null;

            var site = await _siteRepository.GetSiteAsync(siteId);

            var idList = new List<int>();
            idList.AddRange(await GetDescendantIdsAsync(siteId, channelId));
            idList.Add(channelId);

            foreach (var i in idList)
            {
                var contentRepository = await GetContentRepositoryAsync(site, i);
                await contentRepository.UpdateTrashContentsByChannelIdAsync(siteId, i);
            }

            var deletedNum = await _repository.DeleteAsync(Q
                .WhereIn(Attr.Id, idList));

            if (channel.ParentId > 0)
            {
                await _repository.DecrementAsync(Attr.Taxis, Q
                        .Where(Attr.SiteId, channel.SiteId)
                        .Where(Attr.Taxis, ">", channel.Taxis), deletedNum
                );
            }

            await RemoveListCacheAsync(channel.SiteId);

            var parentIds = GetParentIds(channel);
            foreach (var parentId in parentIds)
            {
                await RemoveEntityCacheAsync(parentId);
            }
            await RemoveEntityCacheAsync(channel.Id);

            return channel;
        }

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        public async Task<Channel> GetChannelByLastAddDateAsync(int channelId)
        {
            return await _repository.GetAsync(Q
                .Where(Attr.ParentId, channelId)
                .OrderByDesc(Attr.Id));
        }

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        public async Task<Channel> GetChannelByTaxisAsync(int channelId)
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
            var channel = await GetChannelAsync(channelId);

            var taxis = channel.Taxis;

            return await _repository.CountAsync(Q
                       .Where(Attr.SiteId, siteId)
                       .Where(Attr.ParentId, channel.ParentId)
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

        public async Task<IEnumerable<KeyValuePair<int, Channel>>> GetContainerChannelListAsync(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType, ScopeType scopeType, bool isTotal)
        {
            var query = Q.Select(Container.Channel.Columns).Offset(startNum - 1).Limit(totalNum);
            await QueryWhereGroupAsync(query, siteId, group, groupNot);
            QueryWhereImage(query, isImage);
            QueryOrder(query, taxisType);
            await QueryWhereScopeAsync(query, siteId, channelId, isTotal, scopeType);

            var channelList = await _repository.GetAllAsync<Channel>(query);
            var list = new List<KeyValuePair<int, Channel>>();
            var i = 0;
            foreach (var channel in channelList)
            {
                list.Add(new KeyValuePair<int, Channel>(i++, channel));
            }

            return list;
        }

        public async Task<IEnumerable<string>> GetContentModelPluginIdListAsync()
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(Attr.ContentModelPluginId)
                .Distinct());
        }

        public async Task<IEnumerable<int>> GetIdListAsync(Template template)
        {
            var list = new List<int>();

            if (template.Type == TemplateType.ChannelTemplate)
            {
                if (template.IsDefault)
                {
                    return await _repository.GetAllAsync<int>(Q
                        .Select(Attr.Id)
                        .Where(Attr.SiteId, template.SiteId)
                        .OrWhere(Attr.ChannelTemplateId, template.Id)
                        .OrWhere(Attr.ChannelTemplateId, 0)
                        .OrWhereNull(Attr.ChannelTemplateId));
                }

                return await _repository.GetAllAsync<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.SiteId, template.SiteId)
                    .Where(Attr.ChannelTemplateId, template.Id));
            }

            if (template.Type == TemplateType.ContentTemplate)
            {
                if (template.IsDefault)
                {
                    return await _repository.GetAllAsync<int>(Q
                        .Select(Attr.Id)
                        .Where(Attr.SiteId, template.SiteId)
                        .OrWhere(Attr.ContentTemplateId, template.Id)
                        .OrWhere(Attr.ContentTemplateId, 0)
                        .OrWhereNull(Attr.ContentTemplateId));
                }

                return await _repository.GetAllAsync<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.SiteId, template.SiteId)
                    .Where(Attr.ContentTemplateId, template.Id));
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
            var site = await _siteRepository.GetSiteAsync(sourceSiteId);
            if (site == null) return "内容转移";

            var nodeNames = await GetChannelNameNavigationAsync(site.Id, sourceId);
            if (!string.IsNullOrEmpty(nodeNames))
            {
                return site.SiteName + "：" + nodeNames;
            }
            return site.SiteName;
        }

        public async Task<Channel> GetChannelAsync(int channelId)
        {
            return await GetEntityCacheAsync(channelId);
        }

        public async Task<int> GetSiteIdAsync(int channelId)
        {
            var channel = await GetEntityCacheAsync(channelId);
            if (channel == null) return 0;
            return channel.SiteId;
        }

        public async Task<int> GetIdAsync(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retval = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = await GetIdByIndexNameAsync(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = await GetIdByParentIdAndChannelNameAsync(siteId, retval, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = await GetIdByParentIdAndChannelNameAsync(siteId, siteId, channelName, true);
                }
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }

            return retval;
        }

        public async Task<int> GetIdByIndexNameAsync(int siteId, string indexName)
        {
            if (string.IsNullOrEmpty(indexName)) return 0;

            var cacheList = await GetListCacheAsync(siteId);
            return cacheList.Where(x => x.IndexName == indexName).Select(x => x.Id).FirstOrDefault();
        }

        public async Task<int> GetIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var cacheList = await GetListCacheAsync(siteId);

            Cache cache;

            if (recursive)
            {
                if (siteId == parentId)
                {
                    cache = cacheList.FirstOrDefault(x => x.ChannelName == channelName);
                }
                else
                {
                    cache = cacheList.FirstOrDefault(x => (x.ParentId == parentId || x.ParentsIdList.Contains(parentId)) && x.ChannelName == channelName);
                }
            }
            else
            {
                cache = cacheList.FirstOrDefault(x => x.ParentId == parentId && x.ChannelName == channelName);
            }

            return cache?.Id ?? 0;
        }

        public async Task<List<int>> GetIdListAsync(int siteId)
        {
            var cacheList = await GetListCacheAsync(siteId);
            return cacheList.Select(channel => channel.Id).ToList();
        }

        public async Task<List<int>> GetIdListAsync(int siteId, string channelGroup)
        {
            var channelList = new List<Channel>();
            var cacheList = await GetListCacheAsync(siteId);
            foreach (var cache in cacheList)
            {
                var channel = await GetChannelAsync(cache.Id);
                if (channel == null) continue;

                if (string.IsNullOrEmpty(channel.GroupNameCollection)) continue;

                if (StringUtils.Contains(channel.GroupNameCollection, channelGroup))
                {
                    channelList.Add(channel);
                }
            }
            return channelList.OrderBy(c => c.Taxis).Select(channel => channel.Id).ToList();
        }

        public async Task<List<int>> GetIdListAsync(Channel channel, ScopeType scopeType)
        {
            return await GetIdListAsync(channel, scopeType, string.Empty, string.Empty, string.Empty);
        }

        public async Task<IEnumerable<int>> GetDescendantIdsAsync(int siteId, int parentId)
        {
            var idList = new List<int>();
            var cacheList = await GetListCacheAsync(siteId);

            foreach (var cache in cacheList)
            {
                if (cache.ParentId == parentId || cache.ParentsIdList.Contains(parentId))
                {
                    idList.Add(cache.Id);
                }
            }

            return idList;
        }

        public async Task<IEnumerable<int>> GetChildrenIdsAsync(int siteId, int parentId)
        {
            var idList = new List<int>();
            var cacheList = await GetListCacheAsync(siteId);

            foreach (var cache in cacheList)
            {
                if (cache.ParentId == parentId)
                {
                    idList.Add(cache.Id);
                }
            }

            return idList;
        }

        public async Task<List<int>> GetIdListAsync(Channel channel, ScopeType scopeType, string group, string groupNot, string contentModelPluginId)
        {
            if (channel == null) return new List<int>();

            var channelList = new List<Channel>();

            if (scopeType == ScopeType.Self)
            {
                channelList.Add(channel);
            }
            else
            {
                var cacheList = await GetListCacheAsync(channel.SiteId);

                if (scopeType == ScopeType.All)
                {
                    foreach (var cache in cacheList)
                    {
                        if (cache.Id == channel.Id || cache.ParentId == channel.Id || cache.ParentsIdList.Contains(channel.Id))
                        {
                            var node = await GetChannelAsync(cache.Id);
                            if (node != null)
                            {
                                channelList.Add(node);
                            }
                        }
                    }
                }
                else if (scopeType == ScopeType.Children)
                {
                    foreach (var cache in cacheList)
                    {
                        if (cache.ParentId == channel.Id)
                        {
                            var node = await GetChannelAsync(cache.Id);
                            if (node != null)
                            {
                                channelList.Add(node);
                            }
                        }
                    }
                }
                else if (scopeType == ScopeType.Descendant)
                {
                    foreach (var cache in cacheList)
                    {
                        if (cache.ParentId == channel.Id || cache.ParentsIdList.Contains(channel.Id))
                        {
                            var node = await GetChannelAsync(cache.Id);
                            if (node != null)
                            {
                                channelList.Add(node);
                            }
                        }
                    }
                }
                else if (scopeType == ScopeType.SelfAndChildren)
                {
                    foreach (var cache in cacheList)
                    {
                        if (cache.Id == channel.Id || cache.ParentId == channel.Id)
                        {
                            var node = await GetChannelAsync(cache.Id);
                            if (node != null)
                            {
                                channelList.Add(node);
                            }
                        }
                    }
                }
            }

            var filteredChannelList = new List<Channel>();
            foreach (var node in channelList)
            {
                if (!string.IsNullOrEmpty(group))
                {
                    if (!StringUtils.In(node.GroupNameCollection, group))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(groupNot))
                {
                    if (StringUtils.In(node.GroupNameCollection, groupNot))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(contentModelPluginId))
                {
                    if (!StringUtils.EqualsIgnoreCase(node.ContentModelPluginId, contentModelPluginId))
                    {
                        continue;
                    }
                }
                filteredChannelList.Add(node);
            }

            return filteredChannelList.OrderBy(c => c.Taxis).Select(channelInList => channelInList.Id).ToList();
        }

        public async Task<bool> IsExistsAsync(int channelId)
        {
            var channel = await GetChannelAsync(channelId);
            return channel != null;
        }

        public async Task<string> GetTableNameAsync(Site site, int channelId)
        {
            var channel = await GetChannelAsync(channelId);
            return string.IsNullOrEmpty(channel.TableName) ? site.TableName : channel.TableName;
        }

        public string GetTableName(Site site, Channel channel)
        {
            return string.IsNullOrEmpty(channel.TableName) ? site.TableName : channel.TableName;
        }

        public async Task<IContentRepository> GetContentRepositoryAsync(Site site, int channelId)
        {
            var tableName = await GetTableNameAsync(site, channelId);
            return new ContentRepository(_cache, _settingsManager, _databaseRepository, _contentCheckRepository, _userRepository, _siteRepository, this, _tagRepository, _errorLogRepository, tableName);
        }

        public IContentRepository GetContentRepository(Site site, Channel channel)
        {
            return new ContentRepository(_cache, _settingsManager, _databaseRepository, _contentCheckRepository, _userRepository, _siteRepository, this, _tagRepository, _errorLogRepository, GetTableName(site, channel));
        }

        public async Task<IContentRepository> GetContentRepositoryAsync(int siteId)
        {
            var site = await _siteRepository.GetSiteAsync(siteId);
            return new ContentRepository(_cache, _settingsManager, _databaseRepository, _contentCheckRepository, _userRepository, _siteRepository, this, _tagRepository, _errorLogRepository, site.TableName);
        }

        public IContentRepository GetContentRepository(Site site)
        {
            return new ContentRepository(_cache, _settingsManager, _databaseRepository, _contentCheckRepository, _userRepository, _siteRepository, this, _tagRepository, _errorLogRepository, site.TableName);
        }

        // public async Task<string> GetTableNameAsync(IPluginManager pluginManager, Site site, int channelId)
        // {
        //     return await GetTableNameAsync(pluginManager, site, await GetChannelAsync(channelId));
        // }

        // public async Task<string> GetTableNameAsync(IPluginManager pluginManager, Site site, Channel channel)
        // {
        //     return channel != null ? await GetTableNameAsync(pluginManager, site, channel.ContentModelPluginId) : string.Empty;
        // }

        // public async Task<string> GetTableNameAsync(IPluginManager pluginManager, Site site, string pluginId)
        // {
        //     var tableName = site.TableName;

        //     if (string.IsNullOrEmpty(pluginId)) return tableName;

        //     var contentTable = await pluginManager.GetContentTableNameAsync(pluginId);
        //     if (!string.IsNullOrEmpty(contentTable))
        //     {
        //         tableName = contentTable;
        //     }

        //     return tableName;
        // }

        public async Task<bool> IsContentModelPluginAsync(IPluginManager pluginManager, Site site, Channel channel)
        {
            if (string.IsNullOrEmpty(channel.ContentModelPluginId)) return false;

            var contentTable = await pluginManager.GetContentTableNameAsync(channel.ContentModelPluginId);
            return !string.IsNullOrEmpty(contentTable);
        }

        public async Task<string> GetParentsPathAsync(int channelId)
        {
            var retval = string.Empty;
            var channel = await GetChannelAsync(channelId);
            if (channel != null)
            {
                retval = channel.ParentsPath;
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
            var node = await GetChannelAsync(channelId);
            if (node != null)
            {
                retval = node.ChannelName;
            }
            return retval;
        }

        public async Task<string> GetChannelNameNavigationAsync(int siteId, int channelId)
        {
            var nodeNameList = new List<string>();

            if (channelId == 0) channelId = siteId;

            if (channelId == siteId)
            {
                var node = await GetChannelAsync(siteId);
                return node.ChannelName;
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
                var node = await GetChannelAsync(theChannelId);
                if (node != null)
                {
                    nodeNameList.Add(node.ChannelName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(nodeNameList, " > ");
        }

        public async Task<string> GetContentAttributesOfDisplayAsync(int siteId, int channelId)
        {
            var channel = await GetChannelAsync(channelId);
            if (channel == null) return string.Empty;
            if (siteId != channelId && string.IsNullOrEmpty(channel.ContentAttributesOfDisplay))
            {
                return await GetContentAttributesOfDisplayAsync(siteId, channel.ParentId);
            }
            return channel.ContentAttributesOfDisplay;
        }

        public async Task<bool> IsAncestorOrSelfAsync(int parentId, int childId)
        {
            if (parentId == childId)
            {
                return true;
            }
            var node = await GetChannelAsync(childId);
            if (node == null)
            {
                return false;
            }
            if (StringUtils.In(node.ParentsPath, parentId.ToString()))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsCreatableAsync(Site site, Channel channel)
        {
            if (site == null || channel == null) return false;

            if (!channel.IsChannelCreatable || !string.IsNullOrEmpty(channel.LinkUrl)) return false;

            var isCreatable = false;

            var contentRepository = GetContentRepository(site, channel);

            var linkType = ELinkTypeUtils.GetEnumType(channel.LinkType);

            if (linkType == ELinkType.None)
            {
                isCreatable = true;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExists)
            {
                var count = await contentRepository.GetCountAsync(site, channel, true);
                isCreatable = count != 0;
            }
            else if (linkType == ELinkType.LinkToOnlyOneContent)
            {
                var count = await contentRepository.GetCountAsync(site, channel, true);
                isCreatable = count != 1;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
            {
                var count = await contentRepository.GetCountAsync(site, channel, true);
                if (count != 0 && count != 1)
                {
                    isCreatable = true;
                }
            }
            else if (linkType == ELinkType.LinkToFirstContent)
            {
                var count = await contentRepository.GetCountAsync(site, channel, true);
                isCreatable = count < 1;
            }
            else if (linkType == ELinkType.NoLinkIfChannelNotExists)
            {
                var descendantIds = await GetChildrenIdsAsync(channel.SiteId, channel.Id);
                isCreatable = descendantIds.Count() > 0;
            }
            else if (linkType == ELinkType.LinkToLastAddChannel)
            {
                var descendantIds = await GetChildrenIdsAsync(channel.SiteId, channel.Id);
                isCreatable = descendantIds.Count() == 0;
            }
            else if (linkType == ELinkType.LinkToFirstChannel)
            {
                var descendantIds = await GetChildrenIdsAsync(channel.SiteId, channel.Id);
                isCreatable = descendantIds.Count() == 0;
            }

            return isCreatable;
        }

        public async Task<IList<Channel>> GetChannelListAsync(int siteId, int parentId)
        {
            var list = new List<Channel>();

            var cacheList = await GetListCacheAsync(siteId);
            foreach (var cache in cacheList)
            {
                if (cache.ParentId == parentId)
                {
                    var channel = await GetEntityCacheAsync(cache.Id);
                    if (channel != null)
                    {
                        channel.Children = await GetChannelListAsync(siteId, channel.Id);
                        list.Add(channel);
                    }
                }
            }

            return list;
        }

        public List<int> GetParentIds(Channel channel)
        {
            var list = new List<int>();
            if (!string.IsNullOrWhiteSpace(channel.ParentsPath))
            {
                var array = channel.ParentsPath.Split(',');
                foreach (var s in array)
                {
                    if (int.TryParse(s.Trim(), out var i))
                    {
                        if (i > 0 && !list.Contains(i))
                        {
                            list.Add(i);
                        }
                    }
                }
            }

            return list;
        }

        public int GetParentsCount(Channel channel)
        {
            var list = GetParentIds(channel);
            return list.Count;
        }
    }
}
