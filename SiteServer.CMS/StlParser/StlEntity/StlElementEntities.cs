using System.Collections.Generic;
using System.Text;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "STL标签实体", Description = "通过 {stl:} 实体在模板中实现STL标签")]
    public class StlElementEntities
    {
        private StlElementEntities()
        {
        }

        public const string EntityName = "stl:";

        public static SortedList<string, string> AttributeList => null;

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            contextInfo.IsCurlyBrace = true;
            try
            {
                var stlElement = StlParserUtility.HtmlToXml($"<{stlEntity.Trim(' ', '{', '}')} />");

                var innerBuilder = new StringBuilder(stlElement);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                parsedContent = innerBuilder.ToString();
            }
            catch
            {
                // ignored
            }
            contextInfo.IsCurlyBrace = false;

            return parsedContent;
        }
    }
}
