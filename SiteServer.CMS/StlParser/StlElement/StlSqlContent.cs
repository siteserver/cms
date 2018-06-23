using System.Web.UI;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "数据库值", Description = "通过 stl:sqlContent 标签在模板中显示数据库值")]
    public class StlSqlContent
	{
        private StlSqlContent() { }
        public const string ElementName = "stl:sqlContent";

        private static readonly Attr ConnectionStringName = new Attr("connectionStringName", "数据库链接字符串名称");
        private static readonly Attr ConnectionString = new Attr("connectionString", "数据库链接字符串");
        private static readonly Attr QueryString = new Attr("queryString", "数据库查询语句");
		private static readonly Attr Type = new Attr("type", "显示的类型");
        private static readonly Attr LeftText = new Attr("leftText", "显示在信息前的文字");
        private static readonly Attr RightText = new Attr("rightText", "显示在信息后的文字");
        private static readonly Attr FormatString = new Attr("formatString", "显示的格式");
        private static readonly Attr StartIndex = new Attr("startIndex", "字符开始位置");
        private static readonly Attr Length = new Attr("length", "指定字符长度");
		private static readonly Attr WordNum = new Attr("wordNum", "显示字符的数目");
        private static readonly Attr Ellipsis = new Attr("ellipsis", "文字超出部分显示的文字");
        private static readonly Attr Replace = new Attr("replace", "需要替换的文字，可以是正则表达式");
        private static readonly Attr To = new Attr("to", "替换replace的文字信息");
        private static readonly Attr IsClearTags = new Attr("isClearTags", "是否清除标签信息");
        private static readonly Attr IsReturnToBr = new Attr("isReturnToBr", "是否将回车替换为HTML换行标签");
        private static readonly Attr IsLower = new Attr("isLower", "是否转换为小写");
        private static readonly Attr IsUpper = new Attr("isUpper", "是否转换为大写");

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

                if (StringUtils.EqualsIgnoreCase(name, ConnectionString.Name))
                {
                    connectionString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ConnectionStringName.Name))
                {
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        connectionString = WebConfigUtils.ConnectionString;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString.Name))
                {
                    queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type.Name))
                {
                    type = value.ToLower();
                }
                else if (StringUtils.EqualsIgnoreCase(name, LeftText.Name))
                {
                    leftText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, RightText.Name))
                {
                    rightText = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, FormatString.Name))
                {
                    formatString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StartIndex.Name))
                {
                    startIndex = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Length.Name))
                {
                    length = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum.Name))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Ellipsis.Name))
                {
                    ellipsis = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Replace.Name))
                {
                    replace = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, To.Name))
                {
                    to = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsClearTags.Name))
                {
                    isClearTags = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsReturnToBr.Name))
                {
                    isReturnToBr = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsLower.Name))
                {
                    isLower = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsUpper.Name))
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
		            var dataTable = Database.GetDataTable(connectionString, queryString);
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
                parsedContent = Database.GetString(connectionString, queryString);
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
