using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
	public class RelatedIdentities
	{
        private RelatedIdentities()
		{
		}

        public static List<int> GetRelatedIdentities(int siteId, int relatedIdentity)
        {
            List<int> relatedIdentities = GetChannelRelatedIdentities(siteId, relatedIdentity);

            return relatedIdentities;
        }

        public static List<int> GetChannelRelatedIdentities(int siteId, int nodeId)
        {
            var arraylist = new List<int>();
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, nodeId);
            if (nodeInfo != null)
            {
                var nodeIdCollection = "0," + nodeInfo.Id;
                if (nodeInfo.ParentsCount > 0)
                {
                    nodeIdCollection = "0," + nodeInfo.ParentsPath + "," + nodeInfo.Id;
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

        public static List<TableStyleInfo> GetTableStyleInfoList(SiteInfo siteInfo, int nodeId)
        {
            List<int> relatedIdentities = GetChannelRelatedIdentities(siteInfo.Id, nodeId);

            var tableName = ChannelManager.GetTableName(siteInfo, nodeId);

            return TableStyleManager.GetTableStyleInfoList(tableName, relatedIdentities);
        }
	}
}
