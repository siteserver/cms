using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.IO;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.Plugin.Features;

namespace SiteServer.CMS.Core
{
    public static class NodeManager
    {
        private static class NodeManagerCache
        {
            private static readonly object LockObject = new object();
            private const string CacheKey = "SiteServer.CMS.Core.NodeManager";
            private static readonly FileWatcherClass FileWatcher;

            static NodeManagerCache()
            {
                FileWatcher = new FileWatcherClass(FileWatcherClass.Node);
                FileWatcher.OnFileChange += FileWatcher_OnFileChange;
            }

            private static void FileWatcher_OnFileChange(object sender, EventArgs e)
            {
                CacheUtils.Remove(CacheKey);
            }

            private static void Update(Dictionary<int, Dictionary<int, NodeInfo>> allDict, Dictionary<int, NodeInfo> dic, int publishmentSystemId)
            {
                lock (LockObject)
                {
                    allDict[publishmentSystemId] = dic;
                }
            }

            private static Dictionary<int, Dictionary<int, NodeInfo>> GetAllDictionary()
            {
                var allDict = CacheUtils.Get(CacheKey) as Dictionary<int, Dictionary<int, NodeInfo>>;
                if (allDict != null) return allDict;

                allDict = new Dictionary<int, Dictionary<int, NodeInfo>>();
                CacheUtils.InsertHours(CacheKey, allDict, 24);
                return allDict;
            }

            public static void Remove(int publishmentSystemId)
            {
                var allDict = GetAllDictionary();

                lock (LockObject)
                {
                    allDict.Remove(publishmentSystemId);
                }

                FileWatcher.UpdateCacheFile();
            }

            public static Dictionary<int, NodeInfo> GetNodeInfoDictionaryByPublishmentSystemId(int publishmentSystemId)
            {
                var allDict = GetAllDictionary();

                Dictionary<int, NodeInfo> dict;
                allDict.TryGetValue(publishmentSystemId, out dict);

                if (dict != null) return dict;

                dict = DataProvider.NodeDao.GetNodeInfoDictionaryByPublishmentSystemId(publishmentSystemId);
                Update(allDict, dict, publishmentSystemId);
                return dict;
            }
        }

        public static void RemoveCache(int publishmentSystemId)
        {
            NodeManagerCache.Remove(publishmentSystemId);
            Node.ClearCache();
        }

        public static NodeInfo GetNodeInfo(int publishmentSystemId, int nodeId)
        {
            NodeInfo nodeInfo = null;
            var dict = NodeManagerCache.GetNodeInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            dict?.TryGetValue(nodeId, out nodeInfo);
            return nodeInfo;
        }

        public static List<NodeInfo> GetNodeInfoList(int publishmentSystemId)
        {
            var dic = NodeManagerCache.GetNodeInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            return dic.Values.Where(nodeInfo => nodeInfo != null).ToList();
        }

        public static List<int> GetNodeIdList(int publishmentSystemId)
        {
            var dic = NodeManagerCache.GetNodeInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            return dic.Keys.Where(nodeId => nodeId > 0).ToList();
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
            return nodeInfo != null ? GetTableName(publishmentSystemInfo, nodeInfo.ContentModelPluginId) : string.Empty;
        }

        public static string GetTableName(PublishmentSystemInfo publishmentSystemInfo, string contentModelId)
        {
            var tableName = publishmentSystemInfo.AuxiliaryTableForContent;

            if (string.IsNullOrEmpty(contentModelId)) return tableName;

            var contentTable = PluginManager.GetEnabledFeature<IContentModel>(contentModelId);
            if (!string.IsNullOrEmpty(contentTable?.ContentTableName))
            {
                tableName = contentTable.ContentTableName;
            }

            return tableName;
        }

        //public static ETableStyle GetTableStyle(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        //{
        //    return GetTableStyle(publishmentSystemInfo, GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId));
        //}

        //public static ETableStyle GetTableStyle(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
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

        public static bool IsContentModelPlugin(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            if (string.IsNullOrEmpty(nodeInfo.ContentModelPluginId)) return false;

            var retval = false;

            var contentTable = PluginManager.GetEnabledPluginMetadata<IContentModel>(nodeInfo.ContentModelPluginId);
            if (contentTable != null)
            {
                retval = true;
            }

            return retval;
        }

        public static string GetNodeTreeLastImageHtml(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var imageHtml = string.Empty;
            if (nodeInfo.ParentId == 0)
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
            if (!string.IsNullOrEmpty(nodeInfo.ContentRelatedPluginIds))
            {
                var plugins = PluginManager.GetContentRelatedPlugins(nodeInfo, false);
                foreach (var plugin in plugins)
                {
                    imageHtml +=
                        $@"<img align=""absmiddle"" title=""插件：{plugin.Title}"" border=""0"" src=""{PluginManager.GetPluginIconUrl(plugin)}"" width=""18"" height=""18"" />";
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
                if (!StringUtils.EqualsIgnoreCase(nodeInfo.ContentModelPluginId, contentModelId))
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