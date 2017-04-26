using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlInclude
	{
		private StlInclude(){}
		public const string ElementName = "stl:include";//包含文件

		public const string Attribute_File = "file";	//文件路径
        public const string Attribute_IsContext = "iscontext";            //是否STL标签上下文相关
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add(Attribute_File, "文件路径");
                attributes.Add(Attribute_IsContext, "是否STL解析与当前页面相关");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}

		//对“包含文件”（stl:include）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
            var parsedContent = string.Empty;
			try
			{
				var ie = node.Attributes.GetEnumerator();
				var file = string.Empty;
                var isContext = !pageInfo.PublishmentSystemInfo.Additional.IsCreateIncludeToSsi;
                var isDynamic = false;

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_File))
					{
                        file = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        file = PageUtility.AddVirtualToUrl(file);
                    }
                    else if (attributeName.Equals(Attribute_IsContext))
                    {
                        isContext = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, contextInfo, file, isContext);
                }
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
                    var FSO = new FileSystemObject(pageInfo.PublishmentSystemId);
                    var parsedFile = FSO.CreateIncludeFile(file, false);

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
