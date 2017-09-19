using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "获取栏目值", Description = "通过 stl:channel 标签在模板中显示指定栏目的属性值")]
    public class StlChannel
    {
        private StlChannel() { }
        public const string ElementName = "stl:channel";

        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeParent = "parent";
        public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeType = "type";
        public const string AttributeLeftText = "leftText";
        public const string AttributeRightText = "rightText";
        public const string AttributeFormatString = "formatString";
        public const string AttributeSeparator = "separator";
        public const string AttributeStartIndex = "startIndex";
        public const string AttributeLength = "length";
        public const string AttributeWordNum = "wordNum";
        public const string AttributeEllipsis = "ellipsis";
        public const string AttributeReplace = "replace";
        public const string AttributeTo = "to";
        public const string AttributeIsClearTags = "isClearTags";
        public const string AttributeIsReturnToBr = "isReturnToBr";
        public const string AttributeIsLower = "isLower";
        public const string AttributeIsUpper = "isUpper";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeParent, "显示父栏目属性"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeType, "显示的类型"},
            {AttributeLeftText, "显示在信息前的文字"},
            {AttributeRightText, "显示在信息后的文字"},
            {AttributeFormatString, "显示的格式"},
            {AttributeSeparator, "显示多项时的分割字符串"},
            {AttributeStartIndex, "字符开始位置"},
            {AttributeLength, "指定字符长度"},
            {AttributeWordNum, "显示字符的数目"},
            {AttributeEllipsis, "文字超出部分显示的文字"},
            {AttributeReplace, "需要替换的文字，可以是正则表达式"},
            {AttributeTo, "替换replace的文字信息"},
            {AttributeIsClearTags, "是否清除HTML标签"},
            {AttributeIsReturnToBr, "是否将回车替换为HTML换行标签"},
            {AttributeIsLower, "是否转换为小写"},
            {AttributeIsUpper, "是否转换为大写"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var leftText = string.Empty;
            var rightText = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var type = NodeAttribute.Title;
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

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeParent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeUpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeLeftText))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeRightText))
                {
                    rightText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeFormatString))
                {
                    formatString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSeparator))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeStartIndex))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeLength))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeEllipsis))
                {
                    ellipsis = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeReplace))
                {
                    replace = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTo))
                {
                    to = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsClearTags))
                {
                    isClearTags = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsReturnToBr))
                {
                    isReturnToBr = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsLower))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsUpper))
                {
                    isUpper = TranslateUtils.ToBool(value, true);
                }
            }

            return ParseImpl(pageInfo, contextInfo, leftText, rightText, channelIndex, channelName, upLevel, topLevel, type, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string leftText, string rightText, string channelIndex, string channelName, int upLevel, int topLevel, string type, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper)
        {
            var parsedContent = string.Empty;

            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);

            channelId = StlDataUtility.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);
            var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

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

            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLower().Equals(NodeAttribute.Title.ToLower()))
                {
                    parsedContent = channel.NodeName;

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
                else if (type.ToLower().Equals(NodeAttribute.ChannelIndex.ToLower()))
                {
                    parsedContent = channel.NodeIndexName;

                    if (!string.IsNullOrEmpty(replace))
                    {
                        parsedContent = StringUtils.Replace(replace, parsedContent, to);
                    }

                    if (!string.IsNullOrEmpty(parsedContent) && wordNum > 0)
                    {
                        parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                    }
                }
                else if (type.ToLower().Equals(NodeAttribute.Content.ToLower()))
                {
                    parsedContent = StringUtility.TextEditorContentDecode(channel.Content, pageInfo.PublishmentSystemInfo);

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
                else if (type.ToLower().Equals(NodeAttribute.PageContent.ToLower()))
                {
                    if (contextInfo.IsInnerElement || pageInfo.TemplateInfo.TemplateType != ETemplateType.ChannelTemplate)
                    {
                        parsedContent = StringUtility.TextEditorContentDecode(channel.Content, pageInfo.PublishmentSystemInfo);

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
                        return contextInfo.StlElement;
                    }
                }
                else if (type.ToLower().Equals(NodeAttribute.AddDate.ToLower()))
                {
                    parsedContent = DateUtils.Format(channel.AddDate, formatString);
                }
                else if (type.ToLower().Equals(NodeAttribute.ImageUrl.ToLower()))
                { 
                    parsedContent = InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo, channel.ImageUrl, contextInfo.Attributes, contextInfo.IsCurlyBrace); // contextInfo.IsCurlyBrace = true 表示实体标签
                }
                else if (type.ToLower().Equals(NodeAttribute.Id.ToLower()))
                {
                    parsedContent = channelId.ToString();
                }
                else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex) && contextInfo.ItemContainer?.ChannelItem != null)
                {
                    var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.ChannelItem.ItemIndex, type, contextInfo);
                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
                }
                else if (type.ToLower().Equals(NodeAttribute.CountOfChannels.ToLower()))
                {
                    parsedContent = channel.ChildrenCount.ToString();
                }
                else if (type.ToLower().Equals(NodeAttribute.CountOfContents.ToLower()))
                {
                    parsedContent = channel.ContentNum.ToString();
                }
                else if (type.ToLower().Equals(NodeAttribute.CountOfImageContents.ToLower()))
                { 
                    var count = Content.GetCountCheckedImage(pageInfo.PublishmentSystemId, channel.NodeId);
                    parsedContent = count.ToString();
                }
                else if (type.ToLower().Equals(NodeAttribute.Keywords.ToLower()))
                {
                    parsedContent = channel.Keywords;
                }
                else if (type.ToLower().Equals(NodeAttribute.Description.ToLower()))
                {
                    parsedContent = channel.Description;
                }
                else
                {
                    var attributeName = type;

                    var formCollection = channel.Additional.GetExtendedAttributes();
                    if (formCollection != null && formCollection.Count > 0)
                    {
                        var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.Channel, DataProvider.NodeDao.TableName, attributeName, RelatedIdentities.GetChannelRelatedIdentities(pageInfo.PublishmentSystemId, channel.NodeId));
                        // 如果 styleInfo.TableStyleId <= 0，表示此字段已经被删除了，不需要再显示值了 ekun008
                        if (styleInfo.TableStyleId > 0 && styleInfo.IsVisible)
                        {
                            parsedContent = GetValue(attributeName, formCollection, false, styleInfo.DefaultValue);
                            if (!string.IsNullOrEmpty(parsedContent))
                            {
                                parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, separator, pageInfo.PublishmentSystemInfo, ETableStyle.Channel, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerXml, false);
                                parsedContent = StringUtils.ParseString(InputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    parsedContent = leftText + parsedContent + rightText;
                }
            }

            return parsedContent;
        }

        private static string GetValue(string attributeName, NameValueCollection formCollection, bool isAddAndNotPostBack, string defaultValue)
        {
            var value = string.Empty;
            if (formCollection?[attributeName] != null)
            {
                value = formCollection[attributeName];
            }
            if (isAddAndNotPostBack && string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }
            return value;
        }
    }
}
