using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Node
    {
        private static readonly object LockObject = new object();

        public static int GetPublishmentSystemId(int nodeId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetPublishmentSystemId),
                    nodeId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.NodeDao.GetPublishmentSystemId(nodeId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetSequence(int publishmentSystemId, int nodeId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetSequence),
                publishmentSystemId.ToString(), nodeId.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.NodeDao.GetSequence(publishmentSystemId, nodeId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<int> GetNodeIdListByScopeType(int nodeId, int childrenCount, EScopeType scope, string group, string groupNot)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetNodeIdListByScopeType),
                       nodeId.ToString(), childrenCount.ToString(), EScopeTypeUtils.GetValue(scope), group, groupNot);
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, childrenCount, scope, group, groupNot, string.Empty);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSourceByPublishmentSystemId(int publishmentSystemId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetStlDataSourceByPublishmentSystemId),
                       publishmentSystemId.ToString(), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
            var retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetStlDataSourceByPublishmentSystemId(publishmentSystemId, startNum, totalNum, whereString, orderByString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static DataSet GetStlDataSet(List<int> nodeIdList, int startNum, int totalNum, string whereString, string orderByString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetStlDataSet),
                       TranslateUtils.ObjectCollectionToString(nodeIdList), startNum.ToString(), totalNum.ToString(), whereString, orderByString);
            var retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<DataSet>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetStlDataSet(nodeIdList, startNum, totalNum, whereString, orderByString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetNodeIdByNodeIndexName(int publishmentSystemId, string channelIndex)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetNodeIdByNodeIndexName),
                       publishmentSystemId.ToString(), channelIndex);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.NodeDao.GetNodeIdByNodeIndexName(publishmentSystemId, channelIndex);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetNodeIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetNodeIdByParentIdAndTaxis),
                       parentId.ToString(), taxis.ToString(), isNextChannel.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.NodeDao.GetNodeIdByParentIdAndTaxis(parentId, taxis, isNextChannel);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetNodeIdByParentIdAndNodeName(int publishmentSystemId, int parentId, string nodeName, bool recursive)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node),
                       nameof(GetNodeIdByParentIdAndNodeName), publishmentSystemId.ToString(),
                       parentId.ToString(), nodeName, recursive.ToString());
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.NodeDao.GetNodeIdByParentIdAndNodeName(publishmentSystemId,
                    parentId, nodeName, recursive);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetWhereString(int publishmentSystemId, string groupContent, string groupContentNot, bool isImageExists, bool isImage, string where)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetWhereString),
                       publishmentSystemId.ToString(), groupContent, groupContentNot, isImageExists.ToString(),
                       isImage.ToString(), where);
            var retval = StlCacheUtils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<string>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetWhereString(publishmentSystemId, groupContent, groupContentNot,
                    isImageExists, isImage, where);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<int> GetNodeIdListByScopeType(int channelId, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetNodeIdListByScopeType),
                       channelId.ToString(), EScopeTypeUtils.GetValue(scopeType), groupChannel, groupChannelNot);
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetNodeIdListByScopeType(channelId, scopeType, groupChannel, groupChannelNot);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static List<int> GetNodeIdListByTotalNum(List<int> nodeIdList, int totalNum, string orderByString, string whereString)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetNodeIdListByTotalNum),
                       TranslateUtils.ObjectCollectionToString(nodeIdList), totalNum.ToString(), orderByString, whereString);
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetNodeIdListByTotalNum(nodeIdList, totalNum, orderByString, whereString);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static NodeInfo GetNodeInfoByLastAddDate(int nodeId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetNodeInfoByLastAddDate),
                    nodeId.ToString());
            var retval = StlCacheUtils.GetCache<NodeInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<NodeInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetNodeInfoByLastAddDate(nodeId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static NodeInfo GetNodeInfoByTaxis(int nodeId)
        {
            var cacheKey = StlCacheUtils.GetCacheKey(nameof(Node), nameof(GetNodeInfoByTaxis),
                    nodeId.ToString());
            var retval = StlCacheUtils.GetCache<NodeInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<NodeInfo>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetNodeInfoByTaxis(nodeId);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
