using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "数据库值", Description = "通过 stl:sqlContent 标签在模板中显示数据库值")]
    public class StlSqlContent
	{
        private StlSqlContent() { }
        public const string ElementName = "stl:sqlContent";

        public const string AttributeConnectionStringName = "connectionStringName";
        public const string AttributeConnectionString = "connectionString";
        public const string AttributeQueryString = "queryString";

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
	        {AttributeConnectionStringName, "数据库链接字符串名称"},
	        {AttributeConnectionString, "数据库链接字符串"},
	        {AttributeQueryString, "数据库查询语句"},
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
	        {AttributeIsLower, "是否转换为小写"},
	        {AttributeIsUpper, "是否转换为大写"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
                var connectionString = string.Empty;
                var queryString = string.Empty;

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

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
			        while (ie.MoveNext())
			        {
			            var attr = (XmlAttribute) ie.Current;

			            if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeConnectionString))
			            {
			                connectionString = attr.Value;
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeConnectionStringName))
			            {
			                if (string.IsNullOrEmpty(connectionString))
			                {
			                    connectionString = WebConfigUtils.ConnectionString;
			                }
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeQueryString))
			            {
			                queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType))
			            {
			                type = attr.Value.ToLower();
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
			        }
			    }

			    parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(contextInfo, connectionString, queryString, leftText, rightText, formatString, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, type);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(ContextInfo contextInfo, string connectionString, string queryString, string leftText, string rightText, string formatString, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string type)
        {
            var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(type) && contextInfo.ItemContainer?.SqlItem != null)
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

                if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex))
                {
                    var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.SqlItem.ItemIndex, type, contextInfo);

                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
                }
                else
                {
                    parsedContent = DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, type, formatString);
                }
            }
            else if (!string.IsNullOrEmpty(queryString))
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = WebConfigUtils.ConnectionString;
                }

                parsedContent = BaiRongDataProvider.DatabaseDao.GetString(connectionString, queryString);
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
