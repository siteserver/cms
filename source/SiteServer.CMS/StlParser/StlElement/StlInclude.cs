using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "包含文件", Description = "通过 stl:include 标签在模板中包含另一个文件，作为模板的一部分")]
    public class StlInclude
	{
		private StlInclude(){}
		public const string ElementName = "stl:include";

		public const string AttributeFile = "file";
        public const string AttributeIsContext = "isContext";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
	        {AttributeFile, "文件路径"},
	        {AttributeIsContext, "是否STL解析与当前页面上下文相关"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
            string parsedContent;
			try
			{
                var file = string.Empty;
                var isContext = !pageInfo.PublishmentSystemInfo.Additional.IsCreateIncludeToSsi;
                var isDynamic = false;

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeFile))
                        {
                            file = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            file = PageUtility.AddVirtualToUrl(file);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsContext))
                        {
                            isContext = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, file, isContext);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string file, bool isContext)
        {
            var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(file))
            {
                if (!isContext)
                {
                    var fso = new FileSystemObject(pageInfo.PublishmentSystemId);
                    var parsedFile = fso.CreateIncludeFile(file, false);

                    if (pageInfo.PublishmentSystemInfo.Additional.IsCreateIncludeToSsi)
                    {
                        var pathDifference = PathUtils.GetPathDifference(PathUtils.MapPath("~/"), PathUtility.MapPath(pageInfo.PublishmentSystemInfo, parsedFile));
                        var virtualUrl = pathDifference.Replace("\\", "/").Trim('/');
                        parsedContent = $@"<!--#include virtual=""{virtualUrl}""-->";
                    }
                    else
                    {
                        var filePath = PathUtility.MapPath(pageInfo.PublishmentSystemInfo, parsedFile);
                        parsedContent = FileUtils.ReadText(filePath, pageInfo.TemplateInfo.Charset);
                    }
                }
                else
                {
                    var content = StlCacheManager.FileContent.GetIncludeContent(pageInfo.PublishmentSystemInfo, file, pageInfo.TemplateInfo.Charset);
                    content = StlParserUtility.Amp(content);
                    var contentBuilder = new StringBuilder(content);
                    StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
                    parsedContent = contentBuilder.ToString();
                }
            }

            return parsedContent;
        }
	}
}
