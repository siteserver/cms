using System.Collections.Generic;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public class ChannelManager
    {
        private ChannelManager()
        {

        }

        public static int GetChannelIdByChannelIndex(int siteId, string channelIndex)
        {
            return DataProvider.NodeDao.GetNodeIdByNodeIndexName(siteId, channelIndex);
        }

        public static List<int> GetChannelIdList(int siteId, int channelId)
        {
            var nodeInfo = NodeManager.GetNodeInfo(siteId, channelId);
            return DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo.NodeId, nodeInfo.ChildrenCount, EScopeType.All, string.Empty, string.Empty);
        }

        public static NodeInfo GetNodeInfo(int siteId, int channelId)
        {
            return NodeManager.GetNodeInfo(siteId, channelId);
        }
    }
}
