using System;
using System.Collections.Specialized;
using System.Xml;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlResume
    {
        private StlResume() { }
        public const string ElementName = "stl:resume";//用户登录及状态显示

        public const string Attribute_StyleName = "stylename";              //样式名称

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(Attribute_StyleName, "样式名称");

                return attributes;
            }
        }

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var styleName = string.Empty;

                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;
                StlParserUtility.GetInnerTemplateString(node, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

                var ie = node.Attributes.GetEnumerator();

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_StyleName))
                    {
                        styleName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                }

                parsedContent = ParseImpl(pageInfo, contextInfo, styleName, successTemplateString, failureTemplateString);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string styleName, string successTemplateString, string failureTemplateString)
        {
            var parsedContent = string.Empty;

            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BAjaxUpload);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BQueryString);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsInnerCalendar);

            var resumeTemplate = new ResumeTemplate(pageInfo.PublishmentSystemInfo);
            parsedContent = resumeTemplate.GetTemplate(successTemplateString, failureTemplateString);

            return parsedContent;
        }
    }
}
