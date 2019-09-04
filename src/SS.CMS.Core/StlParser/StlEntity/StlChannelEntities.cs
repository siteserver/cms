using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Enums;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlEntity
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

        internal static async Task<string> ParseAsync(string stlEntity, ParseContext parseContext)
        {
            var parsedContent = string.Empty;

            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var channelIndex = StlParserUtility.GetValueFromEntity(stlEntity);
                var attributeName = entityName.Substring(9, entityName.Length - 10);

                var upLevel = 0;
                var topLevel = -1;
                var channelId = parseContext.ChannelId;
                if (!string.IsNullOrEmpty(channelIndex))
                {
                    //channelId = DataProvider.ChannelDao.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                    channelId = await parseContext.ChannelRepository.GetIdByIndexNameAsync(parseContext.SiteId, channelIndex);
                    if (channelId == 0)
                    {
                        channelId = parseContext.ChannelId;
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

                var nodeInfo = await parseContext.ChannelRepository.GetChannelAsync(await parseContext.GetChannelIdByLevelAsync(parseContext.SiteId, channelId, upLevel, topLevel));

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
                    parsedContent = parseContext.FileManager.TextEditorContentDecode(parseContext.SiteInfo, nodeInfo.Content, parseContext.IsLocal);
                }
                else if (StringUtils.EqualsIgnoreCase(NavigationUrl, attributeName))//栏目链接地址
                {
                    parsedContent = await parseContext.UrlManager.GetChannelUrlAsync(parseContext.SiteInfo, nodeInfo, parseContext.IsLocal);
                }
                else if (StringUtils.EqualsIgnoreCase(ImageUrl, attributeName))//栏目图片地址
                {
                    parsedContent = nodeInfo.ImageUrl;

                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, parsedContent, parseContext.IsLocal);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(AddDate, attributeName))//栏目添加日期
                {
                    parsedContent = DateUtils.Format(nodeInfo.CreatedDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(DirectoryName, attributeName))//生成文件夹名称
                {
                    parsedContent = PathUtils.GetDirectoryName(nodeInfo.FilePath, true);
                }
                else if (StringUtils.EqualsIgnoreCase(Group, attributeName))//栏目组别
                {
                    parsedContent = nodeInfo.GroupNameCollection;
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) && parseContext.Container?.ChannelItem != null)
                {
                    parsedContent = StlParserUtility.ParseItemIndex(parseContext.Container.ChannelItem.Key, attributeName, parseContext).ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.Keywords, attributeName))//栏目组别
                {
                    parsedContent = nodeInfo.Keywords;
                }
                else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.Description, attributeName))//栏目组别
                {
                    parsedContent = nodeInfo.Description;
                }
                else
                {
                    //var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.Channel, DataProvider.ChannelDao.TableName, attributeName, RelatedIdentities.GetChannelRelatedIdentities(pageInfo.SiteId, nodeInfo.ChannelId));
                    //parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, ",", pageInfo.SiteInfo, ETableStyle.Channel, styleInfo, string.Empty, null, string.Empty, true);

                    var styleInfo = await parseContext.TableStyleRepository.GetTableStyleInfoAsync(parseContext.ChannelRepository.TableName, attributeName, parseContext.TableStyleRepository.GetRelatedIdentities(nodeInfo));
                    if (styleInfo.Id > 0)
                    {
                        parsedContent = nodeInfo.Get(attributeName, styleInfo.DefaultValue);
                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            if (InputTypeUtils.EqualsAny(styleInfo.Type, InputType.Image, InputType.File))
                            {
                                parsedContent = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, parsedContent, parseContext.IsLocal);
                            }
                            else
                            {
                                parsedContent = InputParserUtility.GetContentByTableStyle(parseContext.FileManager, parseContext.UrlManager, parseContext.SettingsManager, parsedContent, null, parseContext.SiteInfo, styleInfo, string.Empty, null, string.Empty, true);
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
    }
}
