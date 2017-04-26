using System.Collections.Specialized;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
	public class StlSqlEntities
	{
        private StlSqlEntities()
		{
		}

        public const string EntityName = "Sql";              //数据库实体

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(StlParserUtility.ItemIndex, "排序");

                return attributes;
            }
        }

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            if (contextInfo.ItemContainer == null || contextInfo.ItemContainer.SqlItem == null) return string.Empty;

            try
            {
                var attributeName = stlEntity.Substring(5, stlEntity.Length - 6);
                if (StringUtils.StartsWithIgnoreCase(attributeName, StlParserUtility.ItemIndex))
                {
                    parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.SqlItem.ItemIndex, attributeName, contextInfo).ToString();
                }
                else
                {
                    parsedContent = DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, attributeName, "{0}");
                }
            }
            catch { }

            return parsedContent;
        }
	}
}
