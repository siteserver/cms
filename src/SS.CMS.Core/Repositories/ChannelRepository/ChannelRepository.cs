using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
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

        private async Task InsertChannelInfoAsync(ChannelInfo parentChannelInfo, ChannelInfo channelInfo)
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

            await RemoveCacheBySiteIdAsync(channelInfo.SiteId);
            await RemoveCacheByIdAsync(parentChannelInfo.Id);
            // Permissions.ClearAllCache();
        }

        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (string.IsNullOrEmpty(parentsPath)) return;

            _repository.Decrement(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(parentsPath)),
                subtractNum);
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

        public async Task<int> InsertAsync(ChannelInfo channelInfo)
        {
            var parentChannelInfo = await GetChannelInfoAsync(channelInfo.ParentId);

            await InsertChannelInfoAsync(parentChannelInfo, channelInfo);

            return channelInfo.Id;
        }

        public async Task UpdateAsync(ChannelInfo channelInfo)
        {
            await _repository.UpdateAsync(channelInfo);

            var nodeInfo = await GetChannelInfoAsync(channelInfo.Id);
            if (nodeInfo.ChannelName != channelInfo.ChannelName || nodeInfo.IndexName != channelInfo.IndexName)
            {
                await RemoveCacheBySiteIdAsync(channelInfo.SiteId);
            }

            await RemoveCacheByIdAsync(channelInfo.Id);
        }

        public async Task UpdateExtendAsync(ChannelInfo channelInfo)
        {
            _repository.Update(Q
                .Set(Attr.ExtendValues, channelInfo.ExtendValues)
                .Where(Attr.Id, channelInfo.Id)
            );

            await RemoveCacheByIdAsync(channelInfo.Id);
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

            await RemoveCacheBySiteIdAsync(channelInfo.SiteId);
            await RemoveCacheByIdAsync(channelInfo.Id);
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

            var channelInfo = await GetChannelInfoAsync(channelId);
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

                        if (await _channelGroupRepository.IsExistsAsync(siteId, groupName))
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

                        if (await _channelGroupRepository.IsExistsAsync(siteId, groupName))
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

        public int GetCount(int channelId)
        {
            return _repository.Count(Q
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
    }
}
