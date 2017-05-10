using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "相册图片", Description = "通过 stl:photo 标签在模板中显示内容的相册图片")]
    public class StlPhoto
	{
        private StlPhoto() { }
        public const string ElementName = "stl:photo";

		public const string AttributeType = "type";
        public const string AttributeLeftText = "leftText";
        public const string AttributeRightText = "rightText";
        public const string AttributeFormatString = "formatString";
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
        public const string AttributeIsDynamic = "isDynamic";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, StringUtils.SortedListToAttributeValueString("类型", TypeList)},
            {AttributeLeftText, "显示在信息前的文字"},
            {AttributeRightText, "显示在信息后的文字"},
            {AttributeFormatString, "显示的格式"},
            {AttributeStartIndex, "字符开始位置"},
            {AttributeLength, "指定字符长度"},
            {AttributeWordNum, "显示字符的数目"},
            {AttributeEllipsis, "文字超出部分显示的文字"},
            {AttributeReplace, "需要替换的文字，可以是正则表达式"},
            {AttributeTo, "替换replace的文字信息"},
            {AttributeIsClearTags, "是否清除标签信息"},
            {AttributeIsReturnToBr, "是否将回车替换为HTML换行标签"},
            {AttributeIsLower, "是否转换为小写"},
            {AttributeIsUpper, "是否转换为大写"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public const string TypeItemIndex = "ItemIndex";
        public const string TypeSmallUrl = "SmallUrl";
        public const string TypeMiddleUrl = "MiddleUrl";
        public const string TypeLargeUrl = "LargeUrl";
        public const string TypeDescription = "Description";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeItemIndex, "项次序"},
            {TypeSmallUrl, "小尺寸图"},
            {TypeMiddleUrl, "中尺寸图"},
            {TypeLargeUrl, "大尺寸图"},
            {TypeDescription, "图片简介"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
                var leftText = string.Empty;
                var rightText = string.Empty;
                var formatString = string.Empty;
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
                var type = string.Empty;
                var isDynamic = false;
                var attributes = new StringDictionary();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();

                        if (attributeName.Equals(AttributeType))
                        {
                            type = attr.Value.ToLower();
                        }
                        else if (attributeName.Equals(AttributeLeftText))
                        {
                            leftText = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeRightText))
                        {
                            rightText = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeFormatString))
                        {
                            formatString = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeStartIndex))
                        {
                            startIndex = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeLength))
                        {
                            length = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeWordNum))
                        {
                            wordNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeEllipsis))
                        {
                            ellipsis = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeReplace))
                        {
                            replace = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeTo))
                        {
                            to = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeIsClearTags))
                        {
                            isClearTags = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (attributeName.Equals(AttributeIsReturnToBr))
                        {
                            isReturnToBr = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (attributeName.Equals(AttributeIsLower))
                        {
                            isLower = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (attributeName.Equals(AttributeIsUpper))
                        {
                            isUpper = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (attributeName.Equals(AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            attributes.Add(attributeName, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, attributes, leftText, rightText, formatString, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, type);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, StringDictionary attributes, string leftText, string rightText, string formatString, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string type)
        {
            var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(type) && contextInfo.ItemContainer?.PhotoItem != null)
            {
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
                else
                {
                    formatString = "{0}";
                }

                if (string.IsNullOrEmpty(type) || StringUtils.EqualsIgnoreCase(type, "imageUrl"))
                {
                    type = TypeLargeUrl;
                }

                if (StringUtils.StartsWithIgnoreCase(type, TypeItemIndex))
                {
                    var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.PhotoItem.ItemIndex, type, contextInfo);

                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
                }
                else if (StringUtils.StartsWithIgnoreCase(type, TypeSmallUrl) || StringUtils.StartsWithIgnoreCase(type, TypeMiddleUrl) || StringUtils.StartsWithIgnoreCase(type, TypeLargeUrl))
                {
                    parsedContent = DataBinder.Eval(contextInfo.ItemContainer.PhotoItem.DataItem, type, formatString);

                    parsedContent = InputParserUtility.GetImageOrFlashHtml(pageInfo.PublishmentSystemInfo, parsedContent, attributes, false);
                }
                else if (StringUtils.StartsWithIgnoreCase(type, TypeDescription) || StringUtils.StartsWithIgnoreCase(type, "content"))
                {
                    parsedContent = DataBinder.Eval(contextInfo.ItemContainer.PhotoItem.DataItem, TypeDescription, formatString);
                    parsedContent = StringUtils.ReplaceNewlineToBr(parsedContent);
                }
                else
                {
                    parsedContent = DataBinder.Eval(contextInfo.ItemContainer.PhotoItem.DataItem, type, formatString);
                }
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = StringUtils.ParseString(parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    parsedContent = leftText + parsedContent + rightText;
                }
            }

            return parsedContent;
        }
	}
}
