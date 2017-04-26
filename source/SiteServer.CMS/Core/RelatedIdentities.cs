using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Model;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Core
{
	public class RelatedIdentities
	{
        private RelatedIdentities()
		{
		}

        public static List<int> GetRelatedIdentities(ETableStyle tableStyle, int publishmentSystemId, int relatedIdentity)
        {
            List<int> relatedIdentities;

            if (tableStyle == ETableStyle.Channel || tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.VoteContent || tableStyle == ETableStyle.JobContent || tableStyle == ETableStyle.UserDefined)
            {
                relatedIdentities = GetChannelRelatedIdentities(publishmentSystemId, relatedIdentity);
            }
            else
            {
                relatedIdentities = GetRelatedIdentities(relatedIdentity);
            }

            return relatedIdentities;
        }

        private static List<int> GetRelatedIdentities(int relatedIdentity)
        {
            if (relatedIdentity == 0)
            {
                return TranslateUtils.StringCollectionToIntList("0");
            }
            return TranslateUtils.StringCollectionToIntList(relatedIdentity + ",0");
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

        public static List<TableStyleInfo> GetTableStyleInfoList(PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, int relatedIdentity)
        {
            List<int> relatedIdentities;
            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.Channel || tableStyle == ETableStyle.GovInteractContent || tableStyle == ETableStyle.GovPublicContent || tableStyle == ETableStyle.VoteContent || tableStyle == ETableStyle.JobContent || tableStyle == ETableStyle.UserDefined)
            {
                relatedIdentities = GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, relatedIdentity);
            }
            else
            {
                relatedIdentities = GetRelatedIdentities(relatedIdentity);
            }

            var tableName = GetTableName(publishmentSystemInfo, tableStyle, relatedIdentity);

            return TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
        }

        public static string GetTableName(PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, int relatedIdentity)
        {
            var tableName = publishmentSystemInfo.AuxiliaryTableForContent;

            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.GovInteractContent || tableStyle == ETableStyle.GovPublicContent || tableStyle == ETableStyle.VoteContent || tableStyle == ETableStyle.JobContent || tableStyle == ETableStyle.UserDefined)
            {
                tableName = NodeManager.GetTableName(publishmentSystemInfo, relatedIdentity);
            }
            else if (tableStyle == ETableStyle.Channel)
            {
                tableName = DataProvider.NodeDao.TableName;
            }
            else if (tableStyle == ETableStyle.InputContent)
            {
                tableName = DataProvider.InputContentDao.TableName;
            }

            return tableName;
        }
	}
}
