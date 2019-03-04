using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache
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

                dict = DataProvider.ChannelDao.GetChannelInfoDictionaryBySiteId(siteId);
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

        public static List<int> GetChannelIdList(ChannelInfo channelInfo, EScopeType scopeType)
        {
            return GetChannelIdList(channelInfo, scopeType, string.Empty, string.Empty, string.Empty);
        }

        public static List<int> GetChannelIdList(ChannelInfo channelInfo, EScopeType scopeType, string group, string groupNot, string contentModelPluginId)
        {
            if (channelInfo == null) return new List<int>();

            var dic = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(channelInfo.SiteId);
            var channelInfoList = new List<ChannelInfo>();

            if (channelInfo.ChildrenCount == 0)
            {
                if (scopeType != EScopeType.Children && scopeType != EScopeType.Descendant)
                {
                    channelInfoList.Add(channelInfo);
                }
            }
            else if (scopeType == EScopeType.Self)
            {
                channelInfoList.Add(channelInfo);
            }
            else if (scopeType == EScopeType.All)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.Id == channelInfo.Id || nodeInfo.ParentId == channelInfo.Id || StringUtils.In(nodeInfo.ParentsPath, channelInfo.Id))
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == EScopeType.Children)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.ParentId == channelInfo.Id)
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == EScopeType.Descendant)
            {
                foreach (var nodeInfo in dic.Values)
                {
                    if (nodeInfo.ParentId == channelInfo.Id || StringUtils.In(nodeInfo.ParentsPath, channelInfo.Id))
                    {
                        channelInfoList.Add(nodeInfo);
                    }
                }
            }
            else if (scopeType == EScopeType.SelfAndChildren)
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

        public static bool IsExists(int channelId)
        {
            var list = SiteManager.GetSiteIdList();
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

        public static string GetTableName(SiteInfo siteInfo, int channelId)
        {
            return GetTableName(siteInfo, GetChannelInfo(siteInfo.Id, channelId));
        }

        public static string GetTableName(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            return channelInfo != null ? GetTableName(siteInfo, channelInfo.ContentModelPluginId) : string.Empty;
        }

        public static string GetTableName(SiteInfo siteInfo, string pluginId)
        {
            var tableName = siteInfo.TableName;

            if (string.IsNullOrEmpty(pluginId)) return tableName;

            var contentTable = PluginContentTableManager.GetTableName(pluginId);
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

        public static bool IsContentModelPlugin(SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            if (string.IsNullOrEmpty(nodeInfo.ContentModelPluginId)) return false;

            var contentTable = PluginContentTableManager.GetTableName(nodeInfo.ContentModelPluginId);
            return !string.IsNullOrEmpty(contentTable);
        }

        public static string GetNodeTreeLastImageHtml(SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            var imageHtml = string.Empty;
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelPluginId) || !string.IsNullOrEmpty(nodeInfo.ContentRelatedPluginIds))
            {
                var list = PluginContentManager.GetContentPlugins(nodeInfo, true);
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
            if (nodeInfo != null)
            {
                retval = nodeInfo.AddDate;
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

        public static void AddListItems(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, bool isShowContentNum, PermissionsImpl permissionsImpl)
        {
            var list = GetChannelIdList(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = permissionsImpl.IsOwningChannelId(channelId);
                    if (!enabled)
                    {
                        if (!permissionsImpl.IsDescendantOwningChannelId(siteInfo.Id, channelId)) continue;
                    }
                }
                var nodeInfo = GetChannelInfo(siteInfo.Id, channelId);

                var listitem = new ListItem(GetSelectText(siteInfo, nodeInfo, permissionsImpl, isLastNodeArray, isShowContentNum), nodeInfo.Id.ToString());
                if (!enabled)
                {
                    listitem.Attributes.Add("style", "color:gray;");
                }
                listItemCollection.Add(listitem);
            }
        }

        public static void AddListItems(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, bool isShowContentNum, string contentModelId, PermissionsImpl permissionsImpl)
        {
            var list = GetChannelIdList(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = permissionsImpl.IsOwningChannelId(channelId);
                    if (!enabled)
                    {
                        if (!permissionsImpl.IsDescendantOwningChannelId(siteInfo.Id, channelId)) continue;
                    }
                }
                var nodeInfo = GetChannelInfo(siteInfo.Id, channelId);

                var listitem = new ListItem(GetSelectText(siteInfo, nodeInfo, permissionsImpl, isLastNodeArray, isShowContentNum), nodeInfo.Id.ToString());
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

        public static void AddListItemsForAddContent(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, PermissionsImpl permissionsImpl)
        {
            var list = GetChannelIdList(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = permissionsImpl.IsOwningChannelId(channelId);
                }

                var nodeInfo = GetChannelInfo(siteInfo.Id, channelId);
                if (enabled)
                {
                    if (nodeInfo.Additional.IsContentAddable == false) enabled = false;
                }

                if (!enabled)
                {
                    continue;
                }

                var listitem = new ListItem(GetSelectText(siteInfo, nodeInfo, permissionsImpl, isLastNodeArray, true), nodeInfo.Id.ToString());
                listItemCollection.Add(listitem);
            }
        }

        /// <summary>
        /// 得到栏目，并且不对（栏目是否可添加内容）进行判断
        /// 提供给触发器页面使用
        /// 使用场景：其他栏目的内容变动之后，设置某个栏目（此栏目不能添加内容）触发生成
        /// </summary>
        public static void AddListItemsForCreateChannel(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, PermissionsImpl permissionsImpl)
        {
            var list = GetChannelIdList(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = permissionsImpl.IsOwningChannelId(channelId);
                }

                var nodeInfo = GetChannelInfo(siteInfo.Id, channelId);

                if (!enabled)
                {
                    continue;
                }

                var listitem = new ListItem(GetSelectText(siteInfo, nodeInfo, permissionsImpl, isLastNodeArray, true), nodeInfo.Id.ToString());
                listItemCollection.Add(listitem);
            }
        }

        public static string GetSelectText(SiteInfo siteInfo, ChannelInfo channelInfo, PermissionsImpl adminPermissions, bool[] isLastNodeArray, bool isShowContentNum)
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
                var onlyAdminId = adminPermissions.GetOnlyAdminId(siteInfo.Id, channelInfo.Id);
                var count = ContentManager.GetCount(siteInfo, channelInfo, onlyAdminId);
                retval = string.Concat(retval, " (", count, ")");
            }

            return retval;
        }

        public static string GetContentAttributesOfDisplay(int siteId, int channelId)
        {
            var nodeInfo = GetChannelInfo(siteId, channelId);
            if (nodeInfo == null) return string.Empty;
            if (siteId != channelId && string.IsNullOrEmpty(nodeInfo.Additional.ContentAttributesOfDisplay))
            {
                return GetContentAttributesOfDisplay(siteId, nodeInfo.ParentId);
            }
            return nodeInfo.Additional.ContentAttributesOfDisplay;
        }

        public static List<InputListItem> GetContentsColumns(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll)
        {
            var items = new List<InputListItem>();

            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channelInfo.Additional.ContentAttributesOfDisplay);
            var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
            var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo));

            styleInfoList.Insert(0, new TableStyleInfo
            {
                AttributeName = ContentAttribute.Sequence,
                DisplayName = "序号"
            });

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.InputType == InputType.TextEditor) continue;

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

        public static List<KeyValuePair<int, string>> GetChannels(int siteId, PermissionsImpl permissionsImpl, params string[] channelPermissions)
        {
            var options = new List<KeyValuePair<int, string>>();

            var list = GetChannelIdList(siteId);
            foreach (var channelId in list)
            {
                var enabled = permissionsImpl.HasChannelPermissions(siteId, channelId, channelPermissions);

                var channelInfo = GetChannelInfo(siteId, channelId);
                if (enabled && channelPermissions.Contains(ConfigManager.ChannelPermissions.ContentAdd))
                {
                    if (channelInfo.Additional.IsContentAddable == false) enabled = false;
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

        public static bool IsCreatable(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            if (siteInfo == null || channelInfo == null) return false;

            if (!channelInfo.Additional.IsChannelCreatable || !string.IsNullOrEmpty(channelInfo.LinkUrl)) return false;

            var isCreatable = false;

            var linkType = ELinkTypeUtils.GetEnumType(channelInfo.LinkType);

            if (linkType == ELinkType.None)
            {
                isCreatable = true;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExists)
            {
                var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                isCreatable = count != 0;
            }
            else if (linkType == ELinkType.LinkToOnlyOneContent)
            {
                var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                isCreatable = count != 1;
            }
            else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
            {
                var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                if (count != 0 && count != 1)
                {
                    isCreatable = true;
                }
            }
            else if (linkType == ELinkType.LinkToFirstContent)
            {
                var count = ContentManager.GetCount(siteInfo, channelInfo, true);
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