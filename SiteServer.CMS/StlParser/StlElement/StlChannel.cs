using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.StlElement
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

        public static object Parse(PageInfo pageInfo, ContextInfo contextInfo)
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
            var ellipsis = StringUtils.Constants.Ellipsis;
            var replace = string.Empty;
            var to = string.Empty;
            var isClearTags = false;
            var isReturnToBr = false;
            var isLower = false;
            var isUpper = false;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
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

            var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);

            channelId = ChannelManager.GetChannelId(pageInfo.SiteId, channelId, channelIndex, channelName);
            var channel = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);

            if (contextInfo.IsStlEntity && string.IsNullOrEmpty(type))
            {
                return channel.ToDictionary();
            }

            return ParseImpl(pageInfo, contextInfo, leftText, rightText, type, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, channel, channelId);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string leftText, string rightText, string type, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, ChannelInfo channel, int channelId)
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
            else if (type.Equals(ChannelAttribute.AddDate.ToLower()))
            {
                inputType = InputType.DateTime;
                parsedContent = DateUtils.Format(channel.AddDate, formatString);
            }
            else if (type.Equals(ChannelAttribute.ImageUrl.ToLower()))
            {
                inputType = InputType.Image;
                parsedContent = InputParserUtility.GetImageOrFlashHtml(pageInfo.SiteInfo, channel.ImageUrl, contextInfo.Attributes, contextInfo.IsStlEntity); // contextInfo.IsStlEntity = true 表示实体标签
            }
            else if (type.Equals(ChannelAttribute.Content.ToLower()))
            {
                parsedContent = ContentUtility.TextEditorContentDecode(pageInfo.SiteInfo, channel.Content, pageInfo.IsLocal);

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
                parsedContent = channel.Additional.ToString();
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
                if (contextInfo.IsInnerElement || pageInfo.TemplateInfo.TemplateType != TemplateType.ChannelTemplate)
                {
                    parsedContent = ContentUtility.TextEditorContentDecode(pageInfo.SiteInfo, channel.Content, pageInfo.IsLocal);

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
            else if (StringUtils.StartsWithIgnoreCase(type, ChannelAttribute.ItemIndex) && contextInfo.ItemContainer?.ChannelItem != null)
            {
                var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ChannelItem.ItemIndex, type, contextInfo);
                parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
            }
            else if (type.Equals(ChannelAttribute.CountOfChannels.ToLower()))
            {
                parsedContent = channel.ChildrenCount.ToString();
            }
            else if (type.Equals(ChannelAttribute.CountOfContents.ToLower()))
            {
                var count = ContentManager.GetCount(pageInfo.SiteInfo, channel, true);
                parsedContent = count.ToString();
            }
            else if (type.Equals(ChannelAttribute.CountOfImageContents.ToLower()))
            { 
                var count = StlContentCache.GetCountCheckedImage(pageInfo.SiteId, channel.Id);
                parsedContent = count.ToString();
            }
            else
            {
                var attributeName = type;

                var styleInfo = TableStyleManager.GetTableStyleInfo(DataProvider.ChannelDao.TableName, attributeName, TableStyleManager.GetRelatedIdentities(channel));
                // 如果 styleInfo.TableStyleId <= 0，表示此字段已经被删除了，不需要再显示值了 ekun008
                if (styleInfo.Id > 0)
                {
                    parsedContent = GetValue(attributeName, channel.Additional, false, styleInfo.DefaultValue);
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, separator, pageInfo.SiteInfo, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerHtml, false);
                        inputType = styleInfo.InputType;
                    }
                }
            }

            if (string.IsNullOrEmpty(parsedContent)) return string.Empty;

            parsedContent = InputTypeUtils.ParseString(inputType, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
            return leftText + parsedContent + rightText;
        }

        private static string GetValue(string attributeName, IAttributes attributes, bool isAddAndNotPostBack, string defaultValue)
        {
            var value = attributes.Get(attributeName);
            if (isAddAndNotPostBack && value == null)
            {
                value = defaultValue;
            }
            return value.ToString();
        }
    }
}
