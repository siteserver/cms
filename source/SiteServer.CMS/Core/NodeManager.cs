using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.IO;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.CMS.Core
{
    public static class NodeManager
    {
        private const string CacheFileName = "NodeCache.txt";

        static NodeManager()
        {
            var fileWatcher = new FileWatcherClass(PathUtility.GetCacheFilePath(CacheFileName));
            fileWatcher.OnFileChange += fileWatcher_OnFileChange;
        }

        private static void fileWatcher_OnFileChange(object sender, EventArgs e)
        {
            CacheUtils.Remove(CacheKey);
        }

        public static Dictionary<int, NodeInfo> GetNodeInfoDictionaryByPublishmentSystemId(int publishmentSystemId)
        {
            return GetNodeInfoDictionaryByPublishmentSystemId(publishmentSystemId, false);
        }

        public static Dictionary<int, NodeInfo> GetNodeInfoDictionaryByPublishmentSystemId(int publishmentSystemId, bool flush)
        {
            var ht = GetActiveHashtable();

            Dictionary<int, NodeInfo> dic = null;

            if (!flush)
            {
                dic = ht[publishmentSystemId] as Dictionary<int, NodeInfo>;
            }

            if (dic != null) return dic;

            dic = DataProvider.NodeDao.GetNodeInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            UpdateCache(ht, dic, publishmentSystemId);
            return dic;
        }

        public static NodeInfo GetNodeInfo(int publishmentSystemId, int nodeId)
        {
            NodeInfo nodeInfo = null;
            var hashtable = GetNodeInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            hashtable?.TryGetValue(nodeId, out nodeInfo);
            return nodeInfo;
        }

        public static bool IsExists(int publishmentSystemId, int nodeId)
        {
            var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
            return nodeInfo != null;
        }

        public static bool IsExists(int nodeId)
        {
            var list = PublishmentSystemManager.GetPublishmentSystemIdList();
            foreach (var publishmentSystemId in list)
            {
                var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
                if (nodeInfo != null) return true;
            }

            return false;
        }

        public static int GetNodeIdByParentsCount(int publishmentSystemId, int nodeId, int parentsCount)
        {
            if (parentsCount == 0) return publishmentSystemId;
            if (nodeId == 0 || nodeId == publishmentSystemId) return publishmentSystemId;

            var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                return nodeInfo.ParentsCount == parentsCount ? nodeInfo.NodeId : GetNodeIdByParentsCount(publishmentSystemId, nodeInfo.ParentId, parentsCount);
            }
            return publishmentSystemId;
        }

        public static string GetTableName(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            return GetTableName(publishmentSystemInfo, GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId));
        }

        public static string GetTableName(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            return nodeInfo != null ? GetTableName(publishmentSystemInfo, nodeInfo.ContentModelId) : string.Empty;
        }

        public static string GetTableName(PublishmentSystemInfo publishmentSystemInfo, EAuxiliaryTableType tableType)
        {
            var tableName = string.Empty;
            if (Equals(EAuxiliaryTableType.BackgroundContent, tableType))
            {
                tableName = publishmentSystemInfo.AuxiliaryTableForContent;
            }
            return tableName;
        }

        public static string GetTableName(PublishmentSystemInfo publishmentSystemInfo, string contentModelId)
        {
            var tableName = publishmentSystemInfo.AuxiliaryTableForContent;

            if (string.IsNullOrEmpty(contentModelId)) return tableName;

            var contentTable = PluginCache.GetEnabledFeature<IContentTable>(contentModelId);
            if (!string.IsNullOrEmpty(contentTable?.ContentTableName))
            {
                tableName = contentTable.ContentTableName;
            }

            return tableName;
        }

        public static ETableStyle GetTableStyle(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            return GetTableStyle(publishmentSystemInfo, GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId));
        }

        public static ETableStyle GetTableStyle(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var tableStyle = ETableStyle.BackgroundContent;

            if (string.IsNullOrEmpty(nodeInfo.ContentModelId)) return tableStyle;

            var contentTable = PluginCache.GetEnabledPluginMetadata<IContentTable>(nodeInfo.ContentModelId);
            if (contentTable != null)
            {
                tableStyle = ETableStyle.Custom;
            }

            return tableStyle;
        }

        public static EAuxiliaryTableType GetTableType(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var tableType = EAuxiliaryTableType.BackgroundContent;

            if (string.IsNullOrEmpty(nodeInfo.ContentModelId)) return tableType;

            var contentTable = PluginCache.GetEnabledPluginMetadata<IContentTable>(nodeInfo.ContentModelId);
            if (contentTable != null)
            {
                tableType = EAuxiliaryTableType.Custom;
            }

            return tableType;
        }

        public static string GetNodeTreeLastImageHtml(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var imageHtml = string.Empty;
            if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode)
            {
                var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
                if (publishmentSystemInfo.IsHeadquarters == false)
                {
                    imageHtml =
                        $@"<img align=""absmiddle"" title=""站点"" border=""0"" src=""{PageUtils.Combine(treeDirectoryUrl,
                            "site.gif")}"" /></a>";
                }
                else
                {
                    imageHtml =
                        $@"<img align=""absmiddle"" title=""站点"" border=""0"" src=""{PageUtils.Combine(treeDirectoryUrl,
                            "siteHQ.gif")}"" />";
                }
            }
            if (!string.IsNullOrEmpty(nodeInfo.Additional.PluginIds))
            {
                var pluginChannels = PluginCache.GetChannels(nodeInfo, false);
                foreach (var pluginChannel in pluginChannels)
                {
                    imageHtml +=
                        $@"<img align=""absmiddle"" title=""插件：{pluginChannel.DisplayName}"" border=""0"" src=""{PageUtils.GetPluginDirectoryUrl(pluginChannel.Id, pluginChannel.Icon)}"" width=""18"" height=""18"" />";
                }
            }
            return imageHtml;
        }

        public static DateTime GetAddDate(int publishmentSystemId, int nodeId)
        {
            var retval = DateTime.MinValue;
            var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.AddDate;
            }
            return retval;
        }

        public static int GetParentId(int publishmentSystemId, int nodeId)
        {
            var retval = 0;
            var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.ParentId;
            }
            return retval;
        }

        public static string GetParentsPath(int publishmentSystemId, int nodeId)
        {
            var retval = string.Empty;
            var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.ParentsPath;
            }
            return retval;
        }

        public static int GetTopLevel(int publishmentSystemId, int nodeId)
        {
            var parentsPath = GetParentsPath(publishmentSystemId, nodeId);
            return string.IsNullOrEmpty(parentsPath) ? 0 : parentsPath.Split(',').Length;
        }

        public static string GetNodeName(int publishmentSystemId, int nodeId)
        {
            var retval = string.Empty;
            var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.NodeName;
            }
            return retval;
        }

        public static string GetNodeNameNavigation(int publishmentSystemId, int nodeId)
        {
            var nodeNameList = new List<string>();

            if (nodeId == 0) nodeId = publishmentSystemId;

            if (nodeId == publishmentSystemId)
            {
                var nodeInfo = GetNodeInfo(publishmentSystemId, publishmentSystemId);
                return nodeInfo.NodeName;
            }
            var parentsPath = GetParentsPath(publishmentSystemId, nodeId);
            var nodeIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                nodeIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            nodeIdList.Add(nodeId);
            nodeIdList.Remove(publishmentSystemId);

            foreach (var theNodeId in nodeIdList)
            {
                var nodeInfo = GetNodeInfo(publishmentSystemId, theNodeId);
                if (nodeInfo != null)
                {
                    nodeNameList.Add(nodeInfo.NodeName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(nodeNameList, " > ");
        }

        public static string GetNodeNameNavigationByGovPublic(int publishmentSystemId, int nodeId)
        {
            if (nodeId == 0 || publishmentSystemId == nodeId) return string.Empty;

            var nodeNameList = new List<string>();

            var parentsPath = GetParentsPath(publishmentSystemId, nodeId);
            var nodeIdList = new List<int>();
            if (!string.IsNullOrEmpty(parentsPath))
            {
                nodeIdList = TranslateUtils.StringCollectionToIntList(parentsPath);
            }
            nodeIdList.Add(nodeId);
            nodeIdList.Remove(publishmentSystemId);

            foreach (var theNodeId in nodeIdList)
            {
                var nodeInfo = GetNodeInfo(publishmentSystemId, theNodeId);
                if (nodeInfo != null && nodeInfo.ParentsCount >= 1)
                {
                    nodeNameList.Add(nodeInfo.NodeName);
                }
            }

            return TranslateUtils.ObjectCollectionToString(nodeNameList, " > ");
        }

        public static ENodeType GetNodeType(int publishmentSystemId, int nodeId)
        {
            var retval = ENodeType.BackgroundNormalNode;
            var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                retval = nodeInfo.NodeType;
            }
            return retval;
        }


        private static void UpdateCache(IDictionary ht, Dictionary<int, NodeInfo> dic, int publishmentSystemId)
        {
            lock (ht.SyncRoot)
            {
                ht[publishmentSystemId] = dic;
            }
        }

        public static void RemoveCache(int publishmentSystemId)
        {
            var ht = GetActiveHashtable();

            lock (ht.SyncRoot)
            {
                ht.Remove(publishmentSystemId);
            }

            CacheUtils.UpdateTemporaryCacheFile(CacheFileName);
        }

        private const string CacheKey = "SiteServer.CMS.Core.NodeManager";

        /// <summary>
        /// Returns a collection of SiteSettings which exist in the current Application Domain.
        /// </summary>
        /// <returns></returns>
        public static Hashtable GetActiveHashtable()
        {
            var ht = CacheUtils.Get(CacheKey) as Hashtable;
            if (ht != null) return ht;

            ht = new Hashtable();
            CacheUtils.InsertHours(CacheKey, ht, 24);
            return ht;
        }

        public static void AddListItems(ListItemCollection listItemCollection, PublishmentSystemInfo publishmentSystemInfo, bool isSeeOwning, bool isShowContentNum, string administratorName)
        {
            var list = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var nodeId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = AdminUtility.IsOwningNodeId(administratorName, nodeId);
                    if (!enabled)
                    {
                        if (!AdminUtility.IsHasChildOwningNodeId(administratorName, nodeId)) continue;
                    }
                }
                var nodeInfo = GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);

                var listitem = new ListItem(GetSelectText(publishmentSystemInfo, nodeInfo, isLastNodeArray, isShowContentNum), nodeInfo.NodeId.ToString());
                if (!enabled)
                {
                    listitem.Attributes.Add("style", "color:gray;");
                }
                listItemCollection.Add(listitem);
            }
        }

        public static void AddListItems(ListItemCollection listItemCollection, PublishmentSystemInfo publishmentSystemInfo, bool isSeeOwning, bool isShowContentNum, string contentModelId, string administratorName)
        {
            var list = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var nodeId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = AdminUtility.IsOwningNodeId(administratorName, nodeId);
                    if (!enabled)
                    {
                        if (!AdminUtility.IsHasChildOwningNodeId(administratorName, nodeId)) continue;
                    }
                }
                var nodeInfo = GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);

                var listitem = new ListItem(GetSelectText(publishmentSystemInfo, nodeInfo, isLastNodeArray, isShowContentNum), nodeInfo.NodeId.ToString());
                if (!enabled)
                {
                    listitem.Attributes.Add("style", "color:gray;");
                }
                if (!StringUtils.EqualsIgnoreCase(nodeInfo.ContentModelId, contentModelId))
                {
                    listitem.Attributes.Add("disabled", "disabled");
                }
                listItemCollection.Add(listitem);
            }
        }

        public static void AddListItemsForAddContent(ListItemCollection listItemCollection, PublishmentSystemInfo publishmentSystemInfo, bool isSeeOwning, string administratorName)
        {
            var list = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var nodeId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = AdminUtility.IsOwningNodeId(administratorName, nodeId);
                }

                var nodeInfo = GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                if (enabled)
                {
                    if (nodeInfo.Additional.IsContentAddable == false) enabled = false;
                }

                if (!enabled)
                {
                    continue;
                }

                var listitem = new ListItem(GetSelectText(publishmentSystemInfo, nodeInfo, isLastNodeArray, true), nodeInfo.NodeId.ToString());
                listItemCollection.Add(listitem);
            }
        }

        /// <summary>
        /// 得到栏目，并且不对（栏目是否可添加内容）进行判断
        /// 提供给触发器页面使用
        /// 使用场景：其他栏目的内容变动之后，设置某个栏目（此栏目不能添加内容）触发生成
        /// </summary>
        public static void AddListItemsForCreateChannel(ListItemCollection listItemCollection, PublishmentSystemInfo publishmentSystemInfo, bool isSeeOwning, string administratorName)
        {
            var list = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var nodeId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = AdminUtility.IsOwningNodeId(administratorName, nodeId);
                }

                var nodeInfo = GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);

                if (!enabled)
                {
                    continue;
                }

                var listitem = new ListItem(GetSelectText(publishmentSystemInfo, nodeInfo, isLastNodeArray, true), nodeInfo.NodeId.ToString());
                listItemCollection.Add(listitem);
            }
        }

        public static string GetSelectText(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, bool[] isLastNodeArray, bool isShowContentNum)
        {
            var retval = string.Empty;
            if (nodeInfo.NodeId == nodeInfo.PublishmentSystemId)
            {
                nodeInfo.IsLastNode = true;
            }
            if (nodeInfo.IsLastNode == false)
            {
                isLastNodeArray[nodeInfo.ParentsCount] = false;
            }
            else
            {
                isLastNodeArray[nodeInfo.ParentsCount] = true;
            }
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
            {
                retval = string.Concat(retval, isLastNodeArray[i] ? "　" : "│");
            }
            retval = string.Concat(retval, nodeInfo.IsLastNode ? "└" : "├");
            retval = string.Concat(retval, nodeInfo.NodeName);

            if (isShowContentNum)
            {
                retval = string.Concat(retval, " (", nodeInfo.ContentNum, ")");
            }

            return retval;
        }

        public static string GetContentAttributesOfDisplay(int publishmentSystemId, int nodeId)
        {
            var nodeInfo = GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo == null) return string.Empty;
            if (publishmentSystemId != nodeId && string.IsNullOrEmpty(nodeInfo.Additional.ContentAttributesOfDisplay))
            {
                return GetContentAttributesOfDisplay(publishmentSystemId, nodeInfo.ParentId);
            }
            return nodeInfo.Additional.ContentAttributesOfDisplay;
        }

        public static bool IsAncestorOrSelf(int publishmentSystemId, int parentId, int childId)
        {
            if (parentId == childId)
            {
                return true;
            }
            var nodeInfo = GetNodeInfo(publishmentSystemId, childId);
            if (nodeInfo == null)
            {
                return false;
            }
            if (CompareUtils.Contains(nodeInfo.ParentsPath, parentId.ToString()))
            {
                return true;
            }
            return false;
        }
    }

}