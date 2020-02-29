using System.Text;
using SS.CMS.Abstractions;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SS.CMS.Core;

namespace SS.CMS.StlParser.StlElement
{
    [StlElement(Title = "获取栏目值", Description = "通过 stl:channel 标签在模板中显示指定栏目的属性值")]
    public class StlChannel
    {
        private StlChannel() { }
        public const string ElementName = "stl:channel";

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "显示父栏目属性")]
        private const string Parent = nameof(Parent);

        [StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "显示的类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "显示在信息前的文字")]
        private const string LeftText = nameof(LeftText);

        [StlAttribute(Title = "显示在信息后的文字")]
        private const string RightText = nameof(RightText);

        [StlAttribute(Title = "显示的格式")]
        private const string FormatString = nameof(FormatString);

        [StlAttribute(Title = "显示多项时的分割字符串")]
        private const string Separator = nameof(Separator);

        [StlAttribute(Title = "字符开始位置")]
        private const string StartIndex = nameof(StartIndex);

        [StlAttribute(Title = "指定字符长度")]
        private const string Length = nameof(Length);

        [StlAttribute(Title = "显示字符的数目")]
        private const string WordNum = nameof(WordNum);

        [StlAttribute(Title = "文字超出部分显示的文字")]
        private const string Ellipsis = nameof(Ellipsis);

        [StlAttribute(Title = "需要替换的文字，可以是正则表达式")]
        private const string Replace = nameof(Replace);

        [StlAttribute(Title = "替换replace的文字信息")]
        private const string To = nameof(To);

        [StlAttribute(Title = "是否清除HTML标签")]
        private const string IsClearTags = nameof(IsClearTags);

        [StlAttribute(Title = "是否将回车替换为HTML换行标签")]
        private const string IsReturnToBr = nameof(IsReturnToBr);

        [StlAttribute(Title = "是否转换为小写")]
        private const string IsLower = nameof(IsLower);

        [StlAttribute(Title = "是否转换为大写")]
        private const string IsUpper = nameof(IsUpper);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var leftText = string.Empty;
            var rightText = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var type = string.Empty;
            var formatString = string.Empty;
            string separator = null;
            var startIndex = 0;
            var length = 0;
            var wordNum = 0;
            var ellipsis = Constants.Ellipsis;
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isReturnToBr = false;
            var isLower = false;
            var isUpper = false;

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LeftText))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, RightText))
                {
                    rightText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, FormatString))
                {
                    formatString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Separator))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StartIndex))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Length))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Ellipsis))
                {
                    ellipsis = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Replace))
                {
                    replace = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, To))
                {
                    to = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsClearTags))
                {
                    isClearTags = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsReturnToBr))
                {
                    isReturnToBr = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLower))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
            }

            var dataManager = new StlDataManager(parseManager.DatabaseManager);
            var channelId = await dataManager.GetChannelIdByLevelAsync(parseManager.PageInfo.SiteId, parseManager.ContextInfo.ChannelId, upLevel, topLevel);

            channelId = await parseManager.DatabaseManager.ChannelRepository.GetChannelIdAsync(parseManager.PageInfo.SiteId, channelId, channelIndex, channelName);
            var channel = await parseManager.DatabaseManager.ChannelRepository.GetAsync(channelId);

            if (parseManager.ContextInfo.IsStlEntity && string.IsNullOrEmpty(type))
            {
                return channel.ToDictionary();
            }

            var parsedContent = await ParseImplAsync(parseManager, leftText, rightText, type, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, channel, channelId);

            var innerBuilder = new StringBuilder(parsedContent);
            await parseManager.ParseInnerContentAsync(innerBuilder);
            parsedContent = innerBuilder.ToString();

            if (!StringUtils.EqualsIgnoreCase(type, StlParserUtility.PageContent))
            {
                parsedContent = parsedContent.Replace(Constants.PagePlaceHolder, string.Empty);
            }

            return parsedContent;
        }

        private static async Task<string> ParseImplAsync(IParseManager parseManager, string leftText, string rightText, string type, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, Channel channel, int channelId)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (string.IsNullOrEmpty(type))
            {
                type = nameof(StlParserUtility.Title);
            }
            type = type.ToLower();

            var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(formatString))
            {
                formatString = formatString.Trim();
                if (!formatString.StartsWith("{0"))
                {
                    formatString = "{0:" + formatString;
                }
                if (!formatString.EndsWith("}"))
                {
                    formatString += "}";
                }
            }
            var inputType = InputType.Text;

            if (type.Equals(nameof(Channel.Id).ToLower()))
            {
                parsedContent = channelId.ToString();
            }
            else if (type.Equals(nameof(Channel.SiteId).ToLower()))
            {
                parsedContent = channel.SiteId.ToString();
            }
            else if (type.Equals(nameof(Channel.ContentModelPluginId).ToLower()))
            {
                parsedContent = channel.ContentModelPluginId;
            }
            else if (type.Equals(nameof(Channel.ContentRelatedPluginIds).ToLower()))
            {
                parsedContent = Utilities.ToString(channel.ContentRelatedPluginIds);
            }
            else if (type.Equals(nameof(Channel.ParentId).ToLower()))
            {
                parsedContent = channel.ParentId.ToString();
            }
            else if (type.Equals(nameof(Channel.ParentsPath).ToLower()))
            {
                parsedContent = channel.ParentsPath;
            }
            else if (type.Equals(nameof(Channel.ParentsCount).ToLower()))
            {
                parsedContent = channel.ParentsCount.ToString();
            }
            else if (type.Equals(nameof(Channel.ChildrenCount).ToLower()))
            {
                parsedContent = channel.ChildrenCount.ToString();
            }
            else if (type.Equals(nameof(Channel.IndexName).ToLower()) || type.Equals(nameof(Channel.IndexName).ToLower()))
            {
                parsedContent = channel.IndexName;

                if (!string.IsNullOrEmpty(replace))
                {
                    parsedContent = StringUtils.Replace(replace, parsedContent, to);
                }

                if (!string.IsNullOrEmpty(parsedContent) && wordNum > 0)
                {
                    parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                }
            }
            else if (type.Equals(nameof(Channel.GroupNames).ToLower()))
            {
                parsedContent = Utilities.ToString(channel.GroupNames);
            }
            else if (type.Equals(nameof(Channel.Taxis).ToLower()))
            {
                parsedContent = channel.Taxis.ToString();
            }
            else if (type.Equals(nameof(Channel.AddDate).ToLower()))
            {
                inputType = InputType.DateTime;
                parsedContent = DateUtils.Format(channel.AddDate, formatString);
            }
            else if (type.Equals(nameof(Channel.ImageUrl).ToLower()))
            {
                inputType = InputType.Image;
                var inputParser = new InputParserManager(parseManager.PathManager);
                parsedContent = await inputParser.GetImageOrFlashHtmlAsync(pageInfo.Site, channel.ImageUrl, contextInfo.Attributes, contextInfo.IsStlEntity); // contextInfo.IsStlEntity = true 表示实体标签
            }
            else if (type.Equals(nameof(Channel.Content).ToLower()))
            {
                parsedContent = await ContentUtility.TextEditorContentDecodeAsync(parseManager.PathManager, pageInfo.Site, channel.Content, pageInfo.IsLocal);

                if (isClearTags)
                {
                    parsedContent = StringUtils.StripTags(parsedContent);
                }

                if (!string.IsNullOrEmpty(replace))
                {
                    parsedContent = StringUtils.Replace(replace, parsedContent, to);
                }

                if (!string.IsNullOrEmpty(parsedContent) && wordNum > 0)
                {
                    parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                }
            }
            else if (type.Equals(nameof(Channel.FilePath).ToLower()))
            {
                parsedContent = channel.FilePath;
            }
            else if (type.Equals(nameof(Channel.ChannelFilePathRule).ToLower()))
            {
                parsedContent = channel.ChannelFilePathRule;
            }
            else if (type.Equals(nameof(Channel.ContentFilePathRule).ToLower()))
            {
                parsedContent = channel.ContentFilePathRule;
            }
            else if (type.Equals(nameof(Channel.LinkUrl).ToLower()))
            {
                parsedContent = channel.LinkUrl;
            }
            else if (type.Equals(nameof(Channel.LinkType).ToLower()))
            {
                parsedContent = channel.LinkType.GetValue();
            }
            else if (type.Equals(nameof(Channel.ChannelTemplateId).ToLower()))
            {
                parsedContent = channel.ChannelTemplateId.ToString();
            }
            else if (type.Equals(nameof(Channel.ContentTemplateId).ToLower()))
            {
                parsedContent = channel.ContentTemplateId.ToString();
            }
            else if (type.Equals(nameof(Channel.Keywords).ToLower()))
            {
                parsedContent = channel.Keywords;
            }
            else if (type.Equals(nameof(Channel.Description).ToLower()))
            {
                parsedContent = channel.Description;
            }
            else if (type.Equals(StlParserUtility.Title.ToLower()) || type.Equals(nameof(Channel.ChannelName).ToLower()))
            {
                parsedContent = channel.ChannelName;

                if (isClearTags)
                {
                    parsedContent = StringUtils.StripTags(parsedContent);
                }

                if (!string.IsNullOrEmpty(replace))
                {
                    parsedContent = StringUtils.Replace(replace, parsedContent, to);
                }

                if (!string.IsNullOrEmpty(parsedContent) && wordNum > 0)
                {
                    parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                }
            }
            else if (type.Equals(StlParserUtility.PageContent.ToLower()))
            {
                if (contextInfo.IsInnerElement || pageInfo.Template.TemplateType != TemplateType.ChannelTemplate)
                {
                    parsedContent = await ContentUtility.TextEditorContentDecodeAsync(parseManager.PathManager, pageInfo.Site, channel.Content, pageInfo.IsLocal);

                    if (isClearTags)
                    {
                        parsedContent = StringUtils.StripTags(parsedContent);
                    }

                    if (!string.IsNullOrEmpty(replace))
                    {
                        parsedContent = StringUtils.Replace(replace, parsedContent, to);
                    }

                    if (!string.IsNullOrEmpty(parsedContent) && wordNum > 0)
                    {
                        parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                    }
                }
                else
                {
                    return contextInfo.OuterHtml;
                }
            }
            else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ChannelItem != null)
            {
                var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ChannelItem.Key, type, contextInfo);
                parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
            }
            else if (type.Equals(StlParserUtility.CountOfChannels.ToLower()))
            {
                parsedContent = channel.ChildrenCount.ToString();
            }
            else if (type.Equals(StlParserUtility.CountOfContents.ToLower()))
            {
                var count = await databaseManager.ContentRepository.GetCountAsync(pageInfo.Site, channel);
                parsedContent = count.ToString();
            }
            else if (type.Equals(StlParserUtility.CountOfImageContents.ToLower()))
            { 
                var count = await databaseManager.ContentRepository.GetCountCheckedImageAsync(pageInfo.Site, channel);
                parsedContent = count.ToString();
            }
            else
            {
                var attributeName = type;

                var styleInfo = await databaseManager.TableStyleRepository.GetTableStyleAsync(databaseManager.ChannelRepository.TableName, attributeName, databaseManager.TableStyleRepository.GetRelatedIdentities(channel));
                if (styleInfo.Id > 0)
                {
                    parsedContent = GetValue(attributeName, channel, false, styleInfo.DefaultValue);
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        var inputParser = new InputParserManager(parseManager.PathManager);
                        parsedContent = await inputParser.GetContentByTableStyleAsync(parsedContent, separator, pageInfo.Config, pageInfo.Site, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerHtml, false);
                        inputType = styleInfo.InputType;
                    }
                }
            }

            if (string.IsNullOrEmpty(parsedContent)) return string.Empty;

            parsedContent = InputTypeUtils.ParseString(inputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
            return leftText + parsedContent + rightText;
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
