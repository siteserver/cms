using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "导航实体", Description = "通过 {navigation.}  实体在模板中显示导航链接")]
    public class StlNavigationEntities
	{
        private StlNavigationEntities()
		{
		}

        public const string EntityName = "navigation";

        public static string PreviousChannel = "PreviousChannel";
        public static string NextChannel = "NextChannel";
        public static string PreviousContent = "PreviousContent";
        public static string NextContent = "NextContent";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {PreviousChannel, "上一栏目链接"},
	        {NextChannel, "下一栏目链接"},
	        {PreviousContent, "上一内容链接"},
	        {NextContent, "下一内容链接"}
	    };

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var attributeName = entityName.Substring(12, entityName.Length - 13);

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);

                if (StringUtils.EqualsIgnoreCase(PreviousChannel, attributeName) || StringUtils.EqualsIgnoreCase(NextChannel, attributeName))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(attributeName, PreviousChannel);
                    var siblingNodeId = DataProvider.NodeDao.GetNodeIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingNodeId != 0)
                    {
                        var siblingNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, siblingNodeId);
                        parsedContent = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, siblingNodeInfo);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(PreviousContent, attributeName) || StringUtils.EqualsIgnoreCase(NextContent, attributeName))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var taxis = contextInfo.ContentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(attributeName, PreviousContent);
                        var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                        var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                        var siblingContentId = BaiRongDataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentId);
                            parsedContent = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo);
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            if (string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = PageUtils.UnclickedUrl;
            }

            return parsedContent;
        }
	}
}
