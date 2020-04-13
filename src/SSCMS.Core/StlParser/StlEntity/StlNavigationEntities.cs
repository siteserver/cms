using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "导航实体", Description = "通过 {navigation.}  实体在模板中显示导航链接")]
    public static class StlNavigationEntities
	{
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

        internal static async Task<string> ParseAsync(string stlEntity, IParseManager parseManager)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var parsedContent = string.Empty;
            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var attributeName = entityName.Substring(12, entityName.Length - 13);

                var nodeInfo = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);

                if (StringUtils.EqualsIgnoreCase(PreviousChannel, attributeName) || StringUtils.EqualsIgnoreCase(NextChannel, attributeName))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(attributeName, PreviousChannel);
                    //var siblingChannelId = databaseManager.ChannelRepository.GetIdByParentIdAndTaxis(node.ParentId, taxis, isNextChannel);
                    var siblingChannelId = await databaseManager.ChannelRepository.GetIdByParentIdAndTaxisAsync(pageInfo.SiteId, nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        var siblingNodeInfo = await databaseManager.ChannelRepository.GetAsync(siblingChannelId);
                        parsedContent = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, siblingNodeInfo, pageInfo.IsLocal);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(PreviousContent, attributeName) || StringUtils.EqualsIgnoreCase(NextContent, attributeName))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var contentInfo = await parseManager.GetContentAsync();
                        var taxis = contentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(attributeName, PreviousContent);
                        var tableName = await databaseManager.ChannelRepository.GetTableNameAsync(pageInfo.Site, contextInfo.ChannelId);
                        var siblingContentId = await databaseManager.ContentRepository.GetContentIdAsync(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            var siblingContentInfo = await databaseManager.ContentRepository.GetAsync(pageInfo.Site, contextInfo.ChannelId, siblingContentId);
                            parsedContent = await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, siblingContentInfo, pageInfo.IsLocal);
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
                parsedContent = PageUtils.UnClickableUrl;
            }

            return parsedContent;
        }
	}
}
