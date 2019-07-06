using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;
using SS.CMS.Utils;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Data;
using Attr = SS.CMS.Core.Models.Attributes.ChannelAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        public class CacheInfo
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public List<int> ParentsIdList { get; set; }
            public string ChannelName { get; set; }
            public string IndexName { get; set; }
        }

        private string GetCacheKeyBySiteId(int siteId)
        {
            return _cache.GetKey(nameof(ChannelRepository), nameof(GetCacheKeyBySiteId), siteId.ToString());
        }

        private string GetCacheKeyById(int id)
        {
            return _cache.GetKey(nameof(ChannelRepository), nameof(GetCacheKeyById), id.ToString());
        }

        private async Task RemoveCacheBySiteIdAsync(int siteId)
        {
            var cacheKey = GetCacheKeyBySiteId(siteId);
            await _cache.RemoveAsync(cacheKey);
        }

        private async Task RemoveCacheByIdAsync(int id)
        {
            var cacheKey = GetCacheKeyById(id);
            await _cache.RemoveAsync(cacheKey);
        }

        private async Task<List<CacheInfo>> GetCacheInfoListAsync(int siteId)
        {
            var cacheKey = GetCacheKeyBySiteId(siteId);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                var channelInfoList = await _repository.GetAllAsync(Q
                .Select(Attr.Id, Attr.ParentId, Attr.ParentsPath, Attr.ChannelName, Attr.IndexName)
                .Where(Attr.SiteId, siteId)
                .OrderBy(Attr.Taxis));

                var cacheInfoList = new List<CacheInfo>();
                foreach (var channelInfo in channelInfoList)
                {
                    cacheInfoList.Add(new CacheInfo
                    {
                        Id = channelInfo.Id,
                        ChannelName = channelInfo.ChannelName,
                        IndexName = channelInfo.IndexName,
                        ParentId = channelInfo.ParentId,
                        ParentsIdList = TranslateUtils.StringCollectionToIntList(channelInfo.ParentsPath)
                    });
                }

                return cacheInfoList;
            });
        }

        public async Task<ChannelInfo> GetChannelInfoAsync(int channelId)
        {
            var cacheKey = GetCacheKeyById(channelId);
            return await _cache.GetOrCreateAsync(cacheKey, async options =>
            {
                return await _repository.GetAsync<ChannelInfo>(Q
                .Where(Attr.Id, channelId));
            });
        }

        public async Task<int> GetSiteIdAsync(int channelId)
        {
            var cacheKey = _cache.GetKey(nameof(ChannelRepository), nameof(GetSiteIdAsync), channelId.ToString());
            return await _cache.GetOrCreateIntAsync(cacheKey, async options =>
            {
                var siteId = await _repository.GetAsync<int>(Q
                                .Select(Attr.SiteId)
                                .Where(Attr.Id, channelId));

                if (siteId == 0)
                {
                    siteId = channelId;
                }

                return siteId;
            });
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

            var cacheInfoList = await GetCacheInfoListAsync(siteId);
            return cacheInfoList.Where(x => x.IndexName == indexName).Select(x => x.Id).FirstOrDefault();
        }

        public async Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var cacheInfoList = await GetCacheInfoListAsync(siteId);

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
            var cacheInfoList = await GetCacheInfoListAsync(siteId);
            return cacheInfoList.Select(channelInfo => channelInfo.Id).ToList();
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId, string channelGroup)
        {
            var channelInfoList = new List<ChannelInfo>();
            var cacheInfoList = await GetCacheInfoListAsync(siteId);
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
                var cacheInfoList = await GetCacheInfoListAsync(channelInfo.SiteId);

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