using System.Collections.Generic;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.StlParser.Cache
{
    public class Node
    {
        public static int GetPublishmentSystemId(int nodeId, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Node), nameof(GetPublishmentSystemId), guid, nodeId.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.NodeDao.GetPublishmentSystemId(nodeId);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static List<int> GetNodeIdListByScopeType(int nodeId, int childrenCount, EScopeType scope, string group, string groupNot, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Node), nameof(GetNodeIdListByScopeType), guid, nodeId.ToString(), childrenCount.ToString(), EScopeTypeUtils.GetValue(scope), group, groupNot);
            var retval = Utils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, childrenCount, scope, group, groupNot);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetNodeIdByNodeIndexName(int publishmentSystemId, string channelIndex, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Node), nameof(GetNodeIdByNodeIndexName), guid, publishmentSystemId.ToString(), channelIndex);
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.NodeDao.GetNodeIdByNodeIndexName(publishmentSystemId, channelIndex);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetNodeIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Node), nameof(GetNodeIdByParentIdAndTaxis), guid, parentId.ToString(), taxis.ToString(), isNextChannel.ToString());
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.NodeDao.GetNodeIdByParentIdAndTaxis(parentId, taxis, isNextChannel);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static int GetNodeIdByChannelIdOrChannelIndexOrChannelName(int publishmentSystemId, int channelId, string channelIndex, string channelName, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Node), nameof(GetNodeIdByChannelIdOrChannelIndexOrChannelName), guid, publishmentSystemId.ToString(), channelId.ToString(), channelIndex, channelName);
            var retval = Utils.GetIntCache(cacheKey);
            if (retval != -1) return retval;

            retval = DataProvider.NodeDao.GetNodeIdByChannelIdOrChannelIndexOrChannelName(publishmentSystemId, channelId, channelIndex, channelName);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static string GetWhereString(int publishmentSystemId, string groupContent, string groupContentNot, bool isImageExists, bool isImage, string where, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Node), nameof(GetWhereString), guid, publishmentSystemId.ToString(), groupContent, groupContentNot, isImageExists.ToString(), isImage.ToString(), where);
            var retval = Utils.GetCache<string>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.NodeDao.GetWhereString(publishmentSystemId, groupContent, groupContentNot, isImageExists, isImage, where);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }

        public static List<int> GetNodeIdList(int channelId, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot, string guid)
        {
            var cacheKey = Utils.GetCacheKey(nameof(Node), nameof(GetNodeIdList), guid, channelId.ToString(), totalNum.ToString(), orderByString, whereString, EScopeTypeUtils.GetValue(scopeType), groupChannel, groupChannelNot);
            var retval = Utils.GetCache<List<int>>(cacheKey);
            if (retval != null) return retval;

            retval = DataProvider.NodeDao.GetNodeIdList(channelId, totalNum, orderByString, whereString, scopeType, groupChannel, groupChannelNot);
            Utils.SetCache(cacheKey, retval);
            return retval;
        }
    }
}
