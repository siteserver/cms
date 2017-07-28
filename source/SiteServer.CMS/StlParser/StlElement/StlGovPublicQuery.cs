using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "依申请公开查询", Description = "通过 stl:govPublicQuery 标签在模板中实现依申请公开查询功能")]
    public class StlGovPublicQuery
	{
        private StlGovPublicQuery() { }
        public const string ElementName = "stl:govPublicQuery";

        public const string AttributeStyleName = "styleName";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeStyleName, "样式名称"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var styleName = string.Empty;

                string inputTemplateString;
                string successTemplateString;
                string failureTemplateString;
                StlParserUtility.GetInnerTemplateStringOfInput(node, out inputTemplateString, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeStyleName))
                        {
                            styleName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                    }
                }

                parsedContent = ParseImpl(pageInfo, contextInfo, styleName, inputTemplateString, successTemplateString, failureTemplateString);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string styleName, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BjTemplates);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

            var styleInfo = TagStyleManager.GetTagStyleInfo(pageInfo.PublishmentSystemId, ElementName, styleName) ??
                            new TagStyleInfo();

            var queryTemplate = new GovPublicQueryTemplate(pageInfo.PublishmentSystemInfo, styleInfo);
            var contentBuilder = new StringBuilder(queryTemplate.GetTemplate(styleInfo.IsTemplate, inputTemplateString, successTemplateString, failureTemplateString));

            StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            var parsedContent = contentBuilder.ToString();

            return parsedContent;
        }
	}
}
