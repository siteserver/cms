using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
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

        public static async Task<object> ParseAsync(ParseContext parseContext)
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

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
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

            var channelId = await parseContext.GetChannelIdByLevelAsync(parseContext.SiteId, parseContext.ChannelId, upLevel, topLevel);

            channelId = await parseContext.ChannelRepository.GetChannelIdAsync(parseContext.SiteId, channelId, channelIndex, channelName);
            var channel = await parseContext.ChannelRepository.GetChannelInfoAsync(parseContext.SiteId, channelId);

            if (parseContext.IsStlEntity && string.IsNullOrEmpty(type))
            {
                return channel.ToDictionary();
            }

            var parsedContent = await ParseImplAsync(parseContext, leftText, rightText, type, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, channel, channelId);

            var innerBuilder = new StringBuilder(parsedContent);
            await parseContext.ParseInnerContentAsync(innerBuilder);
            parsedContent = innerBuilder.ToString();

            if (!StringUtils.EqualsIgnoreCase(type, ChannelAttribute.PageContent))
            {
                parsedContent = parsedContent.Replace(ContentUtility.PagePlaceHolder, string.Empty);
            }

            return parsedContent;
        }

        private static async Task<string> ParseImplAsync(ParseContext parseContext, string leftText, string rightText, string type, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, ChannelInfo channel, int channelId)
        {
            if (string.IsNullOrEmpty(type))
            {
                type = ChannelAttribute.Title;
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
                    formatString = formatString + "}";
                }
            }
            var inputType = InputType.Text;

            if (type.Equals(ChannelAttribute.Id.ToLower()))
            {
                parsedContent = channelId.ToString();
            }
            else if (type.Equals(ChannelAttribute.SiteId.ToLower()))
            {
                parsedContent = channel.SiteId.ToString();
            }
            else if (type.Equals(ChannelAttribute.ContentModelPluginId.ToLower()))
            {
                parsedContent = channel.ContentModelPluginId;
            }
            else if (type.Equals(ChannelAttribute.ContentRelatedPluginIds.ToLower()))
            {
                parsedContent = channel.ContentRelatedPluginIds;
            }
            else if (type.Equals(ChannelAttribute.ParentId.ToLower()))
            {
                parsedContent = channel.ParentId.ToString();
            }
            else if (type.Equals(ChannelAttribute.ParentsPath.ToLower()))
            {
                parsedContent = channel.ParentsPath;
            }
            else if (type.Equals(ChannelAttribute.ParentsCount.ToLower()))
            {
                parsedContent = channel.ParentsCount.ToString();
            }
            else if (type.Equals(ChannelAttribute.ChildrenCount.ToLower()))
            {
                parsedContent = channel.ChildrenCount.ToString();
            }
            else if (type.Equals(ChannelAttribute.IsLastNode.ToLower()))
            {
                parsedContent = channel.IsLastNode.ToString();
            }
            else if (type.Equals(ChannelAttribute.ChannelIndex.ToLower()) || type.Equals(ChannelAttribute.IndexName.ToLower()))
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
            else if (type.Equals(ChannelAttribute.GroupNameCollection.ToLower()))
            {
                parsedContent = channel.GroupNameCollection;
            }
            else if (type.Equals(ChannelAttribute.Taxis.ToLower()))
            {
                parsedContent = channel.Taxis.ToString();
            }
            else if (type.Equals(ChannelAttribute.CreatedDate.ToLower()))
            {
                inputType = InputType.DateTime;
                parsedContent = DateUtils.Format(channel.CreatedDate, formatString);
            }
            else if (type.Equals(ChannelAttribute.ImageUrl.ToLower()))
            {
                inputType = InputType.Image;
                parsedContent = InputParserUtility.GetImageOrFlashHtml(parseContext.UrlManager, parseContext.SiteInfo, channel.ImageUrl, parseContext.Attributes, parseContext.IsStlEntity); // contextInfo.IsStlEntity = true 表示实体标签
            }
            else if (type.Equals(ChannelAttribute.Content.ToLower()))
            {
                parsedContent = parseContext.FileManager.TextEditorContentDecode(parseContext.SiteInfo, channel.Content, parseContext.IsLocal);

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
            else if (type.Equals(ChannelAttribute.FilePath.ToLower()))
            {
                parsedContent = channel.FilePath;
            }
            else if (type.Equals(ChannelAttribute.ChannelFilePathRule.ToLower()))
            {
                parsedContent = channel.ChannelFilePathRule;
            }
            else if (type.Equals(ChannelAttribute.ContentFilePathRule.ToLower()))
            {
                parsedContent = channel.ContentFilePathRule;
            }
            else if (type.Equals(ChannelAttribute.LinkUrl.ToLower()))
            {
                parsedContent = channel.LinkUrl;
            }
            else if (type.Equals(ChannelAttribute.LinkType.ToLower()))
            {
                parsedContent = channel.LinkType;
            }
            else if (type.Equals(ChannelAttribute.ChannelTemplateId.ToLower()))
            {
                parsedContent = channel.ChannelTemplateId.ToString();
            }
            else if (type.Equals(ChannelAttribute.ContentTemplateId.ToLower()))
            {
                parsedContent = channel.ContentTemplateId.ToString();
            }
            else if (type.Equals(ChannelAttribute.Keywords.ToLower()))
            {
                parsedContent = channel.Keywords;
            }
            else if (type.Equals(ChannelAttribute.Description.ToLower()))
            {
                parsedContent = channel.Description;
            }
            else if (type.Equals(ChannelAttribute.ExtendValues.ToLower()))
            {
                parsedContent = channel.ExtendValues;
            }
            else if (type.Equals(ChannelAttribute.Title.ToLower()) || type.Equals(ChannelAttribute.ChannelName.ToLower()))
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
            else if (type.Equals(ChannelAttribute.PageContent.ToLower()))
            {
                if (parseContext.IsInnerElement || parseContext.TemplateInfo.Type != TemplateType.ChannelTemplate)
                {
                    parsedContent = parseContext.FileManager.TextEditorContentDecode(parseContext.SiteInfo, channel.Content, parseContext.IsLocal);

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
                    return parseContext.OuterHtml;
                }
            }
            else if (StringUtils.StartsWithIgnoreCase(type, ChannelAttribute.ItemIndex) && parseContext.Container?.ChannelItem != null)
            {
                var itemIndex = StlParserUtility.ParseItemIndex(parseContext.Container.ChannelItem.Key, type, parseContext);
                parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
            }
            else if (type.Equals(ChannelAttribute.CountOfChannels.ToLower()))
            {
                parsedContent = channel.ChildrenCount.ToString();
            }
            else if (type.Equals(ChannelAttribute.CountOfContents.ToLower()))
            {
                var count = await channel.ContentRepository.GetCountAsync(parseContext.SiteInfo, channel, true);
                parsedContent = count.ToString();
            }
            else if (type.Equals(ChannelAttribute.CountOfImageContents.ToLower()))
            {
                var count = channel.ContentRepository.StlGetCountCheckedImage(parseContext.SiteId, channel);
                parsedContent = count.ToString();
            }
            else
            {
                var attributeName = type;

                var styleInfo = parseContext.TableManager.GetTableStyleInfo(parseContext.ChannelRepository.TableName, attributeName, parseContext.TableManager.GetRelatedIdentities(channel));
                if (styleInfo.Id > 0)
                {
                    parsedContent = channel.Get(attributeName, styleInfo.DefaultValue);
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = InputParserUtility.GetContentByTableStyle(parseContext.FileManager, parseContext.UrlManager, parseContext.SettingsManager, parsedContent, separator, parseContext.SiteInfo, styleInfo, formatString, parseContext.Attributes, parseContext.InnerHtml, false);
                        inputType = styleInfo.Type;
                    }
                }
            }

            if (string.IsNullOrEmpty(parsedContent)) return string.Empty;

            parsedContent = InputTypeUtils.ParseString(inputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
            return leftText + parsedContent + rightText;
        }
    }
}
