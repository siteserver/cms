using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        private readonly object LockObject = new object();
        private readonly string CacheKey = StringUtils.GetCacheKey(nameof(ChannelRepository));

        private void CacheUpdate(Dictionary<int, Dictionary<int, ChannelInfo>> allDict, Dictionary<int, ChannelInfo> dic, int siteId)
        {
            lock (LockObject)
            {
                allDict[siteId] = dic;
            }
        }

        private Dictionary<int, Dictionary<int, ChannelInfo>> CacheGetAllDictionary()
        {
            var allDict = _cacheManager.Get<Dictionary<int, Dictionary<int, ChannelInfo>>>(CacheKey);
            if (allDict != null) return allDict;

            allDict = new Dictionary<int, Dictionary<int, ChannelInfo>>();
            _cacheManager.Insert(CacheKey, allDict);
            return allDict;
        }

        private void CacheRemove(int siteId)
        {
            var allDict = CacheGetAllDictionary();

            lock (LockObject)
            {
                allDict.Remove(siteId);
            }
        }

        private async Task CacheUpdateAsync(int siteId, ChannelInfo channelInfo)
        {
            var dict = await GetChannelInfoDictionaryBySiteIdAsync(siteId);

            lock (LockObject)
            {
                dict[channelInfo.Id] = channelInfo;
            }
        }

        public async Task<Dictionary<int, ChannelInfo>> GetChannelInfoDictionaryBySiteIdAsync(int siteId)
        {
            var allDict = CacheGetAllDictionary();

            allDict.TryGetValue(siteId, out var dict);

            if (dict != null) return dict;

            dict = await GetChannelInfoDictionaryBySiteIdToCacheAsync(siteId);
            CacheUpdate(allDict, dict, siteId);
            return dict;
        }

        public void RemoveCacheBySiteId(int siteId)
        {
            CacheRemove(siteId);
            StlClearCache();
        }

        public async Task UpdateCacheAsync(int siteId, ChannelInfo channelInfo)
        {
            await CacheUpdateAsync(siteId, channelInfo);
            StlClearCache();
        }

        public async Task<ChannelInfo> GetChannelInfoAsync(int siteId, int channelId)
        {
            ChannelInfo channelInfo = null;
            var dict = await GetChannelInfoDictionaryBySiteIdAsync(siteId);
            if (channelId == 0) channelId = siteId;
            dict?.TryGetValue(Math.Abs(channelId), out channelInfo);
            return channelInfo;
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

            var dict = await GetChannelInfoDictionaryBySiteIdAsync(siteId);
            var channelInfo = dict.Values.FirstOrDefault(x => x != null && x.IndexName == indexName);
            return channelInfo?.Id ?? 0;
        }

        public async Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var dict = await GetChannelInfoDictionaryBySiteIdAsync(siteId);
            var channelInfoList = dict.Values.OrderBy(x => x.Taxis).ToList();

            ChannelInfo channelInfo;

            if (recursive)
            {
                if (siteId == parentId)
                {
                    channelInfo = channelInfoList.FirstOrDefault(x => x.ChannelName == channelName);

                    //sqlString = $"SELECT Id FROM siteserver_Channel WHERE (SiteId = {siteId} AND ChannelName = '{AttackUtils.FilterSql(channelName)}') ORDER BY Taxis";
                }
                else
                {
                    channelInfo = channelInfoList.FirstOrDefault(x => (x.ParentId == parentId || TranslateUtils.StringCollectionToIntList(x.ParentsPath).Contains(parentId)) && x.ChannelName == channelName);

                    //                    sqlString = $@"SELECT Id
                    //FROM siteserver_Channel 
                    //WHERE ((ParentId = {parentId}) OR
                    //      (ParentsPath = '{parentId}') OR
                    //      (ParentsPath LIKE '{parentId},%') OR
                    //      (ParentsPath LIKE '%,{parentId},%') OR
                    //      (ParentsPath LIKE '%,{parentId}')) AND ChannelName = '{AttackUtils.FilterSql(channelName)}'
                    //ORDER BY Taxis";
                }
            }
            else
            {
                channelInfo = channelInfoList.FirstOrDefault(x => x.ParentId == parentId && x.ChannelName == channelName);

                //sqlString = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND ChannelName = '{AttackUtils.FilterSql(channelName)}') ORDER BY Taxis";
            }

            return channelInfo?.Id ?? 0;
        }

        //public List<string> GetIndexNameList(int siteId)
        //{
        //    var dic = GetChannelInfoDictionaryBySiteId(siteId);
        //    return dic.Values.Where(x => !string.IsNullOrEmpty(x?.IndexName)).Select(x => x.IndexName).Distinct().ToList();
        //}

        public async Task<List<ChannelInfo>> GetChannelInfoListAsync(int siteId)
        {
            var dic = await GetChannelInfoDictionaryBySiteIdAsync(siteId);
            return dic.Values.Where(channelInfo => channelInfo != null).ToList();
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId)
        {
            var dic = await GetChannelInfoDictionaryBySiteIdAsync(siteId);
            return dic.Values.OrderBy(c => c.Taxis).Select(channelInfo => channelInfo.Id).ToList();
        }

        public async Task<Dictionary<int, ChannelInfo>> GetChannelInfoDictionaryAsync(int siteId)
        {
            return await GetChannelInfoDictionaryBySiteIdAsync(siteId);
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId, string channelGroup)
        {
            var channelInfoList = new List<ChannelInfo>();
            var dic = await GetChannelInfoDictionaryBySiteIdAsync(siteId);
            foreach (var channelInfo in dic.Values)
            {
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

            var dic = await GetChannelInfoDictionaryBySiteIdAsync(channelInfo.SiteId);
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
            else if (scopeType == ScopeType.All)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.Id == channelInfo.Id || nodeInfo.ParentId == channelInfo.Id || StringUtils.In(nodeInfo.ParentsPath, channelInfo.Id))
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == ScopeType.Children)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.ParentId == channelInfo.Id)
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == ScopeType.Descendant)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.ParentId == channelInfo.Id || StringUtils.In(nodeInfo.ParentsPath, channelInfo.Id))
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == ScopeType.SelfAndChildren)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.Id == channelInfo.Id || nodeInfo.ParentId == channelInfo.Id)
                    {
                        channelInfoList.Add(nodeInfo);
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

        public async Task<bool> IsExistsAsync(int siteId, int channelId)
        {
            var nodeInfo = await GetChannelInfoAsync(siteId, channelId);
            return nodeInfo != null;
        }

        public async Task<bool> IsExistsAsync(ISiteRepository siteRepository, int channelId)
        {
            var list = siteRepository.GetSiteIdList();
            foreach (var siteId in list)
            {
                var nodeInfo = await GetChannelInfoAsync(siteId, channelId);
                if (nodeInfo != null) return true;
            }

            return false;
        }

        public async Task<int> GetChannelIdByParentsCountAsync(int siteId, int channelId, int parentsCount)
        {
            if (parentsCount == 0) return siteId;
            if (channelId == 0 || channelId == siteId) return siteId;

            var nodeInfo = await GetChannelInfoAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                return nodeInfo.ParentsCount == parentsCount ? nodeInfo.Id : await GetChannelIdByParentsCountAsync(siteId, nodeInfo.ParentId, parentsCount);
            }
            return siteId;
        }

        public async Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, int channelId)
        {
            return await GetTableNameAsync(pluginManager, siteInfo, await GetChannelInfoAsync(siteInfo.Id, channelId));
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

        //public ETableStyle GetTableStyle(SiteInfo siteInfo, int channelId)
        //{
        //    return GetTableStyle(siteInfo, GetChannelInfo(siteInfo.Id, channelId));
        //}

        //public ETableStyle GetTableStyle(SiteInfo siteInfo, NodeInfo nodeInfo)
        //{
        //    var tableStyle = ETableStyle.BackgroundContent;

        //    if (string.IsNullOrEmpty(nodeInfo.ContentModelPluginId)) return tableStyle;

        //    var contentTable = PluginCache.GetEnabledPluginMetadata<IContentModel>(nodeInfo.ContentModelPluginId);
        //    if (contentTable != null)
        //    {
        //        tableStyle = ETableStyle.Custom;
        //    }

        //    return tableStyle;
        //}

        public async Task<bool> IsContentModelPluginAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            if (string.IsNullOrEmpty(nodeInfo.ContentModelPluginId)) return false;

            var contentTable = await pluginManager.GetContentTableNameAsync(nodeInfo.ContentModelPluginId);
            return !string.IsNullOrEmpty(contentTable);
        }

        public async Task<string> GetNodeTreeLastImageHtmlAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            var imageHtml = string.Empty;
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelPluginId) || !string.IsNullOrEmpty(nodeInfo.ContentRelatedPluginIds))
            {
                var list = await pluginManager.GetContentPluginsAsync(nodeInfo, true);
                if (list != null && list.Count > 0)
                {
                    imageHtml += @"<i class=""ion-cube"" style=""font-size: 15px;vertical-align: baseline;""></i>&nbsp;";
                }
            }
            return imageHtml;
        }

        public async Task<int> GetParentIdAsync(int siteId, int channelId)
        {
            var retval = 0;
            var nodeInfo = await GetChannelInfoAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.ParentId;
            }
            return retval;
        }

        public async Task<string> GetParentsPathAsync(int siteId, int channelId)
        {
            var retval = string.Empty;
            var nodeInfo = await GetChannelInfoAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.ParentsPath;
            }
            return retval;
        }

        public async Task<int> GetTopLevelAsync(int siteId, int channelId)
        {
            var parentsPath = await GetParentsPathAsync(siteId, channelId);
            return string.IsNullOrEmpty(parentsPath) ? 0 : parentsPath.Split(',').Length;
        }

        public async Task<string> GetChannelNameAsync(int siteId, int channelId)
        {
            var retval = string.Empty;
            var nodeInfo = await GetChannelInfoAsync(siteId, channelId);
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
                var nodeInfo = await GetChannelInfoAsync(siteId, siteId);
                return nodeInfo.ChannelName;
            }
            var parentsPath = await GetParentsPathAsync(siteId, channelId);
            var channelIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                channelIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            channelIdList.Add(channelId);
            channelIdList.Remove(siteId);

            foreach (var theChannelId in channelIdList)
            {
                var nodeInfo = await GetChannelInfoAsync(siteId, theChannelId);
                if (nodeInfo != null)
                {
                    nodeNameList.Add(nodeInfo.ChannelName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(nodeNameList, " > ");
        }

        public async Task<string> GetSelectTextAsync(SiteInfo siteInfo, ChannelInfo channelInfo, IUserManager userManager, bool[] isLastNodeArray, bool isShowContentNum)
        {
            var retval = string.Empty;
            if (channelInfo.Id == channelInfo.SiteId)
            {
                channelInfo.IsLastNode = true;
            }
            if (channelInfo.IsLastNode == false)
            {
                isLastNodeArray[channelInfo.ParentsCount] = false;
            }
            else
            {
                isLastNodeArray[channelInfo.ParentsCount] = true;
            }
            for (var i = 0; i < channelInfo.ParentsCount; i++)
            {
                retval = string.Concat(retval, isLastNodeArray[i] ? "　" : "│");
            }
            retval = string.Concat(retval, channelInfo.IsLastNode ? "└" : "├");
            retval = string.Concat(retval, channelInfo.ChannelName);

            if (isShowContentNum)
            {
                var onlyAdminId = userManager.GetOnlyAdminId(siteInfo.Id, channelInfo.Id);
                var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, onlyAdminId);
                retval = string.Concat(retval, " (", count, ")");
            }

            return retval;
        }

        public async Task<string> GetContentAttributesOfDisplayAsync(int siteId, int channelId)
        {
            var channelInfo = await GetChannelInfoAsync(siteId, channelId);
            if (channelInfo == null) return string.Empty;
            if (siteId != channelId && string.IsNullOrEmpty(channelInfo.ContentAttributesOfDisplay))
            {
                return await GetContentAttributesOfDisplayAsync(siteId, channelInfo.ParentId);
            }
            return channelInfo.ContentAttributesOfDisplay;
        }

        public async Task<bool> IsAncestorOrSelfAsync(int siteId, int parentId, int childId)
        {
            if (parentId == childId)
            {
                return true;
            }
            var nodeInfo = await GetChannelInfoAsync(siteId, childId);
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

        public async Task<List<KeyValuePair<int, string>>> GetChannelsAsync(int siteId, IUserManager userManager, params string[] channelPermissions)
        {
            var options = new List<KeyValuePair<int, string>>();

            var list = await GetChannelIdListAsync(siteId);
            foreach (var channelId in list)
            {
                var enabled = userManager.HasChannelPermissions(siteId, channelId, channelPermissions);

                var channelInfo = await GetChannelInfoAsync(siteId, channelId);
                if (enabled && channelPermissions.Contains(AuthTypes.ChannelPermissions.ContentAdd))
                {
                    if (channelInfo.IsContentAddable == false) enabled = false;
                }

                if (enabled)
                {
                    var tuple = new KeyValuePair<int, string>(channelId,
                        await GetChannelNameNavigationAsync(siteId, channelId));
                    options.Add(tuple);
                }
            }

            return options;
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