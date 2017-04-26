using System.Collections.Specialized;
using System.Text;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    public class StlElementEntities
    {
        private StlElementEntities()
        {
        }

        public const string EntityName = "stl:";                  //通用实体

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                return attributes;
            }
        }

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
