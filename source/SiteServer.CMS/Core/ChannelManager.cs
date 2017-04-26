using System.Collections;
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

        public static int GetChannelIDByChannelIndex(int siteID, string channelIndex)
        {
            return DataProvider.NodeDao.GetNodeIdByNodeIndexName(siteID, channelIndex);
        }

        public static List<int> GetChannelIDList(int siteID, int channelID)
        {
            var nodeInfo = NodeManager.GetNodeInfo(siteID, channelID);
            return DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo, EScopeType.All, string.Empty, string.Empty);
        }

        public static NodeInfo GetNodeInfo(int siteID, int channelID)
        {
            return NodeManager.GetNodeInfo(siteID, channelID);
        }
    }
}
