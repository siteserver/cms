using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser.Models;

namespace SS.CMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "STL标签实体", Description = "通过 {stl:} 实体在模板中实现STL标签")]
    public static class StlElementEntities
    {
        public const string EntityName = "stl:";

        public static SortedList<string, string> AttributeList => null;

        internal static async Task<string> ParseAsync(string stlEntity, ParseContext parseContext)
        {
            var parsedContent = string.Empty;

            parseContext.IsStlEntity = true;
            try
            {
                var stlElement = $"<{stlEntity.Trim(' ', '{', '}')} />";

                var innerBuilder = new StringBuilder(stlElement);
                await parseContext.ParseInnerContentAsync(innerBuilder);
                parsedContent = innerBuilder.ToString();
            }
            catch
            {
                // ignored
            }
            parseContext.IsStlEntity = false;

            return parsedContent;
        }
    }
}
