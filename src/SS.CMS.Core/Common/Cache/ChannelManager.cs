using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.Security;
using SS.CMS.Utils;

namespace SS.CMS.Core.Cache
{
    public static class ChannelManager
    {
        private static class ChannelManagerCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(ChannelManager));

            private static void Update(Dictionary<int, Dictionary<int, ChannelInfo>> allDict, Dictionary<int, ChannelInfo> dic, int siteId)
            {
                lock (LockObject)
                {
                    allDict[siteId] = dic;
                }
            }

            private static Dictionary<int, Dictionary<int, ChannelInfo>> GetAllDictionary()
            {
                var allDict = DataCacheManager.Get<Dictionary<int, Dictionary<int, ChannelInfo>>>(CacheKey);
                if (allDict != null) return allDict;

                allDict = new Dictionary<int, Dictionary<int, ChannelInfo>>();
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

            public static void Update(int siteId, ChannelInfo channelInfo)
            {
                var dict = GetChannelInfoDictionaryBySiteId(siteId);

                lock (LockObject)
                {
                    dict[channelInfo.Id] = channelInfo;
                }
            }

            public static Dictionary<int, ChannelInfo> GetChannelInfoDictionaryBySiteId(int siteId)
            {
                var allDict = GetAllDictionary();

                Dictionary<int, ChannelInfo> dict;
                allDict.TryGetValue(siteId, out dict);

                if (dict != null) return dict;

                dict = DataProvider.ChannelRepository.GetChannelInfoDictionaryBySiteId(siteId);
                Update(allDict, dict, siteId);
                return dict;
            }
        }

        public static void RemoveCacheBySiteId(int siteId)
        {
            ChannelManagerCache.Remove(siteId);
            StlChannelCache.ClearCache();
        }

        public static void UpdateCache(int siteId, ChannelInfo channelInfo)
        {
            ChannelManagerCache.Update(siteId, channelInfo);
            StlChannelCache.ClearCache();
        }

        public static ChannelInfo GetChannelInfo(int siteId, int channelId)
        {
            ChannelInfo channelInfo = null;
            var dict = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
            if (channelId == 0) channelId = siteId;
            dict?.TryGetValue(Math.Abs(channelId), out channelInfo);
            return channelInfo;
        }

        public static int GetChannelId(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retval = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = GetChannelIdByIndexName(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = GetChannelIdByParentIdAndChannelName(siteId, retval, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = GetChannelIdByParentIdAndChannelName(siteId, siteId, channelName, true);
                }
                if (theChannelId != 0)
                {
                    retval = theChannelId;
                }
            }

            return retval;
        }

        public static int GetChannelIdByIndexName(int siteId, string indexName)
        {
            if (string.IsNullOrEmpty(indexName)) return 0;

            var dict = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
            var channelInfo = dict.Values.FirstOrDefault(x => x != null && x.IndexName == indexName);
            return channelInfo?.Id ?? 0;
        }

