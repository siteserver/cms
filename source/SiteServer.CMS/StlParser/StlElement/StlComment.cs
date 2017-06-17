using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "获取评论值", Description = "通过 stl:comment 标签在模板中显示评论的属性值")]
    public class StlComment
	{
        private StlComment() { }
        public const string ElementName = "stl:comment";

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
            {AttributeType, StringUtils.SortedListToAttributeValueString("显示的类型", TypeList)},
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

        public static readonly string TypeId = "Id";
        public static readonly string TypeAddDate = "AddDate";
        public static readonly string TypeUserName = "UserName";
        public static readonly string TypeDisplayName = "DisplayName";
        public static readonly string TypeGoodCount = "GoodCount";
        public static readonly string TypeContent = "Content";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeId, "评论Id"},
            {TypeAddDate, "评论日期"},
            {TypeUserName, "评论人用户名"},
            {TypeDisplayName, "评论人姓名"},
            {TypeGoodCount, "点赞数"},
            {TypeContent, "评论内容"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
            if (contextInfo.ItemContainer?.CommentItem == null) return string.Empty;
			try
			{
                var ie = node.Attributes?.GetEnumerator();
				var attributes = new StringDictionary();
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
                var type = "content";
                var isDynamic = false;

			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLeftText))
                        {
                            leftText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeRightText))
                        {
                            rightText = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeFormatString))
                        {
                            formatString = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeStartIndex))
                        {
                            startIndex = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeLength))
                        {
                            length = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeWordNum))
                        {
                            wordNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeEllipsis))
                        {
                            ellipsis = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeReplace))
                        {
                            replace = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTo))
                        {
                            to = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsClearTags))
                        {
                            isClearTags = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsReturnToBr))
                        {
                            isReturnToBr = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsLower))
                        {
                            isLower = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsUpper))
                        {
                            isUpper = TranslateUtils.ToBool(attr.Value, true);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            attributes.Add(attr.Name, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(contextInfo, leftText, rightText, formatString, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, type);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(ContextInfo contextInfo, string leftText, string rightText, string formatString, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string type)
        {
            var parsedContent = string.Empty;

            var commentId = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "Id");
            var goodCount = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "GoodCount");
            var userName = SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "UserName");
            var addDate = SqlUtils.EvalDateTime(contextInfo.ItemContainer.CommentItem, "AddDate");
            var content = SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "Content");

            if (StringUtils.EqualsIgnoreCase(type, TypeId))
            {
                parsedContent = commentId.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeAddDate))
            {
                parsedContent = DateUtils.Format(addDate, formatString);
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeUserName))
            {
                parsedContent = string.IsNullOrEmpty(userName) ? "匿名" : userName;
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeDisplayName))
            {
                parsedContent = string.IsNullOrEmpty(userName) ? "匿名" : BaiRongDataProvider.UserDao.GetDisplayName(userName);
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeGoodCount))
            {
                parsedContent = $"<span id='commentsDigg_{commentId}_{true}'>{goodCount}</span>";
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeContent))
            {
                parsedContent = StringUtils.ParseString(content, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);
            }
            else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex))
            {
                parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.CommentItem.ItemIndex, type, contextInfo).ToString();
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = leftText + parsedContent + rightText;
            }

            return parsedContent;
        }
	}
}
