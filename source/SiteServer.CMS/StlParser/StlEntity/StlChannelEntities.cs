using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
	public class StlChannelEntities
	{
        private StlChannelEntities()
		{
		}

        public const string EntityName = "Channel";                  //栏目实体

        public static string ChannelID = "ChannelID";//栏目ID
        public static string ChannelName = "ChannelName";//栏目名称
        public static string ChannelIndex = "ChannelIndex";//栏目索引
		public static string Title = "Title";//栏目名称
        public static string Content = "Content";//栏目正文
        public static string NavigationUrl = "NavigationUrl";//栏目链接地址
        public static string ImageUrl = "ImageUrl";//栏目图片地址
        public static string AddDate = "AddDate";//栏目添加日期
        public static string DirectoryName = "DirectoryName";//生成文件夹名称
        public static string Group = "Group";//栏目组别
        public static string ItemIndex = "ItemIndex";//栏目排序

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(ChannelID, "栏目ID");
                attributes.Add(Title, "栏目名称");
                attributes.Add(ChannelName, "栏目名称");
                attributes.Add(ChannelIndex, "栏目索引");
                attributes.Add(Content, "栏目正文");
                attributes.Add(NavigationUrl, "栏目链接地址");
                attributes.Add(ImageUrl, "栏目图片地址");
                attributes.Add(AddDate, "栏目添加日期");
                attributes.Add(DirectoryName, "生成文件夹名称");
                attributes.Add(Group, "栏目组别");
                attributes.Add(ItemIndex, "栏目排序");
                
                return attributes;
            }
        }

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
                var channelID = contextInfo.ChannelID;
                if (!string.IsNullOrEmpty(channelIndex))
                {
                    channelID = DataProvider.NodeDao.GetNodeIdByNodeIndexName(pageInfo.PublishmentSystemId, channelIndex);
                    if (channelID == 0)
                    {
                        channelID = contextInfo.ChannelID;
                    }
                }
                
                if (attributeName.ToLower().StartsWith("up") && attributeName.IndexOf(".") != -1)
                {
                    if (attributeName.ToLower().StartsWith("up."))
                    {
                        upLevel = 1;
                    }
                    else
                    {
                        var upLevelStr = attributeName.Substring(2, attributeName.IndexOf(".") - 2);
                        upLevel = TranslateUtils.ToInt(upLevelStr);
                    }
                    topLevel = -1;
                    attributeName = attributeName.Substring(attributeName.IndexOf(".") + 1);
                }
                else if (attributeName.ToLower().StartsWith("top") && attributeName.IndexOf(".") != -1)
                {
                    if (attributeName.ToLower().StartsWith("top."))
                    {
                        topLevel = 1;
                    }
                    else
                    {
                        var topLevelStr = attributeName.Substring(3, attributeName.IndexOf(".") - 3);
                        topLevel = TranslateUtils.ToInt(topLevelStr);
                    }
                    upLevel = 0;
                    attributeName = attributeName.Substring(attributeName.IndexOf(".") + 1);
                }

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, channelID, upLevel, topLevel));

                if (StringUtils.EqualsIgnoreCase(ChannelID, attributeName))//栏目ID
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
            catch { }

            return parsedContent;
        }
	}
}
