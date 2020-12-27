using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        public async Task<int> GetSiteIdAsync(int channelId)
        {
            var channel = await GetAsync(channelId);
            return channel?.SiteId ?? 0;
        }

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        public async Task<Channel> GetChannelByLastAddDateAsyncTask(int siteId, int parentId)
        {
            var summaries = await GetSummariesAsync(siteId);
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
            var summaries = await GetSummariesAsync(siteId);
            var channelId = summaries
                .Where(x => x.ParentId == parentId)
                .OrderBy(x => x.Taxis)
                .Select(x => x.Id)
                .FirstOrDefault();
            return await GetAsync(channelId);
        }

        public async Task<int> GetIdByParentIdAndTaxisAsync(int siteId, int parentId, int taxis, bool isNextChannel)
        {
            var summaries = await GetSummariesAsync(siteId);

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

        public async Task<List<string>> GetIndexNamesAsync(int siteId)
        {
            var summaries = await GetSummariesAsync(siteId);
            return summaries
                .Where(x => !string.IsNullOrEmpty(x.IndexName))
                .Select(x => x.IndexName).ToList();
        }

        public async Task<bool> IsIndexNameExistsAsync(int siteId, string indexName)
        {
            var summaries = await GetSummariesAsync(siteId);
            return summaries
                .Where(x => !string.IsNullOrEmpty(x.IndexName))
                .Any(x => x.IndexName == indexName);
        }

        public async Task<int> GetSequenceAsync(int siteId, int channelId)
        {
            var channel = await GetAsync(channelId);
            var summaries = await GetSummariesAsync(siteId);
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

            if (extra == null) return null;

            var cascade = new Cascade<int>
            {
                Value = summary.Id,
                Label = summary.ChannelName,
                Children = await GetCascadeChildrenAsync(site, summary.Id, func)
            };

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

            var summaries = await GetSummariesAsync(site.Id);
            foreach (var cache in summaries)
            {
                if (cache == null) continue;
                if (cache.ParentId == parentId)
                {
                    var cascade = await GetCascadeAsync(site, cache, func);
                    if (cascade != null)
                    {
                        list.Add(cascade);
                    }
                }
            }

            return list;
        }

        public async Task<Cascade<int>> GetCascadeAsync(Site site, IChannelSummary summary)
        {
            return new Cascade<int>
            {
                Value = summary.Id,
                Label = summary.ChannelName,
                Children = await GetCascadeChildrenAsync(site, summary.Id)
            };
        }

        private async Task<List<Cascade<int>>> GetCascadeChildrenAsync(Site site, int parentId)
        {
            var list = new List<Cascade<int>>();

            var summaries = await GetSummariesAsync(site.Id);

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

            var summaries = await GetSummariesAsync(siteId);

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

            var summaries = await GetSummariesAsync(siteId);
            var channelInfo = summaries.FirstOrDefault(x => x != null && x.IndexName == indexName);
            return channelInfo?.Id ?? 0;
        }

        public async Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var summaries = await GetSummariesAsync(siteId);

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
                    channel = summaries.FirstOrDefault(x => (x.ParentId == parentId || ListUtils.Contains(x.ParentsPath, parentId)) && x.ChannelName == channelName);

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

        public async Task<List<Channel>> GetChannelsAsync(int siteId)
        {
            var summaries = await GetSummariesAsync(siteId);
            var list = new List<Channel>();
            foreach (var summary in summaries)
            {
                var channel = await GetAsync(summary.Id);
                list.Add(channel);
            }

            return list;
        }

        public async Task<List<int>> GetChannelIdsAsync(int siteId)
        {
            var summaries = await GetSummariesAsync(siteId);
            return summaries.OrderBy(c => c.Taxis).Select(x => x.Id).ToList();
        }

        public async Task<List<string>> GetChannelIndexNamesAsync(int siteId)
        {
            var summaries = await GetSummariesAsync(siteId);
            return summaries.OrderBy(c => c.Taxis).Where(channelInfo => !string.IsNullOrEmpty(channelInfo.IndexName)).Select(channelInfo => channelInfo.IndexName).ToList();
        }

        private void GetParentIdsRecursive(List<ChannelSummary> summaries, List<int> list, int channelId)
        {
            var summary = summaries.FirstOrDefault(x => x.Id == channelId);
            if (summary != null && summary.ParentId > 0)
            {
                list.Add(summary.ParentId);
                GetParentIdsRecursive(summaries, list, summary.ParentId);
            }
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

        public async Task<List<int>> GetChannelIdsAsync(int siteId, int channelId, ScopeType scopeType)
        {
            if (siteId == 0 || channelId == 0) return new List<int>();

            if (scopeType == ScopeType.Self)
            {
                return new List<int>{ channelId };
            }

            if (scopeType == ScopeType.SelfAndChildren)
            {
                var summaries = await GetSummariesAsync(siteId);
                var list = GetChildIds(summaries, channelId);
                list.Add(channelId);
                return list;
            }

            if (scopeType == ScopeType.Children)
            {
                var summaries = await GetSummariesAsync(siteId);
                return GetChildIds(summaries, channelId);
            }

            if (scopeType == ScopeType.Descendant)
            {
                var summaries = await GetSummariesAsync(siteId);
                var list = new List<int>();
                GetChildIdsRecursive(summaries, list, channelId);
                return list;
            }

            if (scopeType == ScopeType.All)
            {
                var summaries = await GetSummariesAsync(siteId);
                var list = new List<int> { channelId };
                GetChildIdsRecursive(summaries, list, channelId);
                return list;
            }

            return new List<int>();
        }

        public async Task<List<int>> GetChannelIdsAsync(Channel channel, ScopeType scopeType, string group, string groupNot, string contentModelPluginId)
        {
            if (channel == null) return new List<int>();

            var channelIds = await GetChannelIdsAsync(channel.SiteId, channel.Id, scopeType);

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
                    if (!ListUtils.ContainsIgnoreCase(channelInfo.GroupNames, group))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(groupNot))
                {
                    if (ListUtils.ContainsIgnoreCase(channelInfo.GroupNames, groupNot))
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
            var channel = await GetAsync(channelId);
            return channel != null;
        }

        public async Task<string> GetTableNameAsync(Site site, int channelId)
        {
            return GetTableName(site, await GetAsync(channelId));
        }

        public string GetTableName(Site site, IChannelSummary channel)
        {
            return channel != null ? GetTableName(site, channel.TableName) : string.Empty;
        }

        private string GetTableName(Site site, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return site.TableName;
            }

            var tableNames = _settingsManager.GetContentTableNames();
            return ListUtils.ContainsIgnoreCase(tableNames, tableName) ? tableName : site.TableName;
        }

        public bool IsContentModelPlugin(Site site, Channel channel)
        {
            return !string.IsNullOrEmpty(channel.TableName);
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

        private async Task<List<int>> GetParentsPathAsync(int channelId)
        {
            List<int> retVal = null;
            var channel = await GetAsync(channelId);
            if (channel != null)
            {
                retVal = channel.ParentsPath;
            }
            return retVal;
        }

        public async Task<int> GetTopLevelAsync(int siteId, int channelId)
        {
            var parentsPath = await GetParentsPathAsync(channelId);
            return parentsPath?.Count ?? 0;
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
            else if (channelId < 0) channelId = Math.Abs(channelId);

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

            var summaries = await GetSummariesAsync(siteId);
            var parentIds = new List<int>
            {
                channelId
            };
            GetParentIdsRecursive(summaries, parentIds, channelId);
            parentIds.Reverse();

            foreach (var parentId in parentIds)
            {
                if (parentId == currentChannelId)
                {
                    channelNames.Clear();
                }
                var channelName = await GetChannelNameAsync(siteId, parentId);
                channelNames.Add(channelName);
            }

            //for (var index = 0; index < channelIdList.Count; index++)
            //{
            //    if (index > indexOf)
            //    {
            //        var channel = await GetAsync(channelIdList[index]);
            //        if (channel != null)
            //        {
            //            channelNames.Add(channel.ChannelName);
            //        }
            //    }
            //}

            return ListUtils.ToString(channelNames, " > ");
        }

        public async Task<List<int>> GetChannelIdNavigationAsync(int siteId, int channelId)
        {
            var channelIds = new List<int>();

            if (channelId == 0) channelId = siteId;
            else if (channelId < 0) channelId = Math.Abs(channelId);

            if (channelId == siteId)
            {
                channelIds.Add(siteId);
                return channelIds;
            }

            var summaries = await GetSummariesAsync(siteId);

            var parentIds = new List<int>
            {
                channelId
            };
            GetParentIdsRecursive(summaries, parentIds, channelId);
            parentIds.Reverse();

            channelIds.AddRange(parentIds);

            return channelIds;
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
            if (ListUtils.Contains(channel.ParentsPath, parentId))
            {
                return true;
            }
            return false;
        }

        public async Task<List<KeyValuePair<int, string>>> GetChannelsAsync(int siteId, IAuthManager authManager, params string[] contentPermissions)
        {
            var options = new List<KeyValuePair<int, string>>();

            var list = await GetChannelIdsAsync(siteId);
            foreach (var channelId in list)
            {
                var enabled = await authManager.HasChannelPermissionsAsync(siteId, channelId, contentPermissions);

                if (enabled)
                {
                    var tuple = new KeyValuePair<int, string>(channelId,
                        await GetChannelNameNavigationAsync(siteId, channelId));
                    options.Add(tuple);
                }
            }

            return options;
        }

        public bool IsCreatable(Site site, Channel channel, int count)
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
                isCreatable = count != 0;
            }
            else if (linkType == LinkType.LinkToOnlyOneContent)
            {
                isCreatable = count != 1;
            }
            else if (linkType == LinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
            {
                if (count != 0 && count != 1)
                {
                    isCreatable = true;
                }
            }
            else if (linkType == LinkType.LinkToFirstContent)
            {
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
