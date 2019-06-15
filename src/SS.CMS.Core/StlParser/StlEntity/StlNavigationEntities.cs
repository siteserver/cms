using System.Collections.Generic;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "导航实体", Description = "通过 {navigation.}  实体在模板中显示导航链接")]
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

        internal static string Parse(string stlEntity, ParseContext parseContext)
        {
            var parsedContent = string.Empty;
            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var attributeName = entityName.Substring(12, entityName.Length - 13);

                var nodeInfo = ChannelManager.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId);

                if (StringUtils.EqualsIgnoreCase(PreviousChannel, attributeName) || StringUtils.EqualsIgnoreCase(NextChannel, attributeName))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(attributeName, PreviousChannel);
                    //var siblingChannelId = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    var siblingChannelId = StlChannelCache.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        var siblingNodeInfo = ChannelManager.GetChannelInfo(parseContext.SiteId, siblingChannelId);
                        parsedContent = parseContext.UrlManager.GetChannelUrl(parseContext.SiteInfo, siblingNodeInfo, parseContext.IsLocal);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(PreviousContent, attributeName) || StringUtils.EqualsIgnoreCase(NextContent, attributeName))
                {
                    if (parseContext.ContentId != 0)
                    {
                        var taxis = parseContext.ContentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(attributeName, PreviousContent);
                        var siblingContentId = parseContext.ChannelInfo.ContentRepository.StlGetContentId(parseContext.ChannelInfo, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            var contentInfo = parseContext.ChannelInfo.ContentRepository.GetContentInfo(parseContext.SiteInfo, parseContext.ChannelInfo, siblingContentId);
                            parsedContent = parseContext.UrlManager.GetContentUrl(parseContext.SiteInfo, contentInfo, parseContext.IsLocal);
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