        public static int GetChannelIdByParentIdAndChannelName(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var dict = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
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

        //public static List<string> GetIndexNameList(int siteId)
        //{
        //    var dic = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
        //    return dic.Values.Where(x => !string.IsNullOrEmpty(x?.IndexName)).Select(x => x.IndexName).Distinct().ToList();
        //}

        public static List<ChannelInfo> GetChannelInfoList(int siteId)
        {
            var dic = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
            return dic.Values.Where(channelInfo => channelInfo != null).ToList();
        }

        public static List<int> GetChannelIdList(int siteId)
        {
            var dic = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
            return dic.Values.OrderBy(c => c.Taxis).Select(channelInfo => channelInfo.Id).ToList();
        }

        public static Dictionary<int, ChannelInfo> GetChannelInfoDictionary(int siteId)
        {
            return ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
        }

        public static List<int> GetChannelIdList(int siteId, string channelGroup)
        {
            var channelInfoList = new List<ChannelInfo>();
            var dic = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
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

        public static List<int> GetChannelIdList(ChannelInfo channelInfo, ScopeType scopeType)
        {
            return GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        }

        public static List<int> GetChannelIdList(ChannelInfo channelInfo, ScopeType scopeType, string group, string groupNot, string contentModelPluginId)
        {
            if (channelInfo == null) return new List<int>();

            var dic = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(channelInfo.SiteId);
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

        public static bool IsExists(int siteId, int channelId)
        {
            var nodeInfo = GetChannelInfo(siteId, channelId);
            return nodeInfo != null;
        }

        public static bool IsExists(ISiteRepository siteRepository, int channelId)
        {
            var list = siteRepository.GetSiteIdList();
            foreach (var siteId in list)
            {
                var nodeInfo = GetChannelInfo(siteId, channelId);
                if (nodeInfo != null) return true;
            }

            return false;
        }

        public static int GetChannelIdByParentsCount(int siteId, int channelId, int parentsCount)
        {
            if (parentsCount == 0) return siteId;
            if (channelId == 0 || channelId == siteId) return siteId;

            var nodeInfo = GetChannelInfo(siteId, channelId);
            if (nodeInfo != null)
            {
                return nodeInfo.ParentsCount == parentsCount ? nodeInfo.Id : GetChannelIdByParentsCount(siteId, nodeInfo.ParentId, parentsCount);
            }
            return siteId;
        }

        public static string GetTableName(IPluginManager pluginManager, SiteInfo siteInfo, int channelId)
        {
            return GetTableName(pluginManager, siteInfo, GetChannelInfo(siteInfo.Id, channelId));
        }

        public static string GetTableName(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            return channelInfo != null ? GetTableName(pluginManager, siteInfo, channelInfo.ContentModelPluginId) : string.Empty;
        }

        public static string GetTableName(IPluginManager pluginManager, SiteInfo siteInfo, string pluginId)
        {
            var tableName = siteInfo.TableName;

            if (string.IsNullOrEmpty(pluginId)) return tableName;

            var contentTable = pluginManager.GetContentTableName(pluginId);
            if (!string.IsNullOrEmpty(contentTable))
            {
                tableName = contentTable;
            }

            return tableName;
        }

        //public static ETableStyle GetTableStyle(SiteInfo siteInfo, int channelId)
        //{
        //    return GetTableStyle(siteInfo, GetChannelInfo(siteInfo.Id, channelId));
        //}

        //public static ETableStyle GetTableStyle(SiteInfo siteInfo, NodeInfo nodeInfo)
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

        public static bool IsContentModelPlugin(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            if (string.IsNullOrEmpty(nodeInfo.ContentModelPluginId)) return false;

            var contentTable = pluginManager.GetContentTableName(nodeInfo.ContentModelPluginId);
            return !string.IsNullOrEmpty(contentTable);
        }

        public static string GetNodeTreeLastImageHtml(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            var imageHtml = string.Empty;
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelPluginId) || !string.IsNullOrEmpty(nodeInfo.ContentRelatedPluginIds))
            {
                var list = pluginManager.GetContentPlugins(nodeInfo, true);
                if (list != null && list.Count > 0)
                {
                    imageHtml += @"<i class=""ion-cube"" style=""font-size: 15px;vertical-align: baseline;""></i>&nbsp;";
                }
            }
            return imageHtml;
        }

        public static DateTime GetAddDate(int siteId, int channelId)
        {
            var retval = DateTime.MinValue;
            var nodeInfo = GetChannelInfo(siteId, channelId);
            if (nodeInfo != null && nodeInfo.AddDate.HasValue)
            {
                retval = nodeInfo.AddDate.Value;
            }
            return retval;
        }

        public static int GetParentId(int siteId, int channelId)
        {
            var retval = 0;
            var nodeInfo = GetChannelInfo(siteId, channelId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.ParentId;
            }
            return retval;
        }

        public static string GetParentsPath(int siteId, int channelId)
        {
            var retval = string.Empty;
            var nodeInfo = GetChannelInfo(siteId, channelId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.ParentsPath;
            }
            return retval;
        }

        public static int GetTopLevel(int siteId, int channelId)
        {
            var parentsPath = GetParentsPath(siteId, channelId);
            return string.IsNullOrEmpty(parentsPath) ? 0 : parentsPath.Split(',').Length;
        }

        public static string GetChannelName(int siteId, int channelId)
        {
            var retval = string.Empty;
            var nodeInfo = GetChannelInfo(siteId, channelId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.ChannelName;
            }
            return retval;
        }

        public static string GetChannelNameNavigation(int siteId, int channelId)
        {
            var nodeNameList = new List<string>();

            if (channelId == 0) channelId = siteId;

            if (channelId == siteId)
            {
                var nodeInfo = GetChannelInfo(siteId, siteId);
                return nodeInfo.ChannelName;
            }
            var parentsPath = GetParentsPath(siteId, channelId);
            var channelIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                channelIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            channelIdList.Add(channelId);
            channelIdList.Remove(siteId);

            foreach (var theChannelId in channelIdList)
            {
                var nodeInfo = GetChannelInfo(siteId, theChannelId);
                if (nodeInfo != null)
                {
                    nodeNameList.Add(nodeInfo.ChannelName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(nodeNameList, " > ");
        }

        public static string GetSelectText(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo, Permissions adminPermissions, bool[] isLastNodeArray, bool isShowContentNum)
        {
            var retval = string.Empty;
            if (channelInfo.Id == channelInfo.SiteId)
            {
                channelInfo.LastNode = true;
            }
            if (channelInfo.LastNode == false)
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
            retval = string.Concat(retval, channelInfo.LastNode ? "└" : "├");
            retval = string.Concat(retval, channelInfo.ChannelName);

            if (isShowContentNum)
            {
                var onlyAdminId = adminPermissions.GetOnlyAdminId(siteInfo.Id, channelInfo.Id);
                var count = channelInfo.ContentRepository.GetCount(pluginManager, siteInfo, channelInfo, onlyAdminId);
                retval = string.Concat(retval, " (", count, ")");
            }

            return retval;
        }

        public static string GetContentAttributesOfDisplay(int siteId, int channelId)
        {
            var channelInfo = GetChannelInfo(siteId, channelId);
            if (channelInfo == null) return string.Empty;
            if (siteId != channelId && string.IsNullOrEmpty(channelInfo.ContentAttributesOfDisplay))
            {
                return GetContentAttributesOfDisplay(siteId, channelInfo.ParentId);
            }
            return channelInfo.ContentAttributesOfDisplay;
        }

        public static List<InputListItem> GetContentsColumns(ITableStyleRepository tableStyleRepository, IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll)
        {
            var items = new List<InputListItem>();

            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channelInfo.ContentAttributesOfDisplay);
            var pluginIds = pluginManager.GetContentPluginIds(channelInfo);
            var pluginColumns = pluginManager.GetContentColumns(pluginIds);

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(tableStyleRepository.GetContentStyleInfoList(pluginManager, siteInfo, channelInfo));

            styleInfoList.Insert(0, new TableStyleInfo
            {
                AttributeName = ContentAttribute.Sequence,
                DisplayName = "序号"
            });

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Type == InputType.TextEditor) continue;

                var listitem = new InputListItem
                {
                    Text = styleInfo.DisplayName,
                    Value = styleInfo.AttributeName
                };
                if (styleInfo.AttributeName == ContentAttribute.Title)
                {
                    listitem.Selected = true;
                }
                else
                {
                    if (attributesOfDisplay.Contains(styleInfo.AttributeName))
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

        public static bool IsAncestorOrSelf(int siteId, int parentId, int childId)
        {
            if (parentId == childId)
            {
                return true;
            }
            var nodeInfo = GetChannelInfo(siteId, childId);
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

        public static List<KeyValuePair<int, string>> GetChannels(int siteId, Permissions permissions, params string[] channelPermissions)
        {
            var options = new List<KeyValuePair<int, string>>();

            var list = GetChannelIdList(siteId);
            foreach (var channelId in list)
            {
                var enabled = permissions.HasChannelPermissions(siteId, channelId, channelPermissions);

                var channelInfo = GetChannelInfo(siteId, channelId);
                if (enabled && channelPermissions.Contains(Constants.ChannelPermissions.ContentAdd))
                {
                    if (channelInfo.IsContentAddable == false) enabled = false;
                }

                if (enabled)
                {
                    var tuple = new KeyValuePair<int, string>(channelId,
                        GetChannelNameNavigation(siteId, channelId));
                    options.Add(tuple);
                }
            }

            return options;
        }

        public static bool IsCreatable(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo)
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
                var count = channelInfo.ContentRepository.GetCount(pluginManager, siteInfo, channelInfo, true);
                isCreatable = count != 0;
            }
            else if (linkType == ELinkType.LinkToOnlyOneContent)
            {
                var count = channelInfo.ContentRepository.GetCount(pluginManager, siteInfo, channelInfo, true);
                isCreatable = count != 1;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
            {
                var count = channelInfo.ContentRepository.GetCount(pluginManager, siteInfo, channelInfo, true);
                if (count != 0 && count != 1)
                {
                    isCreatable = true;
                }
            }
            else if (linkType == ELinkType.LinkToFirstContent)
            {
                var count = channelInfo.ContentRepository.GetCount(pluginManager, siteInfo, channelInfo, true);
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