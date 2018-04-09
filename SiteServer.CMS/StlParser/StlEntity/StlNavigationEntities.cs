using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [StlClass(Usage = "导航实体", Description = "通过 {navigation.}  实体在模板中显示导航链接")]
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

                var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId);

                if (StringUtils.EqualsIgnoreCase(PreviousChannel, attributeName) || StringUtils.EqualsIgnoreCase(NextChannel, attributeName))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(attributeName, PreviousChannel);
                    //var siblingChannelId = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    var siblingChannelId = Node.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        var siblingNodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, siblingChannelId);
                        parsedContent = PageUtility.GetChannelUrl(pageInfo.SiteInfo, siblingNodeInfo, pageInfo.IsLocal);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(PreviousContent, attributeName) || StringUtils.EqualsIgnoreCase(NextContent, attributeName))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var taxis = contextInfo.ContentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(attributeName, PreviousContent);
                        var tableName = ChannelManager.GetTableName(pageInfo.SiteInfo, contextInfo.ChannelId);
                        //var siblingContentId = DataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        var siblingContentId = Content.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            //var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentId);
                            var contentInfo = Content.GetContentInfo(tableName, siblingContentId);
                            parsedContent = PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, pageInfo.IsLocal);
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
