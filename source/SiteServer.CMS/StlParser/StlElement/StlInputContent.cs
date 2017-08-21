using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "获取提交表单值", Description = "通过 stl:inputContent 标签在模板中显示指定表单的提交值")]
    public class StlInputContent
	{
        private StlInputContent() { }
        public const string ElementName = "stl:inputContent";

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
	        {AttributeIsClearTags, "是否清除标签信息"},
	        {AttributeIsReturnToBr, "是否将回车替换为HTML换行标签"},
	        {AttributeIsLower, "是否转换为小写"},
	        {AttributeIsUpper, "是否转换为大写"}
	    };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    if (contextInfo.ItemContainer?.InputItem == null) return string.Empty;

            var leftText = string.Empty;
            var rightText = string.Empty;
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
            var type = string.Empty;

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value.ToLower();
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

            return ParseImpl(pageInfo, contextInfo, leftText, rightText, formatString, separator, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, type);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string leftText, string rightText, string formatString, string separator, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string type)
        {
            var parsedContent = string.Empty;

            if (InputContentAttribute.Id.ToLower().Equals(type))
            {
                parsedContent = SqlUtils.EvalString(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.Id);
            }
            else if (InputContentAttribute.AddDate.ToLower().Equals(type))
            {
                var addDate = SqlUtils.EvalDateTime(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.AddDate);
                parsedContent = DateUtils.Format(addDate, formatString);
            }
            else if (InputContentAttribute.UserName.ToLower().Equals(type))
            {
                parsedContent = SqlUtils.EvalString(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.UserName);
            }
            else if (InputContentAttribute.Reply.ToLower().Equals(type))
            {
                var content = SqlUtils.EvalString(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.Reply);
                parsedContent = content;
                parsedContent = StringUtils.ParseString(InputType.TextEditor, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
            }
            else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex))
            {
                parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.InputItem.ItemIndex, type, contextInfo).ToString();
            }
            else
            {
                var id = SqlUtils.EvalInt(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.Id);
                //var inputContentInfo = DataProvider.InputContentDao.GetContentInfo(id);
                var inputContentInfo = InputContent.GetContentInfo(id);
                if (inputContentInfo != null)
                {
                    parsedContent = inputContentInfo.GetExtendedAttribute(type);
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        if (!InputContentAttribute.HiddenAttributes.Contains(type.ToLower()))
                        {
                            var styleInfo = TableStyleManager.GetTableStyleInfo(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, type, RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, pageInfo.PublishmentSystemId, inputContentInfo.InputId));
                            parsedContent = InputParserUtility.GetContentByTableStyle(parsedContent, separator, pageInfo.PublishmentSystemInfo, ETableStyle.InputContent, styleInfo, formatString, contextInfo.Attributes, contextInfo.InnerXml, false);
                            parsedContent = StringUtils.ParseString(InputTypeUtils.GetEnumType(styleInfo.InputType), parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = leftText + parsedContent + rightText;
            }

            return parsedContent;
        }
	}
}
