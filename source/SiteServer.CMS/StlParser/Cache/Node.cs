using System.Collections.Generic;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Node
    {
        private static readonly object LockObject = new object();

        public static int GetPublishmentSystemId(int nodeId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node), nameof(GetPublishmentSystemId),
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

        public static List<int> GetNodeIdListByScopeType(int nodeId, int childrenCount, EScopeType scope, string group, string groupNot, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node), nameof(GetNodeIdListByScopeType),
                       nodeId.ToString(), childrenCount.ToString(), EScopeTypeUtils.GetValue(scope), group, groupNot);
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, childrenCount, scope, group, groupNot);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static int GetNodeIdByNodeIndexName(int publishmentSystemId, string channelIndex, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node), nameof(GetNodeIdByNodeIndexName),
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

        public static int GetNodeIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node), nameof(GetNodeIdByParentIdAndTaxis),
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

        public static int GetNodeIdByChannelIdOrChannelIndexOrChannelName(int publishmentSystemId, int channelId, string channelIndex, string channelName, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node),
                       nameof(GetNodeIdByChannelIdOrChannelIndexOrChannelName), publishmentSystemId.ToString(),
                       channelId.ToString(), channelIndex, channelName);
            var retval = StlCacheUtils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetIntCache(cacheKey);
                if (retval == -1)
                {
                    retval = DataProvider.NodeDao.GetNodeIdByChannelIdOrChannelIndexOrChannelName(publishmentSystemId,
                    channelId, channelIndex, channelName);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static string GetWhereString(int publishmentSystemId, string groupContent, string groupContentNot, bool isImageExists, bool isImage, string where, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node), nameof(GetWhereString),
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

        public static List<int> GetNodeIdList(int channelId, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node), nameof(GetNodeIdList),
                       channelId.ToString(), totalNum.ToString(), orderByString, whereString,
                       EScopeTypeUtils.GetValue(scopeType), groupChannel, groupChannelNot);
            var retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = StlCacheUtils.GetCache<List<int>>(cacheKey);
                if (retval == null)
                {
                    retval = DataProvider.NodeDao.GetNodeIdList(channelId, totalNum, orderByString, whereString, scopeType,
                    groupChannel, groupChannelNot);
                    StlCacheUtils.SetCache(cacheKey, retval);
                }
            }

            return retval;
        }

        public static NodeInfo GetNodeInfoByLastAddDate(int nodeId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node), nameof(GetNodeInfoByLastAddDate),
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

        public static NodeInfo GetNodeInfoByTaxis(int nodeId, string guid)
        {
            var cacheKey = StlCacheUtils.GetCacheKeyByGuid(guid, nameof(Node), nameof(GetNodeInfoByTaxis),
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
