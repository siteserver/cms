using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "包含文件", Description = "通过 stl:include 标签在模板中包含另一个文件，作为模板的一部分")]
    public class StlInclude
    {
        private StlInclude() { }
        public const string ElementName = "stl:include";

        [StlAttribute(Title = "文件路径")]
        private const string File = nameof(File);

        public static async Task<object> ParseAsync(ParseContext parseContext)
        {
            var file = string.Empty;
            var parameters = new Dictionary<string, string>();

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, File))
                {
                    file = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                    file = parseContext.UrlManager.AddVirtualToUrl(file);
                }
                else
                {
                    parameters[name] = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
            }

            return await ParseImplAsync(parseContext, file, parameters);
        }

        private static async Task<string> ParseImplAsync(ParseContext parseContext, string file, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;

            var pageParameters = parseContext.PageInfo.Parameters;
            parseContext.PageInfo.Parameters = parameters;

            var filePath = parseContext.PathManager.MapPath(parseContext.SiteInfo, parseContext.PathManager.AddVirtualToPath(file));
            var content = await parseContext.TemplateRepository.GetContentByFilePathAsync(filePath);
            var contentBuilder = new StringBuilder(content);
            await parseContext.ParseTemplateContentAsync(contentBuilder);
            var parsedContent = contentBuilder.ToString();

            parseContext.PageInfo.Parameters = pageParameters;

            return parsedContent;
        }
    }
}
