using System.Web.UI;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "数据库值", Description = "通过 stl:sqlContent 标签在模板中显示数据库值")]
    public class StlSqlContent
	{
        private StlSqlContent() { }
        public const string ElementName = "stl:sqlContent";

        [StlAttribute(Title = "数据库链接字符串名称")]
        private const string ConnectionStringName = nameof(ConnectionStringName);
        
        [StlAttribute(Title = "数据库链接字符串")]
        private const string ConnectionString = nameof(ConnectionString);
        
        [StlAttribute(Title = "数据库查询语句")]
        private const string QueryString = nameof(QueryString);
        
		[StlAttribute(Title = "显示的类型")]
        private const string Type = nameof(Type);
        
        [StlAttribute(Title = "显示在信息前的文字")]
        private const string LeftText = nameof(LeftText);
        
        [StlAttribute(Title = "显示在信息后的文字")]
        private const string RightText = nameof(RightText);
        
        [StlAttribute(Title = "显示的格式")]
        private const string FormatString = nameof(FormatString);
        
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
        
        [StlAttribute(Title = "是否清除标签信息")]
        private const string IsClearTags = nameof(IsClearTags);
        
        [StlAttribute(Title = "是否将回车替换为HTML换行标签")]
        private const string IsReturnToBr = nameof(IsReturnToBr);
        
        [StlAttribute(Title = "是否转换为小写")]
        private const string IsLower = nameof(IsLower);
        
        [StlAttribute(Title = "是否转换为大写")]
        private const string IsUpper = nameof(IsUpper);

        public static object Parse(PageInfo pageInfo, ContextInfo contextInfo)
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

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ConnectionString))
                {
                    connectionString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ConnectionStringName))
                {
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        connectionString = WebConfigUtils.ConnectionString;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString))
                {
                    queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value.ToLower();
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

		    if (contextInfo.IsStlEntity && string.IsNullOrEmpty(type))
		    {
		        object dataItem = null;
		        if (contextInfo.ItemContainer?.SqlItem != null)
		        {
		            dataItem = contextInfo.ItemContainer?.SqlItem.DataItem;
		        }
		        else if (!string.IsNullOrEmpty(queryString))
		        {
		            var dataTable = StlDatabaseCache.GetDataTable(connectionString, queryString);
		            var dictList = TranslateUtils.DataTableToDictionaryList(dataTable);
		            if (dictList != null && dictList.Count >= 1)
		            {
		                dataItem = dictList[0];
		            }
		        }

		        return dataItem;
		    }

            return ParseImpl(contextInfo, connectionString, queryString, leftText, rightText, formatString, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, type);
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

                //parsedContent = DataProvider.DatabaseDao.GetString(connectionString, queryString);
                parsedContent = StlDatabaseCache.GetString(connectionString, queryString);
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                parsedContent = InputTypeUtils.ParseString(InputType.Text, parsedContent, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString);

                if (!string.IsNullOrEmpty(parsedContent))
                {
                    parsedContent = leftText + parsedContent + rightText;
                }
            }

            return parsedContent;
        }
	}
}
