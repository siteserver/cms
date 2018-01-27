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

namespace SiteServer.CMS.Core
{
    public static class ChannelManager
    {
        private static class ChannelManagerCache
        {
            private static readonly object LockObject = new object();
            private const string CacheKey = "SiteServer.CMS.Core.ChannelManager";
            private static readonly FileWatcherClass FileWatcher;

            static ChannelManagerCache()
            {
                FileWatcher = new FileWatcherClass(FileWatcherClass.Node);
                FileWatcher.OnFileChange += FileWatcher_OnFileChange;
            }

            private static void FileWatcher_OnFileChange(object sender, EventArgs e)
            {
                CacheUtils.Remove(CacheKey);
            }

            private static void Update(Dictionary<int, Dictionary<int, ChannelInfo>> allDict, Dictionary<int, ChannelInfo> dic, int siteId)
            {
                lock (LockObject)
                {
                    allDict[siteId] = dic;
                }
            }

            private static Dictionary<int, Dictionary<int, ChannelInfo>> GetAllDictionary()
            {
                var allDict = CacheUtils.Get(CacheKey) as Dictionary<int, Dictionary<int, ChannelInfo>>;
                if (allDict != null) return allDict;

                allDict = new Dictionary<int, Dictionary<int, ChannelInfo>>();
                CacheUtils.InsertHours(CacheKey, allDict, 24);
                return allDict;
            }

            public static void Remove(int siteId)
            {
                var allDict = GetAllDictionary();

                lock (LockObject)
                {
                    allDict.Remove(siteId);
                }

                FileWatcher.UpdateCacheFile();
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

        public static void RemoveCache(int siteId)
        {
            ChannelManagerCache.Remove(siteId);
            Node.ClearCache();
        }

        public static ChannelInfo GetChannelInfo(int siteId, int channelId)
        {
            ChannelInfo nodeInfo = null;
            var dict = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
            dict?.TryGetValue(channelId, out nodeInfo);
            return nodeInfo;
        }

        public static List<ChannelInfo> GetChannelInfoList(int siteId)
        {
            var dic = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
            return dic.Values.Where(nodeInfo => nodeInfo != null).ToList();
        }

        public static List<int> GetChannelIdList(int siteId)
        {
            var dic = ChannelManagerCache.GetChannelInfoDictionaryBySiteId(siteId);
            return dic.Keys.Where(channelId => channelId > 0).ToList();
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

        public static string GetTableName(SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            return nodeInfo != null ? GetTableName(siteInfo, nodeInfo.ContentModelPluginId) : string.Empty;
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
            if (nodeInfo.ParentId == 0)
            {
                var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
                if (siteInfo.IsRoot == false)
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
                foreach (var service in PluginContentManager.GetContentPlugins(nodeInfo, false))
                {
                    imageHtml +=
                        $@"<img align=""absmiddle"" title=""插件：{service.Metadata.Title}"" border=""0"" src=""{PluginManager.GetPluginIconUrl(service)}"" width=""18"" height=""18"" />";
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

        public static void AddListItems(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, bool isShowContentNum, string administratorName)
        {
            var list = DataProvider.ChannelDao.GetIdListBySiteId(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = AdminUtility.IsOwningChannelId(administratorName, channelId);
                    if (!enabled)
                    {
                        if (!AdminUtility.IsHasChildOwningChannelId(administratorName, channelId)) continue;
                    }
                }
                var nodeInfo = GetChannelInfo(siteInfo.Id, channelId);

                var listitem = new ListItem(GetSelectText(siteInfo, nodeInfo, isLastNodeArray, isShowContentNum), nodeInfo.Id.ToString());
                if (!enabled)
                {
                    listitem.Attributes.Add("style", "color:gray;");
                }
                listItemCollection.Add(listitem);
            }
        }

        public static void AddListItems(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, bool isShowContentNum, string contentModelId, string administratorName)
        {
            var list = DataProvider.ChannelDao.GetIdListBySiteId(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = AdminUtility.IsOwningChannelId(administratorName, channelId);
                    if (!enabled)
                    {
                        if (!AdminUtility.IsHasChildOwningChannelId(administratorName, channelId)) continue;
                    }
                }
                var nodeInfo = GetChannelInfo(siteInfo.Id, channelId);

                var listitem = new ListItem(GetSelectText(siteInfo, nodeInfo, isLastNodeArray, isShowContentNum), nodeInfo.Id.ToString());
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

        public static void AddListItemsForAddContent(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, string administratorName)
        {
            var list = DataProvider.ChannelDao.GetIdListBySiteId(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = AdminUtility.IsOwningChannelId(administratorName, channelId);
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

                var listitem = new ListItem(GetSelectText(siteInfo, nodeInfo, isLastNodeArray, true), nodeInfo.Id.ToString());
                listItemCollection.Add(listitem);
            }
        }

        /// <summary>
        /// 得到栏目，并且不对（栏目是否可添加内容）进行判断
        /// 提供给触发器页面使用
        /// 使用场景：其他栏目的内容变动之后，设置某个栏目（此栏目不能添加内容）触发生成
        /// </summary>
        public static void AddListItemsForCreateChannel(ListItemCollection listItemCollection, SiteInfo siteInfo, bool isSeeOwning, string administratorName)
        {
            var list = DataProvider.ChannelDao.GetIdListBySiteId(siteInfo.Id);
            var nodeCount = list.Count;
            var isLastNodeArray = new bool[nodeCount];
            foreach (var channelId in list)
            {
                var enabled = true;
                if (isSeeOwning)
                {
                    enabled = AdminUtility.IsOwningChannelId(administratorName, channelId);
                }

                var nodeInfo = GetChannelInfo(siteInfo.Id, channelId);

                if (!enabled)
                {
                    continue;
                }

                var listitem = new ListItem(GetSelectText(siteInfo, nodeInfo, isLastNodeArray, true), nodeInfo.Id.ToString());
                listItemCollection.Add(listitem);
            }
        }

        public static string GetSelectText(SiteInfo siteInfo, ChannelInfo nodeInfo, bool[] isLastNodeArray, bool isShowContentNum)
        {
            var retval = string.Empty;
            if (nodeInfo.Id == nodeInfo.SiteId)
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
            retval = string.Concat(retval, nodeInfo.ChannelName);

            if (isShowContentNum)
            {
                retval = string.Concat(retval, " (", nodeInfo.ContentNum, ")");
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
            if (CompareUtils.Contains(nodeInfo.ParentsPath, parentId.ToString()))
            {
                return true;
            }
            return false;
        }
    }

}