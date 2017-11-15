using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "包含文件", Description = "通过 stl:include 标签在模板中包含另一个文件，作为模板的一部分")]
    public class StlInclude
	{
		private StlInclude(){}
		public const string ElementName = "stl:include";

		public const string AttributeFile = "file";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
	        {AttributeFile, "文件路径"}
	    };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var file = string.Empty;

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeFile))
                {
                    file = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    file = PageUtility.AddVirtualToUrl(file);
                }
            }

            return ParseImpl(pageInfo, contextInfo, file);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string file)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;

            var content = TemplateManager.GetIncludeContent(pageInfo.PublishmentSystemInfo, file, pageInfo.TemplateInfo.Charset);
            content = StlParserUtility.Amp(content);
            var contentBuilder = new StringBuilder(content);
            StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            var parsedContent = contentBuilder.ToString();

            return parsedContent;
        }
	}
}
