using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "栏目实体", Description = "通过 {channel.} 实体在模板中显示栏目值")]
    public class StlChannelEntities
	{
        private StlChannelEntities()
		{
		}

        public const string EntityName = "channel";

        public const string ChannelId = "ChannelID";
        public const string ChannelName = "ChannelName";
        public const string ChannelIndex = "ChannelIndex";
		public const string Title = "Title";
        public const string Content = "Content";
        public const string NavigationUrl = "NavigationUrl";
        public const string ImageUrl = "ImageUrl";
        public const string AddDate = "AddDate";
        public const string DirectoryName = "DirectoryName";
        public const string Group = "Group";
        public const string ItemIndex = "ItemIndex";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {ChannelId, "栏目ID"},
	        {Title, "栏目名称"},
	        {ChannelName, "栏目名称"},
	        {ChannelIndex, "栏目索引"},
	        {Content, "栏目正文"},
	        {NavigationUrl, "栏目链接地址"},
	        {ImageUrl, "栏目图片地址"},
	        {AddDate, "栏目添加日期"},
	        {DirectoryName, "生成文件夹名称"},
	        {Group, "栏目组别"},
	        {ItemIndex, "栏目排序"}
	    };

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var channelIndex = StlParserUtility.GetValueFromEntity(stlEntity);
                var attributeName = entityName.Substring(9, entityName.Length - 10);

                var upLevel = 0;
                var topLevel = -1;
                var channelId = contextInfo.ChannelId;
                if (!string.IsNullOrEmpty(channelIndex))
                {
                    channelId = DataProvider.NodeDao.GetNodeIdByNodeIndexName(pageInfo.PublishmentSystemId, channelIndex);
                    if (channelId == 0)
                    {
                        channelId = contextInfo.ChannelId;
                    }
                }
                
                if (attributeName.ToLower().StartsWith("up") && attributeName.IndexOf(".", StringComparison.Ordinal) != -1)
                {
                    if (attributeName.ToLower().StartsWith("up."))
                    {
                        upLevel = 1;
                    }
                    else
                    {
                        var upLevelStr = attributeName.Substring(2, attributeName.IndexOf(".", StringComparison.Ordinal) - 2);
                        upLevel = TranslateUtils.ToInt(upLevelStr);
                    }
                    topLevel = -1;
                    attributeName = attributeName.Substring(attributeName.IndexOf(".", StringComparison.Ordinal) + 1);
                }
                else if (attributeName.ToLower().StartsWith("top") && attributeName.IndexOf(".", StringComparison.Ordinal) != -1)
                {
                    if (attributeName.ToLower().StartsWith("top."))
                    {
                        topLevel = 1;
                    }
                    else
                    {
                        var topLevelStr = attributeName.Substring(3, attributeName.IndexOf(".", StringComparison.Ordinal) - 3);
                        topLevel = TranslateUtils.ToInt(topLevelStr);
                    }
                    upLevel = 0;
                    attributeName = attributeName.Substring(attributeName.IndexOf(".", StringComparison.Ordinal) + 1);
                }

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, channelId, upLevel, topLevel));

                if (StringUtils.EqualsIgnoreCase(ChannelId, attributeName))//栏目ID
                {
                    parsedContent = nodeInfo.NodeId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(Title, attributeName) || StringUtils.EqualsIgnoreCase(ChannelName, attributeName))//栏目名称
                {
                    parsedContent = nodeInfo.NodeName;
                }
                else if (StringUtils.EqualsIgnoreCase(ChannelIndex, attributeName))//栏目索引
                {
                    parsedContent = nodeInfo.NodeIndexName;
                }
                else if (StringUtils.EqualsIgnoreCase(Content, attributeName))//栏目正文
                {
                    parsedContent = StringUtility.TextEditorContentDecode(nodeInfo.Content, pageInfo.PublishmentSystemInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(NavigationUrl, attributeName))//栏目链接地址
                {
                    parsedContent = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, nodeInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(ImageUrl, attributeName))//栏目图片地址
                {
                    parsedContent = nodeInfo.ImageUrl;

                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, parsedContent);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(AddDate, attributeName))//栏目添加日期
                {
                    parsedContent = DateUtils.Format(nodeInfo.AddDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(DirectoryName, attributeName))//生成文件夹名称
                {
                    parsedContent = PathUtils.GetDirectoryName(nodeInfo.FilePath);
                }
                else if (StringUtils.EqualsIgnoreCase(Group, attributeName))//栏目组别
                {
                    parsedContent = nodeInfo.NodeGroupNameCollection;
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) && contextInfo.ItemContainer != null && contextInfo.ItemContainer.ChannelItem != null)
                {
                    parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ChannelItem.ItemIndex, attributeName, contextInfo).ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(NodeAttribute.Keywords, attributeName))//栏目组别
                {
                    parsedContent = nodeInfo.Keywords;
                }
                else if (StringUtils.EqualsIgnoreCase(NodeAttribute.Description, attributeName))//栏目组别
                {
                    parsedContent = nodeInfo.Description;
                }
                else
                {
                    var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.Channel, DataProvider.NodeDao.TableName, attributeName, RelatedIdentities.GetChannelRelatedIdentities(pageInfo.PublishmentSystemId, nodeInfo.NodeId));
                    parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, ",", pageInfo.PublishmentSystemInfo, ETableStyle.Channel, styleInfo, string.Empty, null, string.Empty, true);
                }
            }
            catch
            {
                // ignored
            }

            return parsedContent;
        }
	}
}
