using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.StlParser.Model;
using SS.CMS.Core;

namespace SS.CMS.StlParser.StlElement
{
    [StlElement(Title = "包含文件", Description = "通过 stl:include 标签在模板中包含另一个文件，作为模板的一部分")]
    public class StlInclude
	{
		private StlInclude(){}
		public const string ElementName = "stl:include";

        [StlAttribute(Title = "文件路径")]
        private const string File = nameof(File);
        
        public static async Task<object> ParseAsync(IParseManager parseManager)
		{
		    var file = string.Empty;
            var parameters = new Dictionary<string, string>();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, File))
                {
                    file = StringUtils.ReplaceIgnoreCase(value, "{Stl.SiteUrl}", "@");
                    file = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(file);
                    file = parseManager.PathManager.AddVirtualToUrl(file);
                }
                else
                {
                    parameters[name] = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
            }

            return await ParseImplAsync(parseManager, file, parameters);
		}

        private static async Task<string> ParseImplAsync(IParseManager parseManager, string file, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;

            var pageParameters = parseManager.PageInfo.Parameters;
            parseManager.PageInfo.Parameters = parameters;

            var content = await parseManager.PathManager.GetIncludeContentAsync(parseManager.PageInfo.Site, file);
            var contentBuilder = new StringBuilder(content);
            await parseManager.ParseTemplateContentAsync(contentBuilder);
            var parsedContent = contentBuilder.ToString();

            parseManager.PageInfo.Parameters = pageParameters;

            return parsedContent;
        }
	}
}
