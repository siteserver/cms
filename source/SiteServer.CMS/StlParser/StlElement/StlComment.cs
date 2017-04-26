using System;
using System.Collections.Specialized;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlComment
	{
        private StlComment() { }
        public const string ElementName = "stl:comment";                    //评论属性

		public const string AttributeType = "type";						    //显示的类型
        public const string AttributeLeftText = "lefttext";                 //显示在信息前的文字
        public const string AttributeRightText = "righttext";               //显示在信息后的文字
        public const string AttributeFormatString = "formatstring";         //显示的格式
        public const string AttributeStartIndex = "startindex";			    //字符开始位置
        public const string AttributeLength = "length";			            //指定字符长度
		public const string AttributeWordNum = "wordnum";					//显示字符的数目
        public const string AttributeEllipsis = "ellipsis";                 //文字超出部分显示的文字
        public const string AttributeReplace = "replace";                   //需要替换的文字，可以是正则表达式
        public const string AttributeTo = "to";                             //替换replace的文字信息
        public const string AttributeIsClearTags = "iscleartags";           //是否清除标签信息
        public const string AttributeIsReturnToBr = "isreturntobr";         //是否将回车替换为HTML换行标签
        public const string AttributeIsLower = "islower";			        //转换为小写
        public const string AttributeIsUpper = "isupper";			        //转换为大写
        public const string AttributeIsDynamic = "isdynamic";               //是否动态显示

		public static ListDictionary AttributeList => new ListDictionary
		{
		    {AttributeType, "显示的类型"},
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
		    {AttributeIsLower, "转换为小写"},
		    {AttributeIsUpper, "转换为大写"},
		    {AttributeIsDynamic, "是否动态显示"}
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

            var commentId = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "ID");
            //var nodeId = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, CommentAttribute.NodeId);
            //var contentId = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, CommentAttribute.ContentId);
            var goodCount = SqlUtils.EvalInt(contextInfo.ItemContainer.CommentItem, "GoodCount");
            var userName = SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "UserName");
            //var isChecked = TranslateUtils.ToBool(SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, CommentAttribute.IsChecked));
            var addDate = SqlUtils.EvalDateTime(contextInfo.ItemContainer.CommentItem, "AddDate");
            var content = SqlUtils.EvalString(contextInfo.ItemContainer.CommentItem, "Content");

            if (type == "id")
            {
                parsedContent = commentId.ToString();
            }
            else if (type == "adddate")
            {
                parsedContent = DateUtils.Format(addDate, formatString);
            }
            else if (type == "username")
            {
                parsedContent = string.IsNullOrEmpty(userName) ? "匿名" : userName;
            }
            else if (type == "goodcount")
            {
                parsedContent = $"<span id='commentsDigg_{commentId}_{true}'>{goodCount}</span>";
            }
            else if (type == "content")
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
