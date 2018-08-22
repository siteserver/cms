using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public static class RelatedIdentities
	{
        public static List<int> GetRelatedIdentities(int siteId, int relatedIdentity)
        {
            List<int> relatedIdentities = GetChannelRelatedIdentities(siteId, relatedIdentity);

            return relatedIdentities;
        }

        public static List<int> GetChannelRelatedIdentities(int siteId, int channelId)
        {
            var arraylist = new List<int>();
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (channelInfo != null)
            {
                var channelIdCollection = "0," + channelInfo.Id;
                if (channelInfo.ParentsCount > 0)
                {
                    channelIdCollection = "0," + channelInfo.ParentsPath + "," + channelInfo.Id;
                }

                arraylist = TranslateUtils.StringCollectionToIntList(channelIdCollection);
                arraylist.Reverse();
            }
            else
            {
                arraylist.Add(0);
            }
            return arraylist;
        }

        public static List<TableStyleInfo> GetTableStyleInfoList(SiteInfo siteInfo, int channelId)
        {
            List<int> relatedIdentities = GetChannelRelatedIdentities(siteInfo.Id, channelId);

            var tableName = ChannelManager.GetTableName(siteInfo, channelId);

            return TableStyleManager.GetTableStyleInfoList(tableName, relatedIdentities);
        }
	}
}
