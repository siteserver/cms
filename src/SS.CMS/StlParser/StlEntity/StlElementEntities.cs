using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;

namespace SS.CMS.StlParser.StlEntity
{
    [StlElement(Title = "STL标签实体", Description = "通过 {stl:} 实体在模板中实现STL标签")]
    public static class StlElementEntities
    {
        public const string EntityName = "stl:";

        public static SortedList<string, string> AttributeList => null;

        internal static async Task<string> ParseAsync(string stlEntity, IParseManager parseManager)
        {
            var parsedContent = string.Empty;

            parseManager.ContextInfo.IsStlEntity = true;
            try
            {
                var stlElement = $"<{stlEntity.Trim(' ', '{', '}')} />";

                var innerBuilder = new StringBuilder(stlElement);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                parsedContent = innerBuilder.ToString();
            }
            catch
            {
                // ignored
            }
            parseManager.ContextInfo.IsStlEntity = false;

            return parsedContent;
        }
    }
}
