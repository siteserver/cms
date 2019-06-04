using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;

namespace SS.CMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "STL标签实体", Description = "通过 {stl:} 实体在模板中实现STL标签")]
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

            contextInfo.IsStlEntity = true;
            try
            {
                var stlElement = $"<{stlEntity.Trim(' ', '{', '}')} />";

                var innerBuilder = new StringBuilder(stlElement);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                parsedContent = innerBuilder.ToString();
            }
            catch
            {
                // ignored
            }
            contextInfo.IsStlEntity = false;

            return parsedContent;
        }
    }
}
