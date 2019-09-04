using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "显示导航", Description = "通过 stl:navigation 标签在模板中显示链接导航")]
    public class StlNavigation
    {
        private StlNavigation() { }
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

        public static async Task<object> ParseAsync(ParseContext parseContext)
        {
            var attributes = new NameValueCollection();
            var type = TypeNextContent;
            var emptyText = string.Empty;
            var tipText = string.Empty;
            var wordNum = 0;
            var isKeyboard = false;

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

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

            return await ParseImplAsync(parseContext, attributes, type, emptyText, tipText, wordNum, isKeyboard);
        }

        private static async Task<string> ParseImplAsync(ParseContext parseContext, NameValueCollection attributes, string type, string emptyText, string tipText, int wordNum, bool isKeyboard)
        {
            string parsedContent;

            var innerHtml = string.Empty;
            string successTemplateString;
            string failureTemplateString;
            StlParserUtility.GetYesNo(parseContext.InnerHtml, out successTemplateString, out failureTemplateString);

            if (string.IsNullOrEmpty(successTemplateString))
            {
                var nodeInfo = await parseContext.ChannelRepository.GetChannelAsync(parseContext.ChannelId);

                if (type.ToLower().Equals(TypePreviousChannel.ToLower()) || type.ToLower().Equals(TypeNextChannel.ToLower()))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    //var siblingChannelId = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    var siblingChannelId = await parseContext.ChannelRepository.GetIdByParentIdAndTaxisAsync(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        var siblingNodeInfo = await parseContext.ChannelRepository.GetChannelAsync(siblingChannelId);
                        var url = await parseContext.UrlManager.GetChannelUrlAsync(parseContext.SiteInfo, siblingNodeInfo, parseContext.IsLocal);
                        if (url.Equals(PageUtils.UnClickableUrl))
                        {
                            attributes["target"] = string.Empty;
                        }
                        attributes["href"] = url;

                        if (string.IsNullOrEmpty(parseContext.InnerHtml))
                        {
                            innerHtml = await parseContext.ChannelRepository.GetChannelNameAsync(siblingChannelId);
                            if (wordNum > 0)
                            {
                                innerHtml = StringUtils.MaxLengthText(innerHtml, wordNum);
                            }
                        }
                        else
                        {
                            parseContext.ChannelId = siblingChannelId;
                            var innerBuilder = new StringBuilder(parseContext.InnerHtml);
                            await parseContext.ParseInnerContentAsync(innerBuilder);
                            innerHtml = innerBuilder.ToString();
                        }
                    }
                }
                else if (type.ToLower().Equals(TypePreviousContent.ToLower()) || type.ToLower().Equals(TypeNextContent.ToLower()))
                {
                    var channelInfo = await parseContext.GetChannelAsync();
                    var contentRepository = parseContext.ChannelRepository.GetContentRepository(parseContext.SiteInfo, channelInfo);

                    var contentInfo = await parseContext.GetContentInfoAsync();
                    if (contentInfo != null)
                    {
                        var taxis = contentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(type, TypePreviousContent);
                        //var siblingContentId = DataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        var siblingContentId = await contentRepository.GetContentIdAsync(channelInfo.Id, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            //var siblingContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentId);
                            var siblingContentInfo = await contentRepository.GetContentInfoAsync(siblingContentId);
                            var url = await parseContext.UrlManager.GetContentUrlAsync(parseContext.SiteInfo, siblingContentInfo, parseContext.IsLocal);
                            if (url.Equals(PageUtils.UnClickableUrl))
                            {
                                attributes["target"] = string.Empty;
                            }
                            attributes["href"] = url;

                            if (isKeyboard)
                            {
                                var keyCode = isNextContent ? 39 : 37;
                                var scriptContent = new StringBuilder();
                                parseContext.PageInfo.AddPageBodyCodeIfNotExists(parseContext.UrlManager, PageInfo.Const.Jquery);
                                scriptContent.Append($@"<script language=""javascript"" type=""text/javascript""> 
      $(document).keydown(function(event){{
        if(event.keyCode=={keyCode}){{location = '{url}';}}
      }});
</script> 
");
                                var nextOrPrevious = isNextContent ? "nextContent" : "previousContent";

                                parseContext.BodyCodes[nextOrPrevious] = scriptContent.ToString();
                            }

                            if (string.IsNullOrEmpty(parseContext.InnerHtml))
                            {
                                innerHtml = siblingContentInfo.Title;
                                if (wordNum > 0)
                                {
                                    innerHtml = StringUtils.MaxLengthText(innerHtml, wordNum);
                                }
                            }
                            else
                            {
                                var innerBuilder = new StringBuilder(parseContext.InnerHtml);
                                parseContext.ContentId = siblingContentId;
                                await parseContext.ParseInnerContentAsync(innerBuilder);
                                innerHtml = innerBuilder.ToString();
                            }
                        }
                    }
                }

                parsedContent = string.IsNullOrEmpty(attributes["href"]) ? emptyText : $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
            }
            else
            {
                var nodeInfo = await parseContext.ChannelRepository.GetChannelAsync(parseContext.ChannelId);

                var isSuccess = false;
                var context = parseContext.Clone();

                if (type.ToLower().Equals(TypePreviousChannel.ToLower()) || type.ToLower().Equals(TypeNextChannel.ToLower()))
                {
                    var taxis = nodeInfo.Taxis;
                    var isNextChannel = !StringUtils.EqualsIgnoreCase(type, TypePreviousChannel);
                    //var siblingChannelId = DataProvider.ChannelDao.GetIdByParentIdAndTaxis(nodeInfo.ParentId, taxis, isNextChannel);
                    var siblingChannelId = await parseContext.ChannelRepository.GetIdByParentIdAndTaxisAsync(nodeInfo.ParentId, taxis, isNextChannel);
                    if (siblingChannelId != 0)
                    {
                        isSuccess = true;
                        context.ContextType = EContextType.Channel;
                        context.ChannelId = siblingChannelId;
                    }
                }
                else if (type.ToLower().Equals(TypePreviousContent.ToLower()) || type.ToLower().Equals(TypeNextContent.ToLower()))
                {
                    var channelInfo = await parseContext.GetChannelAsync();
                    var contentRepository = parseContext.ChannelRepository.GetContentRepository(parseContext.SiteInfo, channelInfo);

                    var contentInfo = await parseContext.GetContentInfoAsync();
                    if (contentInfo != null)
                    {
                        var taxis = contentInfo.Taxis;
                        var isNextContent = !StringUtils.EqualsIgnoreCase(type, TypePreviousContent);
                        //var siblingContentId = DataProvider.ContentDao.GetContentId(tableName, contextInfo.ChannelId, taxis, isNextContent);
                        var siblingContentId = await contentRepository.GetContentIdAsync(channelInfo.Id, taxis, isNextContent);
                        if (siblingContentId != 0)
                        {
                            isSuccess = true;
                            context.ContextType = EContextType.Content;
                            context.ContentId = siblingContentId;
                            context.ContentInfo = null;
                        }
                    }
                }

                parsedContent = isSuccess ? successTemplateString : failureTemplateString;

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    var innerBuilder = new StringBuilder(parsedContent);
                    await context.ParseInnerContentAsync(innerBuilder);

                    parsedContent = innerBuilder.ToString();
                }
            }

            parsedContent = tipText + parsedContent;

            return parsedContent;
        }
    }
}
