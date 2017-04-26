using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
	public class StlNavigationEntities
	{
        private StlNavigationEntities()
		{
		}

        public const string EntityName = "Navigation";              //导航实体

        public static string PreviousChannel = "PreviousChannel";          //上一栏目链接
        public static string NextChannel = "NextChannel";                  //下一栏目链接
        public static string PreviousContent = "PreviousContent";          //上一内容链接
        public static string NextContent = "NextContent";                  //下一内容链接

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(PreviousChannel, "上一栏目链接");
                attributes.Add(NextChannel, "下一栏目链接");
                attributes.Add(PreviousContent, "上一内容链接");
                attributes.Add(NextContent, "下一内容链接");

                return attributes;
            }
        }

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var attributeName = entityName.Substring(12, entityName.Length - 13);

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelID);

                if (StringUtils.EqualsIgnoreCase(PreviousChannel, attributeName) || StringUtils.EqualsIgnoreCase(NextChannel, attributeName))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = true;
                    if (StringUtils.EqualsIgnoreCase(attributeName, PreviousChannel))
                    {
                        isNextChannel = false;
                    }
                    var siblingNodeID = DataProvider.NodeDao.GetNodeIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingNodeID != 0)
                    {
                        var siblingNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, siblingNodeID);
                        parsedContent = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, siblingNodeInfo);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(PreviousContent, attributeName) || StringUtils.EqualsIgnoreCase(NextContent, attributeName))
                {
                    if (contextInfo.ContentID != 0)
                    {
                        var taxis = contextInfo.ContentInfo.Taxis;
                        var isNextContent = true;
                        if (StringUtils.EqualsIgnoreCase(attributeName, PreviousContent))
                        {
                            isNextContent = false;
                        }
                        var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
                        var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelID);
                        var siblingContentID = BaiRongDataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelID, taxis, isNextContent);
                        if (siblingContentID != 0)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentID);
                            parsedContent = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo);
                        }
                    }
                }
            }
            catch { }

            if (string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = PageUtils.UnclickedUrl;
            }

            return parsedContent;
        }
	}
}
