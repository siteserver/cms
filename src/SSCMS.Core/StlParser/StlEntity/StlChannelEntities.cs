using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "栏目实体", Description = "通过 {channel.} 实体在模板中显示栏目值")]
    public static class StlChannelEntities
	{
        public const string EntityName = "channel";

        private const string ChannelId = nameof(ChannelId);
	    private const string ChannelName = nameof(ChannelName);
        private const string ChannelIndex = nameof(ChannelIndex);
        private const string Title = nameof(Title);
        private const string Content = nameof(Content);
        private const string NavigationUrl = nameof(NavigationUrl);
        private const string ImageUrl = nameof(ImageUrl);
        private const string AddDate = nameof(AddDate);
        private const string DirectoryName = nameof(DirectoryName);
        private const string Group = nameof(Group);
        private const string ItemIndex = nameof(ItemIndex);

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

        internal static async Task<string> ParseAsync(string stlEntity, IParseManager parseManager)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

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
                    //channelId = databaseManager.ChannelRepository.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                    channelId = await databaseManager.ChannelRepository.GetChannelIdByIndexNameAsync(pageInfo.SiteId, channelIndex);
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

                var dataManager = new StlDataManager(parseManager.DatabaseManager);
                var nodeInfo = await databaseManager.ChannelRepository.GetAsync(await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, channelId, upLevel, topLevel));

                if (StringUtils.EqualsIgnoreCase(ChannelId, attributeName))//栏目ID
                {
                    parsedContent = nodeInfo.Id.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(Title, attributeName) || StringUtils.EqualsIgnoreCase(ChannelName, attributeName))//栏目名称
                {
                    parsedContent = nodeInfo.ChannelName;
                }
                else if (StringUtils.EqualsIgnoreCase(ChannelIndex, attributeName))//栏目索引
                {
                    parsedContent = nodeInfo.IndexName;
                }
                else if (StringUtils.EqualsIgnoreCase(Content, attributeName))//栏目正文
                {
                    parsedContent = await parseManager.PathManager.DecodeTextEditorAsync(pageInfo.Site, nodeInfo.Content, pageInfo.IsLocal);
                }
                else if (StringUtils.EqualsIgnoreCase(NavigationUrl, attributeName))//栏目链接地址
                {
                    parsedContent = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, nodeInfo, pageInfo.IsLocal);
                }
                else if (StringUtils.EqualsIgnoreCase(ImageUrl, attributeName))//栏目图片地址
                {
                    parsedContent = nodeInfo.ImageUrl;

                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = await parseManager.PathManager.ParseNavigationUrlAsync(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(AddDate, attributeName))//栏目添加日期
                {
                    parsedContent = DateUtils.Format(nodeInfo.AddDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(DirectoryName, attributeName))//生成文件夹名称
                {
                    parsedContent = PathUtils.GetDirectoryName(nodeInfo.FilePath, true);
                }
                else if (StringUtils.EqualsIgnoreCase(Group, attributeName))//栏目组别
                {
                    parsedContent = Utilities.ToString(nodeInfo.GroupNames);
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ChannelItem != null)
                {
                    parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ChannelItem.Key, attributeName, contextInfo).ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(Channel.Keywords), attributeName))//栏目组别
                {
                    parsedContent = nodeInfo.Keywords;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(Channel.Description), attributeName))//栏目组别
                {
                    parsedContent = nodeInfo.Description;
                }
                else
                {
                    //var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.Channel, databaseManager.ChannelRepository.TableName, attributeName, RelatedIdentities.GetChannelRelatedIdentities(pageInfo.SiteId, node.ChannelId));
                    //parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, ",", pageInfo.Site, ETableStyle.Channel, styleInfo, string.Empty, null, string.Empty, true);

                    var styleInfo = await databaseManager.TableStyleRepository.GetTableStyleAsync(databaseManager.ChannelRepository.TableName, attributeName, databaseManager.TableStyleRepository.GetRelatedIdentities(nodeInfo));
                    if (styleInfo.Id > 0)
                    {
                        parsedContent = GetValue(attributeName, nodeInfo, false, styleInfo.DefaultValue);
                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            if (InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.Image, InputType.File))
                            {
                                parsedContent = await parseManager.PathManager.ParseNavigationUrlAsync(pageInfo.Site, parsedContent, pageInfo.IsLocal);
                            }
                            else
                            {
                                var inputParser = new InputParserManager(parseManager.PathManager);

                                parsedContent = await inputParser.GetContentByTableStyleAsync(parsedContent, null, pageInfo.Config, pageInfo.Site, styleInfo, string.Empty, null, string.Empty, true);
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return parsedContent;
        }

        private static string GetValue(string attributeName, Channel attributes, bool isAddAndNotPostBack, string defaultValue)
        {
            var value = attributes.Get<string>(attributeName);
            if (isAddAndNotPostBack && string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            } 

            return value;
        }
    }
}
