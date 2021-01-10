using System.Linq;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "数据库值", Description = "通过 stl:sqlContent 标签在模板中显示数据库值")]
    public static class StlSqlContent
	{
        public const string ElementName = "stl:sqlContent";

        [StlAttribute(Title = "数据库类型名称")]
        public const string DatabaseTypeName = nameof(DatabaseTypeName);

        [StlAttribute(Title = "数据库类型")]
        public const string DatabaseType = nameof(DatabaseType);

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

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var databaseType = parseManager.SettingsManager.DatabaseType;
		    var connectionString = parseManager.SettingsManager.DatabaseConnectionString;
            var queryString = string.Empty;

            var leftText = string.Empty;
            var rightText = string.Empty;
            var formatString = string.Empty;
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
            var type = string.Empty;

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, DatabaseType))
                {
                    databaseType = TranslateUtils.ToEnum(value, Datory.DatabaseType.MySql);
                }
                else if (StringUtils.EqualsIgnoreCase(name, DatabaseTypeName))
                {
                    value = parseManager.SettingsManager.Configuration[value];
                    if (!string.IsNullOrEmpty(value))
                    {
                        databaseType = TranslateUtils.ToEnum(value, Datory.DatabaseType.MySql);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ConnectionString))
                {
                    connectionString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ConnectionStringName))
                {
                    var connection = parseManager.SettingsManager.Configuration[value];
                    if (!string.IsNullOrEmpty(connection))
                    {
                        connectionString = connection;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString))
                {
                    queryString = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = StringUtils.ToLower(value);
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

		    if (parseManager.ContextInfo.IsStlEntity && string.IsNullOrEmpty(type))
		    {
		        object dataItem = null;
		        if (parseManager.ContextInfo.ItemContainer?.SqlItem != null)
		        {
		            dataItem = parseManager.ContextInfo.ItemContainer?.SqlItem.Value;
		        }
		        else if (!string.IsNullOrEmpty(queryString))
		        {
		            var rows = parseManager.DatabaseManager.GetRows(databaseType, connectionString, queryString);
                    if (rows != null && rows.Any())
		            {
		                dataItem = rows.First();
		            }
		        }

		        return dataItem;
		    }

            return Parse(parseManager, connectionString, queryString, leftText, rightText, formatString, startIndex, length, wordNum, ellipsis, replace, to, isClearTags, isReturnToBr, isLower, isUpper, type);
		}

        private static string Parse(IParseManager parseManager, string connectionString, string queryString, string leftText, string rightText, string formatString, int startIndex, int length, int wordNum, string ellipsis, string replace, string to, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string type)
        {
            var contextInfo = parseManager.ContextInfo;

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
                    var itemIndex = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.SqlItem.Key, type, contextInfo);

                    parsedContent = !string.IsNullOrEmpty(formatString) ? string.Format(formatString, itemIndex) : itemIndex.ToString();
                }
                else
                {
                    var value = ListUtils.GetValueIgnoreCase(contextInfo.ItemContainer.SqlItem.Value, type);
                    if (value != null)
                    {
                        parsedContent = string.Format(formatString, value);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(queryString))
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = parseManager.SettingsManager.Database.ConnectionString;
                }

                //parsedContent = GlobalSettings.DatabaseRepository.GetString(connectionString, queryString);
                parsedContent = parseManager.DatabaseManager.GetString(connectionString, queryString);
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
