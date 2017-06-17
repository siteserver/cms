using System;
using System.Collections.Generic;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;
using System.Text;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "提交简历", Description = "通过 stl:resume 标签在模板中实现提交简历功能")]
    public class StlResume
    {
        private StlResume() { }
        public const string ElementName = "stl:resume";

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

                string yes;
                string no;
                StlInnerUtility.GetYesNo(node, pageInfo, out yes, out no);

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

                parsedContent = ParseImpl(pageInfo, contextInfo, styleName, yes, no);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string styleName, string yes, string no)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BAjaxUpload);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BQueryString);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsInnerCalendar);

            yes = StlParserManager.ParseInnerContent(yes, pageInfo, contextInfo);
            no = StlParserManager.ParseInnerContent(no, pageInfo, contextInfo);

            var resumeTemplate = new ResumeTemplate(pageInfo.PublishmentSystemInfo);
            var parsedContent = resumeTemplate.GetTemplate(yes, no);

            return parsedContent;
        }
    }
}
