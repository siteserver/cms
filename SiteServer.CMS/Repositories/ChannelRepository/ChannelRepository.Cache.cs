using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using TableStyle = SiteServer.Abstractions.TableStyle;

namespace SiteServer.CMS.Repositories
{
    public partial class ChannelRepository
    {
        private async Task<int> GetParentIdAsync(int channelId)
        {
            var channel = await GetAsync(channelId);
            return channel?.ParentId ?? 0;
        }

        public async Task<int> GetSiteIdAsync(int channelId)
        {
            var channel = await GetAsync(channelId);
            return channel?.SiteId ?? 0;
        }

        private async Task<int> GetIdByParentIdAndOrderAsync(int siteId, int parentId, int order)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            var channelIds = summaries
                .Where(x => x.ParentId == parentId)
                .OrderBy(x => x.Taxis)
                .Select(x => x.Id)
                .ToList();

            var channelId = parentId;

            var index = 1;
            foreach (var id in channelIds)
            {
                channelId = id;
                if (index == order)
                    break;
                index++;
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
                var order = TranslateUtils.ToInt(orderArr[index]);
                channelId = await GetIdByParentIdAndOrderAsync(siteId, channelId, order);
            }
            return channelId;
        }

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        public async Task<Channel> GetChannelByLastAddDateAsyncTask(int siteId, int parentId)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            var channelId = summaries
                .Where(x => x.ParentId == parentId)
                .OrderByDescending(x => x.AddDate)
                .Select(x => x.Id)
                .FirstOrDefault();
            return await GetAsync(channelId);
        }

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        public async Task<Channel> GetChannelByTaxisAsync(int siteId, int parentId)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            var channelId = summaries
                .Where(x => x.ParentId == parentId)
                .OrderBy(x => x.Taxis)
                .Select(x => x.Id)
                .FirstOrDefault();
            return await GetAsync(channelId);
        }

        public async Task<int> GetIdByParentIdAndTaxisAsync(int siteId, int parentId, int taxis, bool isNextChannel)
        {
            var summaries = await GetAllSummaryAsync(siteId);

            return isNextChannel
                ? summaries
                    .Where(x => x.ParentId == parentId && x.Taxis > taxis)
                    .OrderBy(x => x.Taxis)
                    .Select(x => x.Id)
                    .FirstOrDefault()
                : summaries
                    .Where(x => x.ParentId == parentId && x.Taxis < taxis)
                    .OrderByDescending(x => x.Taxis)
                    .Select(x => x.Id)
                    .FirstOrDefault();
        }

        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        public async Task<string> GetOrderStringInSiteAsync(int siteId, int channelId)
        {
            var retVal = "";
            if (channelId != 0)
            {
                var parentId = await GetParentIdAsync(channelId);
                if (parentId != 0)
                {
                    var orderString = await GetOrderStringInSiteAsync(siteId, parentId);
                    retVal = orderString + "_" + await GetOrderInSiblingAsync(siteId, channelId, parentId);
                }
                else
                {
                    retVal = "1";
                }
            }
            return retVal;
        }

        private async Task<int> GetOrderInSiblingAsync(int siteId, int channelId, int parentId)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            var channelIds = summaries
                .Where(x => x.ParentId == parentId)
                .OrderBy(x => x.Taxis)
                .Select(x => x.Id).ToList();
            return channelIds.IndexOf(channelId) + 1;
        }

        public async Task<IEnumerable<string>> GetIndexNameListAsync(int siteId)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            return summaries
                .Where(x => !string.IsNullOrEmpty(x.IndexName))
                .Select(x => x.IndexName);
        }

        public async Task<bool> IsIndexNameExistsAsync(int siteId, string indexName)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            return summaries
                .Where(x => !string.IsNullOrEmpty(x.IndexName))
                .Any(x => x.IndexName == indexName);
        }

        public async Task<int> GetCountAsync(int siteId, int parentId)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            return summaries.Count(x => x.ParentId == parentId);
        }

        public async Task<int> GetSequenceAsync(int siteId, int channelId)
        {
            var channel = await GetAsync(channelId);
            var summaries = await GetAllSummaryAsync(siteId);
            return summaries.Count(x => x.ParentId == channel.ParentId && x.Taxis > channel.Taxis) + 1;
        }

        // review

        public async Task<Cascade<int>> GetCascadeAsync(Site site, IChannelSummary summary, Func<IChannelSummary, Task<object>> func)
        {
            object extra = null;
            if (func != null)
            {
                extra = await func(summary);
            }
            var cascade = new Cascade<int>
            {
                Value = summary.Id,
                Label = summary.ChannelName,
                Children = await GetCascadeChildrenAsync(site, summary.Id, func)
            };
            if (extra == null) return cascade;

            var dict = TranslateUtils.ToDictionary(extra);
            foreach (var o in dict)
            {
                cascade[o.Key] = o.Value;
            }

            return cascade;
        }

        public async Task<List<Cascade<int>>> GetCascadeChildrenAsync(Site site, int parentId, Func<IChannelSummary, Task<object>> func)
        {
            var list = new List<Cascade<int>>();

            var summaries = await GetAllSummaryAsync(site.Id);
            foreach (var cache in summaries)
            {
                if (cache == null) continue;
                if (cache.ParentId == parentId)
                {
                    list.Add(await GetCascadeAsync(site, cache, func));
                }
            }

            return list;
        }

        public async Task<Cascade<int>> GetCascadeAsync(Site site, IChannelSummary summary)
        {
            var cascade = new Cascade<int>
            {
                Value = summary.Id,
                Label = summary.ChannelName,
                Children = await GetCascadeChildrenAsync(site, summary.Id)
            };
            cascade["count"] = await DataProvider.ContentRepository.GetCountAsync(site, summary);
            return cascade;
        }

        private async Task<List<Cascade<int>>> GetCascadeChildrenAsync(Site site, int parentId)
        {
            var list = new List<Cascade<int>>();

            var summaries = await GetAllSummaryAsync(site.Id);

            foreach (var summary in summaries)
            {
                if (summary == null) continue;
                if (summary.ParentId == parentId)
                {
                    list.Add(await GetCascadeAsync(site, summary));
                }
            }

            return list;
        }

        public async Task<IList<Channel>> GetChildrenAsync(int siteId, int parentId)
        {
            var list = new List<Channel>();

            var summaries = await GetAllSummaryAsync(siteId);

            foreach (var summary in summaries)
            {
                if (summary == null) continue;
                if (summary.ParentId == parentId)
                {
                    var channel = await GetAsync(summary.Id);
                    channel.Children = await GetChildrenAsync(siteId, channel.Id);
                    list.Add(channel);
                }
            }

            return list;
        }

        public async Task<int> GetChannelIdAsync(int siteId, int channelId, string channelIndex, string channelName)
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

        public async Task<int> GetChannelIdByIndexNameAsync(int siteId, string indexName)
        {
            if (string.IsNullOrEmpty(indexName)) return 0;

            var summaries = await GetAllSummaryAsync(siteId);
            var channelInfo = summaries.FirstOrDefault(x => x != null && x.IndexName == indexName);
            return channelInfo?.Id ?? 0;
        }

        public async Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var summaries = await GetAllSummaryAsync(siteId);

            IChannelSummary channel;

            if (recursive)
            {
                if (siteId == parentId)
                {
                    channel = summaries.FirstOrDefault(x => x.ChannelName == channelName);

                    //sqlString = $"SELECT Id FROM siteserver_Channel WHERE (SiteId = {siteId} AND ChannelName = '{AttackUtils.FilterSql(channelName)}') ORDER BY Taxis";
                }
                else
                {
                    channel = summaries.FirstOrDefault(x => (x.ParentId == parentId || Utilities.GetIntList(x.ParentsPath).Contains(parentId)) && x.ChannelName == channelName);

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
                channel = summaries.FirstOrDefault(x => x.ParentId == parentId && x.ChannelName == channelName);

                //sqlString = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND ChannelName = '{AttackUtils.FilterSql(channelName)}') ORDER BY Taxis";
            }

            return channel?.Id ?? 0;
        }

        public async Task<List<Channel>> GetChannelListAsync(int siteId)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            var list = new List<Channel>();
            foreach (var summary in summaries)
            {
                var channel = await GetAsync(summary.Id);
                list.Add(channel);
            }

            return list;
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            return summaries.OrderBy(c => c.Taxis).Select(x => x.Id).ToList();
        }

        public async Task<List<string>> GetChannelIndexNameListAsync(int siteId)
        {
            var summaries = await GetAllSummaryAsync(siteId);
            return summaries.OrderBy(c => c.Taxis).Where(channelInfo => !string.IsNullOrEmpty(channelInfo.IndexName)).Select(channelInfo => channelInfo.IndexName).ToList();
        }

        private List<int> GetChildIds(List<ChannelSummary> summaries, int parentId)
        {
            return summaries.Where(x => x.ParentId == parentId).Select(x => x.Id).ToList();
        }

        private void GetChildIdsRecursive(List<ChannelSummary> summaries, List<int> list, int parentId)
        {
            var childIds = summaries.Where(x => x.ParentId == parentId).Select(x => x.Id).ToList();
            if (childIds.Count > 0)
            {
                list.AddRange(childIds);
                foreach (var childId in childIds)
                {
                    GetChildIdsRecursive(summaries, list, childId);
                }
            }
        }

        public async Task<List<int>> GetChannelIdsAsync(Channel channel, EScopeType scopeType)
        {
            if (channel == null) return new List<int>();

            if (scopeType == EScopeType.Self)
            {
                return new List<int>{ channel.Id };
            }

            if (scopeType == EScopeType.SelfAndChildren)
            {
                var summaries = await GetAllSummaryAsync(channel.SiteId);
                var list = GetChildIds(summaries, channel.Id);
                list.Add(channel.Id);
                return list;
            }

            if (scopeType == EScopeType.Children)
            {
                var summaries = await GetAllSummaryAsync(channel.SiteId);
                return GetChildIds(summaries, channel.Id);
            }

            if (scopeType == EScopeType.Descendant)
            {
                var summaries = await GetAllSummaryAsync(channel.SiteId);
                var list = new List<int>();
                GetChildIdsRecursive(summaries, list, channel.Id);
                return list;
            }

            if (scopeType == EScopeType.All)
            {
                var summaries = await GetAllSummaryAsync(channel.SiteId);
                var list = new List<int> {channel.Id};
                GetChildIdsRecursive(summaries, list, channel.Id);
                return list;
            }

            return new List<int>();
        }

        public async Task<List<int>> GetChannelIdsAsync(Channel channel, EScopeType scopeType, string group, string groupNot, string contentModelPluginId)
        {
            if (channel == null) return new List<int>();

            var channelIds = await GetChannelIdsAsync(channel, scopeType);

            if (string.IsNullOrEmpty(group) && string.IsNullOrEmpty(groupNot) &&
                string.IsNullOrEmpty(contentModelPluginId))
            {
                return channelIds;
            }

            var filteredChannelIds = new List<int>();
            foreach (var channelId in channelIds)
            {
                var channelInfo = await GetAsync(channelId);
                if (!string.IsNullOrEmpty(group))
                {
                    if (!StringUtils.ContainsIgnoreCase(channelInfo.GroupNames, group))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(groupNot))
                {
                    if (StringUtils.ContainsIgnoreCase(channelInfo.GroupNames, groupNot))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(contentModelPluginId))
                {
                    if (!StringUtils.EqualsIgnoreCase(channelInfo.ContentModelPluginId, contentModelPluginId))
                    {
                        continue;
                    }
                }
                filteredChannelIds.Add(channelInfo.Id);
            }

            return filteredChannelIds;
        }

        public async Task<bool> IsExistsAsync(int channelId)
        {
            var nodeInfo = await GetAsync(channelId);
            return nodeInfo != null;
        }

        public async Task<string> GetTableNameAsync(Site site, int channelId)
        {
            return await GetTableNameAsync(site, await GetAsync(channelId));
        }

        public async Task<string> GetTableNameAsync(Site site, IChannelSummary channel)
        {
            return channel != null ? await GetTableNameAsync(site, channel.ContentModelPluginId) : string.Empty;
        }

        private async Task<string> GetTableNameAsync(Site site, string pluginId)
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

        public async Task<bool> IsContentModelPluginAsync(Site site, Channel node)
        {
            if (string.IsNullOrEmpty(node.ContentModelPluginId)) return false;

            var contentTable = await PluginContentTableManager.GetTableNameAsync(node.ContentModelPluginId);
            return !string.IsNullOrEmpty(contentTable);
        }

        public async Task<string> GetNodeTreeLastImageHtmlAsync(Site site, Channel node)
        {
            var imageHtml = string.Empty;
            if (!string.IsNullOrEmpty(node.ContentModelPluginId) || 
                node.ContentRelatedPluginIds != null && node.ContentRelatedPluginIds.Any())
            {
                var list = await PluginContentManager.GetContentPluginsAsync(node, true);
                if (list != null && list.Count > 0)
                {
                    imageHtml += @"<i class=""ion-cube"" style=""font-size: 15px;vertical-align: baseline;""></i>&nbsp;";
                }
            }
            return imageHtml;
        }

        public async Task<List<string>> GetGroupNamesAsync(int channelId)
        {
            var channel = await GetAsync(channelId);
            return channel?.GroupNames ?? new List<string>();
        }

        public async Task<int> GetParentIdAsync(int siteId, int channelId)
        {
            var retVal = 0;
            var channel = await GetAsync(channelId);
            if (channel != null)
            {
                retVal = channel.ParentId;
            }
            return retVal;
        }

        private async Task<string> GetParentsPathAsync(int siteId, int channelId)
        {
            var retVal = string.Empty;
            var channel = await GetAsync(channelId);
            if (channel != null)
            {
                retVal = channel.ParentsPath;
            }
            return retVal;
        }

        public async Task<int> GetTopLevelAsync(int siteId, int channelId)
        {
            var parentsPath = await GetParentsPathAsync(siteId, channelId);
            return string.IsNullOrEmpty(parentsPath) ? 0 : parentsPath.Split(',').Length;
        }

        public async Task<string> GetChannelNameAsync(int siteId, int channelId)
        {
            var retVal = string.Empty;
            var channel = await GetAsync(channelId);
            if (channel != null)
            {
                retVal = channel.ChannelName;
            }
            return retVal;
        }

        public async Task<string> GetIndexNameAsync(int siteId, int channelId)
        {
            var retVal = string.Empty;
            var channel = await GetAsync(channelId);
            if (channel != null)
            {
                retVal = channel.IndexName;
            }
            return retVal;
        }

        public async Task<string> GetChannelNameNavigationAsync(int siteId, int channelId)
        {
            return await GetChannelNameNavigationAsync(siteId, siteId, channelId);
        }

        public async Task<string> GetChannelNameNavigationAsync(int siteId, int currentChannelId, int channelId)
        {
            var channelNames = new List<string>();

            if (channelId == 0) channelId = siteId;

            if (channelId == siteId)
            {
                var channel = await GetAsync(siteId);
                return channel.ChannelName;
            }
            if (channelId == currentChannelId)
            {
                var channel = await GetAsync(channelId);
                return channel.ChannelName;
            }

            var parentsPath = await GetParentsPathAsync(siteId, channelId);
            var channelIdList = new List<int>();
            var indexOf = -1;
            if (!string.IsNullOrEmpty(parentsPath))
            {
                channelIdList = Utilities.GetIntList(parentsPath);
                indexOf = channelIdList.IndexOf(currentChannelId);
            }
            channelIdList.Add(channelId);
            //channelIdList.Remove(siteId);

            for (var index = 0; index < channelIdList.Count; index++)
            {
                if (index > indexOf)
                {
                    var channel = await GetAsync(channelIdList[index]);
                    if (channel != null)
                    {
                        channelNames.Add(channel.ChannelName);
                    }
                }
            }

            return Utilities.ToString(channelNames, " > ");
        }

        public async Task AddListItemsAsync(ListItemCollection listItemCollection, Site site, bool isSeeOwning, bool isShowContentNum, PermissionsImpl permissionsImpl)
        {
            var list = await GetChannelIdListAsync(site.Id);
            var nodeCount = list.Count;
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
                var channel = await GetAsync(channelId);

                var listitem = new ListItem(await GetSelectTextAsync(site, channel, permissionsImpl, isShowContentNum), channel.Id.ToString());
                if (!enabled)
                {
                    listitem.Attributes.Add("style", "color:gray;");
                }
                listItemCollection.Add(listitem);
            }
        }

        public async Task AddListItemsForAddContentAsync(ListItemCollection listItemCollection, Site site, bool isSeeOwning, PermissionsImpl permissionsImpl)
        {
            var list = await GetChannelIdListAsync(site.Id);
            var nodeCount = list.Count;
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = await permissionsImpl.IsOwningChannelIdAsync(channelId);
                }

                var channel = await GetAsync(channelId);

                if (!enabled)
                {
                    continue;
                }

                var listitem = new ListItem(await GetSelectTextAsync(site, channel, permissionsImpl, true), channel.Id.ToString());
                listItemCollection.Add(listitem);
            }
        }

        private async Task<string> GetSelectTextAsync(Site site, Channel channel, PermissionsImpl adminPermissions, bool isShowContentNum)
        {
            var retVal = string.Empty;
            retVal = string.Concat(retVal, channel.ChannelName);

            if (isShowContentNum)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                retVal = string.Concat(retVal, " (", count, ")");
            }

            return retVal;
        }

        public async Task<string> GetContentAttributesOfDisplayAsync(int siteId, int channelId)
        {
            var channel = await GetAsync(channelId);
            if (channel == null) return string.Empty;
            if (siteId != channelId && string.IsNullOrEmpty(channel.ListColumns))
            {
                return await GetContentAttributesOfDisplayAsync(siteId, channel.ParentId);
            }
            return channel.ListColumns;
        }

        public async Task<List<InputStyle>> GetInputStylesAsync(Site site, Channel channel)
        {
            var items = new List<InputStyle>();

            var styleList = ColumnsManager.GetContentListStyles(await DataProvider.TableStyleRepository.GetContentStyleListAsync(site, channel));

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

        public async Task<bool> IsAncestorOrSelfAsync(int siteId, int parentId, int childId)
        {
            if (parentId == childId)
            {
                return true;
            }
            var channel = await GetAsync(childId);
            if (channel == null)
            {
                return false;
            }
            if (StringUtils.In(channel.ParentsPath, parentId.ToString()))
            {
                return true;
            }
            return false;
        }

        public async Task<List<KeyValuePair<int, string>>> GetChannelsAsync(int siteId, PermissionsImpl permissionsImpl, params string[] channelPermissions)
        {
            var options = new List<KeyValuePair<int, string>>();

            var list = await GetChannelIdListAsync(siteId);
            foreach (var channelId in list)
            {
                var enabled = await permissionsImpl.HasChannelPermissionsAsync(siteId, channelId, channelPermissions);

                if (enabled)
                {
                    var tuple = new KeyValuePair<int, string>(channelId,
                        await GetChannelNameNavigationAsync(siteId, channelId));
                    options.Add(tuple);
                }
            }

            return options;
        }

        public async Task<bool> IsCreatableAsync(Site site, Channel channel)
        {
            if (site == null || channel == null) return false;

            if (!string.IsNullOrEmpty(channel.LinkUrl)) return false;

            var isCreatable = false;

            var linkType = channel.LinkType;

            if (linkType == LinkType.None)
            {
                isCreatable = true;
            }
            else if (linkType == LinkType.NoLinkIfContentNotExists)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                isCreatable = count != 0;
            }
            else if (linkType == LinkType.LinkToOnlyOneContent)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                isCreatable = count != 1;
            }
            else if (linkType == LinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                if (count != 0 && count != 1)
                {
                    isCreatable = true;
                }
            }
            else if (linkType == LinkType.LinkToFirstContent)
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                isCreatable = count < 1;
            }
            else if (linkType == LinkType.NoLinkIfChannelNotExists)
            {
                isCreatable = channel.ChildrenCount != 0;
            }
            else if (linkType == LinkType.LinkToLastAddChannel)
            {
                isCreatable = channel.ChildrenCount <= 0;
            }
            else if (linkType == LinkType.LinkToFirstChannel)
            {
                isCreatable = channel.ChildrenCount <= 0;
            }

            return isCreatable;
        }
    }
}
