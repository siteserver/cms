using System;
using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [StlElement(Title = "数据库实体", Description = "通过 {sql.} 实体在模板中显示数据库值")]
    public static class StlSqlEntities
	{
        public const string EntityName = "sql";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {StlParserUtility.ItemIndex, "排序"}
	    };

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            if (contextInfo.ItemContainer?.SqlItem == null) return string.Empty;

            try
            {
                var attributeName = stlEntity.Substring(5, stlEntity.Length - 6);

                if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex))
                {
                    parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.SqlItem.Key, attributeName, contextInfo).ToString();
                }
                else
                {
                    if (contextInfo.ItemContainer.SqlItem.Value.TryGetValue(attributeName, out var value))
                    {
                        parsedContent = Convert.ToString(value);
                    }
                }
            }
            catch
            {
                // ignored
            }

            return parsedContent;
        }
	}
}
