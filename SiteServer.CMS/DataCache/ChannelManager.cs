using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;
using TableStyle = SiteServer.Abstractions.TableStyle;

namespace SiteServer.CMS.DataCache
{
    public static class ChannelManager
    {
        private static class ChannelManagerCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(ChannelManager));

            private static void Update(Dictionary<int, Dictionary<int, Channel>> allDict, Dictionary<int, Channel> dic, int siteId)
            {
                lock (LockObject)
                {
                    allDict[siteId] = dic;
                }
            }

            private static Dictionary<int, Dictionary<int, Channel>> GetAllDictionary()
            {
                var allDict = DataCacheManager.Get<Dictionary<int, Dictionary<int, Channel>>>(CacheKey);
                if (allDict != null) return allDict;

                allDict = new Dictionary<int, Dictionary<int, Channel>>();
                DataCacheManager.Insert(CacheKey, allDict);
                return allDict;
            }

            public static void Remove(int siteId)
            {
                var allDict = GetAllDictionary();

                lock (LockObject)
                {
                    allDict.Remove(siteId);
                }
            }

            public static async Task UpdateAsync(int siteId, Channel channel)
            {
                var dict = await GetChannelDictionaryBySiteIdAsync(siteId);

                lock (LockObject)
                {
                    dict[channel.Id] = channel;
                }
            }

