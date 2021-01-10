using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "显示导航", Description = "通过 stl:navigation 标签在模板中显示链接导航")]
    public static class StlNavigation
    {
        public const string ElementName = "stl:navigation";

        [StlAttribute(Title = "类型")]
        private const string Type = nameof(Type);
        
        [StlAttribute(Title = "当无内容时显示的信息")]
        private const string EmptyText = nameof(EmptyText);
        
        [StlAttribute(Title = "导航提示信息")]
        private const string TipText = nameof(TipText);
        
        [StlAttribute(Title = "显示字数")]
        private const string WordNum = nameof(WordNum);
        
        [StlAttribute(Title = "是否开启键盘，↑↓←→键分别为上下左右")]
        private const string IsKeyboard = nameof(IsKeyboard);

        public const string TypePreviousChannel = "PreviousChannel";
        public const string TypeNextChannel = "NextChannel";
        public const string TypePreviousContent = "PreviousContent";
        public const string TypeNextContent = "NextContent";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypePreviousChannel, "上一栏目链接"},
            {TypeNextChannel, "下一栏目链接"},
            {TypePreviousContent, "上一内容链接"},
            {TypeNextContent, "下一内容链接"}
        };

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = TypeNextContent;
            var emptyText = string.Empty;
            var tipText = string.Empty;
            var wordNum = 0;
            var isKeyboard = false;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, EmptyText))
                {
                    emptyText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, TipText))
                {
                    tipText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsKeyboard))
                {
                    isKeyboard = TranslateUtils.ToBool(value);
                }
                else
                {
                    TranslateUtils.AddAttributeIfNotExists(attributes, name, value);
                }
            }

            return parseManager.ContextInfo.IsStlEntity
                ? await ParseEntityAsync(type, parseManager)
                : await ParseElementAsync(parseManager, attributes, type, emptyText, tipText, wordNum, isKeyboard);
        }

        private static async Task<string> ParseElementAsync(IParseManager parseManager, NameValueCollection attributes, string type, string emptyText, string tipText, int wordNum, bool isKeyboard)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            string parsedContent;

            var innerHtml = string.Empty;
            StlParserUtility.GetYesNo(contextInfo.InnerHtml, out var successTemplateString, out var failureTemplateString);

            var content = await parseManager.GetContentAsync();

            if (string.IsNullOrEmpty(successTemplateString))
            {
                var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);

                if (StringUtils.EqualsIgnoreCase(type, TypePreviousChannel) || StringUtils.EqualsIgnoreCase(type, TypeNextChannel))
                {
                    var taxis = channel.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    var siblingChannelId = await databaseManager.ChannelRepository.GetIdByParentIdAndTaxisAsync(pageInfo.SiteId, channel.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        var siblingNodeInfo = await databaseManager.ChannelRepository.GetAsync(siblingChannelId);
                        var url = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, siblingNodeInfo, pageInfo.IsLocal);
                        if (url.Equals(PageUtils.UnClickableUrl))
                        {
                            attributes["target"] = string.Empty;
                        }
                        attributes["href"] = url;

                        if (string.IsNullOrEmpty(contextInfo.InnerHtml))
                        {
                            innerHtml = await databaseManager.ChannelRepository.GetChannelNameAsync(pageInfo.SiteId, siblingChannelId);
                            if (wordNum > 0)
                            {
                                innerHtml = StringUtils.MaxLengthText(innerHtml, wordNum);
                            }
                        }
                        else
                        {
                            contextInfo.ChannelId = siblingChannelId;
                            var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                            await parseManager.ParseInnerContentAsync(innerBuilder);
                            innerHtml = innerBuilder.ToString();
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypePreviousContent) || StringUtils.EqualsIgnoreCase(type, TypeNextContent))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var taxis = content.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(type, TypePreviousContent);
                        var tableName = await databaseManager.ChannelRepository.GetTableNameAsync(pageInfo.Site, contextInfo.ChannelId);
                        var siblingContentId = await databaseManager.ContentRepository.GetContentIdAsync(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            var siblingContentInfo = await databaseManager.ContentRepository.GetAsync(pageInfo.Site, contextInfo.ChannelId, siblingContentId);
                            var url = await parseManager.PathManager.GetContentUrlAsync(pageInfo.Site, siblingContentInfo, pageInfo.IsLocal);
                            if (url.Equals(PageUtils.UnClickableUrl))
                            {
                                attributes["target"] = string.Empty;
                            }
                            attributes["href"] = url;

                            if (isKeyboard)
                            {
                                var keyCode = isNextContent ? 39 : 37;
                                var scriptContent = new StringBuilder();
                                await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Jquery);
                                scriptContent.Append($@"<script language=""javascript"" type=""text/javascript""> 
      $(document).keydown(function(event){{
        if(event.keyCode=={keyCode}){{location = '{url}';}}
      }});
</script> 
");
                                var nextOrPrevious = isNextContent ? "nextContent" : "previousContent";

                                pageInfo.BodyCodes[nextOrPrevious] = scriptContent.ToString();
                            }

                            if (string.IsNullOrEmpty(contextInfo.InnerHtml))
                            {
                                innerHtml = siblingContentInfo.Title;
                                if (wordNum > 0)
                                {
                                    innerHtml = StringUtils.MaxLengthText(innerHtml, wordNum);
                                }
                            }
                            else
                            {
                                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                                contextInfo.ContentId = siblingContentId;
                                await parseManager.ParseInnerContentAsync(innerBuilder);
                                innerHtml = innerBuilder.ToString();
                            }
                        }
                    }
                }

                parsedContent = string.IsNullOrEmpty(attributes["href"]) ? emptyText : $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
            }
            else
            {
                var context = parseManager.ContextInfo;

                var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);

                var isSuccess = false;
                parseManager.ContextInfo = contextInfo.Clone();

                if (StringUtils.EqualsIgnoreCase(type, TypePreviousChannel) || StringUtils.EqualsIgnoreCase(type, TypeNextChannel))
                {
                    var taxis = channel.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    //var siblingChannelId = databaseManager.ChannelRepository.GetIdByParentIdAndTaxis(node.ParentId, taxis, isNextChannel);
                    var siblingChannelId = await databaseManager.ChannelRepository.GetIdByParentIdAndTaxisAsync(pageInfo.SiteId, channel.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        isSuccess = true;
                        parseManager.ContextInfo.ContextType = ParseType.Channel;
                        parseManager.ContextInfo.ChannelId = siblingChannelId;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypePreviousContent) || StringUtils.EqualsIgnoreCase(type, TypeNextContent))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var taxis = content.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(type, TypePreviousContent);
                        var tableName = await databaseManager.ChannelRepository.GetTableNameAsync(pageInfo.Site, contextInfo.ChannelId);
                        var siblingContentId = await databaseManager.ContentRepository.GetContentIdAsync(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            isSuccess = true;
                            parseManager.ContextInfo.ContextType = ParseType.Content;
                            parseManager.ContextInfo.ContentId = siblingContentId;
                            parseManager.ContextInfo.SetContent(null);
                        }
                    }
                }

                parsedContent = isSuccess ? successTemplateString : failureTemplateString;

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    var innerBuilder = new StringBuilder(parsedContent);
                    await parseManager.ParseInnerContentAsync(innerBuilder);

                    parsedContent = innerBuilder.ToString();
                }

                parseManager.ContextInfo = context;
            }

            parsedContent = tipText + parsedContent;

            return parsedContent;
        }

        private static async Task<string> ParseEntityAsync(string type, IParseManager parseManager)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var parsedContent = string.Empty;
            try
            {
                var nodeInfo = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);

                if (StringUtils.EqualsIgnoreCase(TypePreviousChannel, type) || StringUtils.EqualsIgnoreCase(TypeNextChannel, type))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    //var siblingChannelId = databaseManager.ChannelRepository.GetIdByParentIdAndTaxis(node.ParentId, taxis, isNextChannel);
                    var siblingChannelId = await databaseManager.ChannelRepository.GetIdByParentIdAndTaxisAsync(pageInfo.SiteId, nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        var siblingNodeInfo = await databaseManager.ChannelRepository.GetAsync(siblingChannelId);
                        parsedContent = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, siblingNodeInfo, pageInfo.IsLocal);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(TypePreviousContent, type) || StringUtils.EqualsIgnoreCase(TypeNextContent, type))
                {
                    if (contextInfo.ContentId != 0)
                    {
                        var contentInfo = await parseManager.GetContentAsync();
                        var taxis = contentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(type, TypePreviousContent);
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
