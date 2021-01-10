using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "包含文件", Description = "通过 stl:include 标签在模板中包含另一个文件，作为模板的一部分")]
    public static class StlInclude
	{
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

            return await ParseAsync(parseManager, file, parameters);
		}

        private static async Task<string> ParseAsync(IParseManager parseManager, string file, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;
            var pageInfo = parseManager.PageInfo;

            var pageParameters = pageInfo.Parameters;
            pageInfo.Parameters = parameters;
            var pageIncludeFile = pageInfo.IncludeFile;
            pageInfo.IncludeFile = file;
            var pageEditableIndex = pageInfo.EditableIndex;
            pageInfo.EditableIndex = 0;

            var content = await parseManager.PathManager.GetIncludeContentAsync(pageInfo.Site, file);
            var contentBuilder = new StringBuilder(content);
            await parseManager.ParseTemplateContentAsync(contentBuilder);
            var parsedContent = contentBuilder.ToString();

            pageInfo.Parameters = pageParameters;
            pageInfo.IncludeFile = pageIncludeFile;
            pageInfo.EditableIndex = pageEditableIndex;

            return parsedContent;
        }
	}
}