            public static async Task<Dictionary<int, Channel>> GetChannelDictionaryBySiteIdAsync(int siteId)
            {
                var allDict = GetAllDictionary();

                allDict.TryGetValue(siteId, out var dict);

                if (dict != null) return dict;

                dict = await DataProvider.ChannelRepository.GetChannelDictionaryBySiteIdAsync(siteId);
                Update(allDict, dict, siteId);
                return dict;
            }
        }

        public static void RemoveCacheBySiteId(int siteId)
        {
            ChannelManagerCache.Remove(siteId);
            StlChannelCache.ClearCache();
        }

        public static async Task UpdateCacheAsync(int siteId, Channel channel)
        {
            await ChannelManagerCache.UpdateAsync(siteId, channel);
            StlChannelCache.ClearCache();
        }

        public static async Task<Channel> GetChannelAsync(int siteId, int channelId)
        {
            Channel channel = null;
            var dict = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(siteId);
            if (channelId == 0) channelId = siteId;
            dict?.TryGetValue(Math.Abs(channelId), out channel);
            return channel;
        }

        public static async Task<List<Cascade<int>>> GetChannelOptionsAsync(int siteId, int parentId = 0)
        {
            var list = new List<Cascade<int>>();

            var dic = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(siteId);

            foreach (var channelInfo in dic.Values)
            {
                if (channelInfo == null) continue;
                if (channelInfo.ParentId == parentId)
                {
                    list.Add(new Cascade<int>
                    {
                        Value = channelInfo.Id,
                        Label = channelInfo.ChannelName,
                        Children = await GetChannelOptionsAsync(siteId, channelInfo.Id)
                    });
                }
            }

            return list;
        }

        public static async Task<IList<Channel>> GetChildrenAsync(int siteId, int parentId)
        {
            var list = new List<Channel>();

            var dic = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(siteId);

            foreach (var channelInfo in dic.Values)
            {
                if (channelInfo == null) continue;
                if (channelInfo.ParentId == parentId)
                {
                    channelInfo.Children = await GetChildrenAsync(siteId, channelInfo.Id);
                    list.Add(channelInfo);
                }
            }

            return list;
        }

        public static async Task<int> GetChannelIdAsync(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retVal = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = await GetChannelIdByIndexNameAsync(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retVal = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = await GetChannelIdByParentIdAndChannelNameAsync(siteId, retVal, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = await GetChannelIdByParentIdAndChannelNameAsync(siteId, siteId, channelName, true);
                }
                if (theChannelId != 0)
                {
                    retVal = theChannelId;
                }
            }

            return retVal;
        }

        public static async Task<int> GetChannelIdByIndexNameAsync(int siteId, string indexName)
        {
            if (string.IsNullOrEmpty(indexName)) return 0;

            var dict = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(siteId);
            var channelInfo = dict.Values.FirstOrDefault(x => x != null && x.IndexName == indexName);
            return channelInfo?.Id ?? 0;
        }

        public static async Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var dict = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(siteId);
            var channelInfoList = dict.Values.OrderBy(x => x.Taxis).ToList();

            Channel channel;

            if (recursive)
            {
                if (siteId == parentId)
                {
                    channel = channelInfoList.FirstOrDefault(x => x.ChannelName == channelName);

                    //sqlString = $"SELECT Id FROM siteserver_Channel WHERE (SiteId = {siteId} AND ChannelName = '{AttackUtils.FilterSql(channelName)}') ORDER BY Taxis";
                }
                else
                {
                    channel = channelInfoList.FirstOrDefault(x => (x.ParentId == parentId || StringUtils.GetIntList(x.ParentsPath).Contains(parentId)) && x.ChannelName == channelName);

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
                channel = channelInfoList.FirstOrDefault(x => x.ParentId == parentId && x.ChannelName == channelName);

                //sqlString = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND ChannelName = '{AttackUtils.FilterSql(channelName)}') ORDER BY Taxis";
            }

            return channel?.Id ?? 0;
        }

        //public static List<string> GetIndexNameList(int siteId)
        //{
        //    var dic = ChannelManagerCache.GetChannelDictionaryBySiteId(siteId);
        //    return dic.Values.Where(x => !string.IsNullOrEmpty(x?.IndexName)).Select(x => x.IndexName).Distinct().ToList();
        //}

        public static async Task<List<Channel>> GetChannelListAsync(int siteId)
        {
            var dic = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(siteId);
            return dic.Values.Where(channelInfo => channelInfo != null).ToList();
        }

        public static async Task<List<int>> GetChannelIdListAsync(int siteId)
        {
            var dic = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(siteId);
            return dic.Values.OrderBy(c => c.Taxis).Select(channelInfo => channelInfo.Id).ToList();
        }

        public static async Task<List<int>> GetChannelIdListAsync(int siteId, string channelGroup)
        {
            var channelInfoList = new List<Channel>();
            var dic = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(siteId);
            foreach (var channelInfo in dic.Values)
            {
                if (channelInfo.GroupNames == null) continue;

                if (StringUtils.ContainsIgnoreCase(channelInfo.GroupNames, channelGroup))
                {
                    channelInfoList.Add(channelInfo);
                }
            }
            return channelInfoList.OrderBy(c => c.Taxis).Select(channelInfo => channelInfo.Id).ToList();
        }

        public static async Task<List<int>> GetChannelIdListAsync(Channel channel, EScopeType scopeType)
        {
            return await GetChannelIdListAsync(channel, scopeType, string.Empty, string.Empty, string.Empty);
        }

        public static async Task<List<int>> GetChannelIdListAsync(Channel channel, EScopeType scopeType, string group, string groupNot, string contentModelPluginId)
        {
            if (channel == null) return new List<int>();

            var dic = await ChannelManagerCache.GetChannelDictionaryBySiteIdAsync(channel.SiteId);
            var channelInfoList = new List<Channel>();

            if (channel.ChildrenCount == 0)
            {
                if (scopeType != EScopeType.Children && scopeType != EScopeType.Descendant)
                {
                    channelInfoList.Add(channel);
                }
            }
            else if (scopeType == EScopeType.Self)
            {
                channelInfoList.Add(channel);
            }
            else if (scopeType == EScopeType.All)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.Id == channel.Id || nodeInfo.ParentId == channel.Id || StringUtils.In(nodeInfo.ParentsPath, channel.Id))
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == EScopeType.Children)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.ParentId == channel.Id)
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == EScopeType.Descendant)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.ParentId == channel.Id || StringUtils.In(nodeInfo.ParentsPath, channel.Id))
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == EScopeType.SelfAndChildren)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.Id == channel.Id || nodeInfo.ParentId == channel.Id)
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }

            var filteredChannelList = new List<Channel>();
            foreach (var nodeInfo in channelInfoList)
            {
                if (!string.IsNullOrEmpty(group))
                {
                    if (!StringUtils.ContainsIgnoreCase(nodeInfo.GroupNames, group))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(groupNot))
                {
                    if (StringUtils.ContainsIgnoreCase(nodeInfo.GroupNames, groupNot))
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
                filteredChannelList.Add(nodeInfo);
            }

            return filteredChannelList.OrderBy(c => c.Taxis).Select(channelInfoInList => channelInfoInList.Id).ToList();
        }

        public static async Task<bool> IsExistsAsync(int siteId, int channelId)
        {
            var nodeInfo = await GetChannelAsync(siteId, channelId);
            return nodeInfo != null;
        }

        public static async Task<bool> IsExistsAsync(int channelId)
        {
            var list = await DataProvider.SiteRepository.GetSiteIdListAsync();
            foreach (var siteId in list)
            {
                var nodeInfo = await GetChannelAsync(siteId, channelId);
                if (nodeInfo != null) return true;
            }

            return false;
        }

        public static async Task<int> GetChannelIdByParentsCountAsync(int siteId, int channelId, int parentsCount)
        {
            if (parentsCount == 0) return siteId;
            if (channelId == 0 || channelId == siteId) return siteId;

            var nodeInfo = await GetChannelAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                return nodeInfo.ParentsCount == parentsCount ? nodeInfo.Id : await GetChannelIdByParentsCountAsync(siteId, nodeInfo.ParentId, parentsCount);
            }
            return siteId;
        }

        public static async Task<string> GetTableNameAsync(Site site, int channelId)
        {
            return await GetTableNameAsync(site, await GetChannelAsync(site.Id, channelId));
        }

        public static async Task<string> GetTableNameAsync(Site site, Channel channel)
        {
            return channel != null ? await GetTableNameAsync(site, channel.ContentModelPluginId) : string.Empty;
        }

        public static async Task<string> GetTableNameAsync(Site site, string pluginId)
        {
            var tableName = site.TableName;

            if (string.IsNullOrEmpty(pluginId)) return tableName;

            var contentTable = await PluginContentTableManager.GetTableNameAsync(pluginId);
            if (!string.IsNullOrEmpty(contentTable))
            {
                tableName = contentTable;
            }

            return tableName;
        }

        //public static ETableStyle GetTableStyle(Site site, int channelId)
        //{
        //    return GetTableStyle(site, GetChannel(site.Id, channelId));
        //}

        //public static ETableStyle GetTableStyle(Site site, NodeInfo node)
        //{
        //    var tableStyle = ETableStyle.BackgroundContent;

        //    if (string.IsNullOrEmpty(node.ContentModelPluginId)) return tableStyle;

        //    var contentTable = PluginCache.GetEnabledPluginMetadata<IContentModel>(node.ContentModelPluginId);
        //    if (contentTable != null)
        //    {
        //        tableStyle = ETableStyle.Custom;
        //    }

        //    return tableStyle;
        //}

        public static async Task<bool> IsContentModelPluginAsync(Site site, Channel node)
        {
            if (string.IsNullOrEmpty(node.ContentModelPluginId)) return false;

            var contentTable = await PluginContentTableManager.GetTableNameAsync(node.ContentModelPluginId);
            return !string.IsNullOrEmpty(contentTable);
        }

        public static async Task<string> GetNodeTreeLastImageHtmlAsync(Site site, Channel node)
        {
            var imageHtml = string.Empty;
            if (!string.IsNullOrEmpty(node.ContentModelPluginId) || node.ContentRelatedPluginIds.Any())
            {
                var list = await PluginContentManager.GetContentPluginsAsync(node, true);
                if (list != null && list.Count > 0)
                {
                    imageHtml += @"<i class=""ion-cube"" style=""font-size: 15px;vertical-align: baseline;""></i>&nbsp;";
                }
            }
            return imageHtml;
        }

        public static async Task<DateTime> GetAddDateAsync(int siteId, int channelId)
        {
            var retVal = DateTime.MinValue;
            var nodeInfo = await GetChannelAsync(siteId, channelId);
            if (nodeInfo != null && nodeInfo.AddDate.HasValue)
            {
                retVal = nodeInfo.AddDate.Value;
            }
            return retVal;
        }

        public static async Task<int> GetParentIdAsync(int siteId, int channelId)
        {
            var retVal = 0;
            var nodeInfo = await GetChannelAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                retVal = nodeInfo.ParentId;
            }
            return retVal;
        }

        public static async Task<string> GetParentsPathAsync(int siteId, int channelId)
        {
            var retVal = string.Empty;
            var nodeInfo = await GetChannelAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                retVal = nodeInfo.ParentsPath;
            }
            return retVal;
        }

        public static async Task<int> GetTopLevelAsync(int siteId, int channelId)
        {
            var parentsPath = await GetParentsPathAsync(siteId, channelId);
            return string.IsNullOrEmpty(parentsPath) ? 0 : parentsPath.Split(',').Length;
        }

        public static async Task<string> GetChannelNameAsync(int siteId, int channelId)
        {
            var retVal = string.Empty;
            var nodeInfo = await GetChannelAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                retVal = nodeInfo.ChannelName;
            }
            return retVal;
        }

        public static async Task<string> GetIndexNameAsync(int siteId, int channelId)
        {
            var retVal = string.Empty;
            var nodeInfo = await GetChannelAsync(siteId, channelId);
            if (nodeInfo != null)
            {
                retVal = nodeInfo.IndexName;
            }
            return retVal;
        }

        public static async Task<string> GetChannelNameNavigationAsync(int siteId, int channelId)
        {
            return await GetChannelNameNavigationAsync(siteId, siteId, channelId);
        }

        public static async Task<string> GetChannelNameNavigationAsync(int siteId, int currentChannelId, int channelId)
        {
            var nodeNameList = new List<string>();

            if (channelId == 0) channelId = siteId;

            if (channelId == siteId)
            {
                var nodeInfo = await GetChannelAsync(siteId, siteId);
                return nodeInfo.ChannelName;
            }

            var parentsPath = await GetParentsPathAsync(siteId, channelId);
            var channelIdList = new List<int>();
            var indexOf = -1;
            if (!string.IsNullOrEmpty(parentsPath))
            {
                channelIdList = StringUtils.GetIntList(parentsPath);
                indexOf = channelIdList.IndexOf(currentChannelId);
            }
            channelIdList.Add(channelId);
            //channelIdList.Remove(siteId);

            for (var index = 0; index < channelIdList.Count; index++)
            {
                if (index > indexOf)
                {
                    var nodeInfo = await GetChannelAsync(siteId, channelIdList[index]);
                    if (nodeInfo != null)
                    {
                        nodeNameList.Add(nodeInfo.ChannelName);
                    }
                }
            }

            return TranslateUtils.ObjectCollectionToString(nodeNameList, " > ");
        }

        public static async Task AddListItemsAsync(ListItemCollection listItemCollection, Site site, bool isSeeOwning, bool isShowContentNum, PermissionsImpl permissionsImpl)
        {
            var list = await GetChannelIdListAsync(site.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = await permissionsImpl.IsOwningChannelIdAsync(channelId);
                    if (!enabled)
                    {
                        if (!await permissionsImpl.IsDescendantOwningChannelIdAsync(site.Id, channelId)) continue;
                    }
                }
                var nodeInfo = await GetChannelAsync(site.Id, channelId);

                var listitem = new ListItem(await GetSelectTextAsync(site, nodeInfo, permissionsImpl, isLastNodeArray, isShowContentNum), nodeInfo.Id.ToString());
                if (!enabled)
                {
                    listitem.Attributes.Add("style", "color:gray;");
                }
                listItemCollection.Add(listitem);
            }
        }

        public static async Task AddListItemsAsync(ListItemCollection listItemCollection, Site site, bool isSeeOwning, bool isShowContentNum, string contentModelId, PermissionsImpl permissionsImpl)
        {
            var list = await GetChannelIdListAsync(site.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = await permissionsImpl.IsOwningChannelIdAsync(channelId);
                    if (!enabled)
                    {
                        if (!await permissionsImpl.IsDescendantOwningChannelIdAsync(site.Id, channelId)) continue;
                    }
                }
                var nodeInfo = await GetChannelAsync(site.Id, channelId);

                var listitem = new ListItem(await GetSelectTextAsync(site, nodeInfo, permissionsImpl, isLastNodeArray, isShowContentNum), nodeInfo.Id.ToString());
                if (!enabled)
                {
                    listitem.Attributes.Add("style", "color:gray;");
                }
                if (!StringUtils.EqualsIgnoreCase(nodeInfo.ContentModelPluginId, contentModelId))
                {
                    listitem.Attributes.Add("disabled", "disabled");
                }
                listItemCollection.Add(listitem);
            }
        }

        public static async Task AddListItemsForAddContentAsync(ListItemCollection listItemCollection, Site site, bool isSeeOwning, PermissionsImpl permissionsImpl)
        {
            var list = await GetChannelIdListAsync(site.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = await permissionsImpl.IsOwningChannelIdAsync(channelId);
                }

                var nodeInfo = await GetChannelAsync(site.Id, channelId);
                if (enabled)
                {
                    if (nodeInfo.IsContentAddable == false) enabled = false;
                }

                if (!enabled)
                {
                    continue;
                }

                var listitem = new ListItem(await GetSelectTextAsync(site, nodeInfo, permissionsImpl, isLastNodeArray, true), nodeInfo.Id.ToString());
                listItemCollection.Add(listitem);
            }
        }

        /// <summary>
        /// 得到栏目，并且不对（栏目是否可添加内容）进行判断
        /// 提供给触发器页面使用
        /// 使用场景：其他栏目的内容变动之后，设置某个栏目（此栏目不能添加内容）触发生成
        /// </summary>
        public static async Task AddListItemsForCreateChannelAsync(ListItemCollection listItemCollection, Site site, bool isSeeOwning, PermissionsImpl permissionsImpl)
        {
            var list = await GetChannelIdListAsync(site.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = await permissionsImpl.IsOwningChannelIdAsync(channelId);
                }

                var nodeInfo = await GetChannelAsync(site.Id, channelId);

                if (!enabled)
                {
                    continue;
                }

                var listitem = new ListItem(await GetSelectTextAsync(site, nodeInfo, permissionsImpl, isLastNodeArray, true), nodeInfo.Id.ToString());
                listItemCollection.Add(listitem);
            }
        }

        public static async Task<string> GetSelectTextAsync(Site site, Channel channel, PermissionsImpl adminPermissions, bool[] isLastNodeArray, bool isShowContentNum)
        {
            var retVal = string.Empty;
            if (channel.Id == channel.SiteId)
            {
                channel.LastNode = true;
            }
            if (channel.LastNode == false)
            {
                isLastNodeArray[channel.ParentsCount] = false;
            }
            else
            {
                isLastNodeArray[channel.ParentsCount] = true;
            }
            for (var i = 0; i < channel.ParentsCount; i++)
            {
                retVal = string.Concat(retVal, isLastNodeArray[i] ? "　" : "│");
            }
            retVal = string.Concat(retVal, channel.LastNode ? "└" : "├");
            retVal = string.Concat(retVal, channel.ChannelName);

            if (isShowContentNum)
            {
                var adminId = await adminPermissions.GetAdminIdAsync(site.Id, channel.Id);
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel, adminId);
                retVal = string.Concat(retVal, " (", count, ")");
            }

            return retVal;
        }

        public static async Task<string> GetContentAttributesOfDisplayAsync(int siteId, int channelId)
        {
            var nodeInfo = await GetChannelAsync(siteId, channelId);
            if (nodeInfo == null) return string.Empty;
            if (siteId != channelId && string.IsNullOrEmpty(nodeInfo.ContentAttributesOfDisplay))
            {
                return await GetContentAttributesOfDisplayAsync(siteId, nodeInfo.ParentId);
            }
            return nodeInfo.ContentAttributesOfDisplay;
        }

        public static async Task<List<InputListItem>> GetContentsColumnsAsync(Site site, Channel channel, bool includeAll)
        {
            var items = new List<InputListItem>();

            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channel.ContentAttributesOfDisplay);
            var pluginIds = PluginContentManager.GetContentPluginIds(channel);
            var pluginColumns = await PluginContentManager.GetContentColumnsAsync(pluginIds);

            var styleList = ContentUtility.GetAllTableStyleList(await TableStyleManager.GetContentStyleListAsync(site, channel));

            styleList.Insert(0, new TableStyle
            {
                AttributeName = ContentAttribute.Sequence,
                DisplayName = "序号"
            });

            foreach (var style in styleList)
            {
                if (style.Type == InputType.TextEditor) continue;

                var listitem = new InputListItem
                {
                    Text = style.DisplayName,
                    Value = style.AttributeName
                };
                if (style.AttributeName == ContentAttribute.Title)
                {
                    listitem.Selected = true;
                }
                else
                {
                    if (attributesOfDisplay.Contains(style.AttributeName))
                    {
                        listitem.Selected = true;
                    }
                }

                if (includeAll || listitem.Selected)
                {
                    items.Add(listitem);
                }
            }

            if (pluginColumns != null)
            {
                foreach (var pluginId in pluginColumns.Keys)
                {
                    var contentColumns = pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        var listitem = new InputListItem
                        {
                            Text = $"{columnName}({pluginId})",
                            Value = attributeName
                        };

                        if (attributesOfDisplay.Contains(attributeName))
                        {
                            listitem.Selected = true;
                        }

                        if (includeAll || listitem.Selected)
                        {
                            items.Add(listitem);
                        }
                    }
                }
            }

            return items;
        }

        public static async Task<List<InputStyle>> GetInputStylesAsync(Site site, Channel channel)
        {
            var items = new List<InputStyle>();

            var styleList = ContentUtility.GetAllTableStyleList(await TableStyleManager.GetContentStyleListAsync(site, channel));

            foreach (var style in styleList)
            {
                var listitem = new InputStyle
                {
                    DisplayName = style.DisplayName,
                    AttributeName = style.AttributeName
                };
                items.Add(listitem);
            }

            return items;
        }

        public static async Task<bool> IsAncestorOrSelfAsync(int siteId, int parentId, int childId)
        {
            if (parentId == childId)
            {
                return true;
            }
            var nodeInfo = await GetChannelAsync(siteId, childId);
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

        public static async Task<List<KeyValuePair<int, string>>> GetChannelsAsync(int siteId, PermissionsImpl permissionsImpl, params string[] channelPermissions)
        {
            var options = new List<KeyValuePair<int, string>>();

            var list = await GetChannelIdListAsync(siteId);
            foreach (var channelId in list)
            {
                var enabled = await permissionsImpl.HasChannelPermissionsAsync(siteId, channelId, channelPermissions);

                var channelInfo = await GetChannelAsync(siteId, channelId);
                if (enabled && channelPermissions.Contains(Constants.ChannelPermissions.ContentAdd))
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

        public static async Task<bool> IsCreatableAsync(Site site, Channel channel)
        {
            if (site == null || channel == null) return false;

            if (!channel.IsChannelCreatable || !string.IsNullOrEmpty(channel.LinkUrl)) return false;

            var isCreatable = false;

            var linkType = ELinkTypeUtils.GetEnumType(channel.LinkType);

            if (linkType == ELinkType.None)
            {
                isCreatable = true;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExists)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                isCreatable = count != 0;
            }
            else if (linkType == ELinkType.LinkToOnlyOneContent)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                isCreatable = count != 1;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                if (count != 0 && count != 1)
                {
                    isCreatable = true;
                }
            }
            else if (linkType == ELinkType.LinkToFirstContent)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                isCreatable = count < 1;
            }
            else if (linkType == ELinkType.NoLinkIfChannelNotExists)
            {
                isCreatable = channel.ChildrenCount != 0;
            }
            else if (linkType == ELinkType.LinkToLastAddChannel)
            {
                isCreatable = channel.ChildrenCount <= 0;
            }
            else if (linkType == ELinkType.LinkToFirstChannel)
            {
                isCreatable = channel.ChildrenCount <= 0;
            }

            return isCreatable;
        }
    }

}