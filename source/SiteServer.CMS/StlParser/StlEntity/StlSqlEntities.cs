using System.Collections.Generic;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "数据库实体", Description = "通过 {sql.} 实体在模板中显示数据库值")]
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

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            if (contextInfo.ItemContainer?.SqlItem == null) return string.Empty;

            try
            {
                var attributeName = stlEntity.Substring(5, stlEntity.Length - 6);
                parsedContent = StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex) ? StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.SqlItem.ItemIndex, attributeName, contextInfo).ToString() : DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, attributeName, "{0}");
            }
            catch
            {
                // ignored
            }

            return parsedContent;
        }
	}
}
