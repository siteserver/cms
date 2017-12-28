using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Model;
using BaiRong.Core.Model;
using BaiRong.Core.Table;

namespace SiteServer.CMS.Core
{
	public class RelatedIdentities
	{
        private RelatedIdentities()
		{
		}

        public static List<int> GetRelatedIdentities(int publishmentSystemId, int relatedIdentity)
        {
            List<int> relatedIdentities = GetChannelRelatedIdentities(publishmentSystemId, relatedIdentity);

            return relatedIdentities;
        }

        public static List<int> GetChannelRelatedIdentities(int publishmentSystemId, int nodeId)
        {
            var arraylist = new List<int>();
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo != null)
            {
                var nodeIdCollection = "0," + nodeInfo.NodeId;
                if (nodeInfo.ParentsCount > 0)
                {
                    nodeIdCollection = "0," + nodeInfo.ParentsPath + "," + nodeInfo.NodeId;
                }

                arraylist = TranslateUtils.StringCollectionToIntList(nodeIdCollection);
                arraylist.Reverse();
            }
            else
            {
                arraylist.Add(0);
            }
            return arraylist;
        }

        public static List<TableStyleInfo> GetTableStyleInfoList(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            List<int> relatedIdentities = GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeId);

            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);

            return TableStyleManager.GetTableStyleInfoList(tableName, relatedIdentities);
        }
	}
}
