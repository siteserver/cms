using System;
using System.Collections.Generic;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "数据库实体", Description = "通过 {sql.} 实体在模板中显示数据库值")]
    public class StlSqlEntities
    {
        private StlSqlEntities()
        {
        }

        public const string EntityName = "sql";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {StlParserUtility.ItemIndex, "排序"}
        };

        internal static string Parse(string stlEntity, ParseContext parseContext)
        {
            var parsedContent = string.Empty;

            if (parseContext.Container?.SqlItem == null) return string.Empty;

            try
            {
                var attributeName = stlEntity.Substring(5, stlEntity.Length - 6);

                if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex))
                {
                    parsedContent = StlParserUtility.ParseItemIndex(parseContext.Container.SqlItem.Key, attributeName, parseContext).ToString();
                }
                else
                {
                    if (parseContext.Container.SqlItem.Value.TryGetValue(attributeName, out var value))
                    {
                        parsedContent = Convert.ToString(value);
                    }
                }

                // parsedContent = StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) ? StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.SqlItem.ItemIndex, attributeName, contextInfo).ToString() : DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, attributeName, "{0}");
            }
            catch
            {
                // ignored
            }

            return parsedContent;
        }
    }
}
