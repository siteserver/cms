using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

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

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var styleName = string.Empty;

            string yes;
            string no;
            StlInnerUtility.GetYesNo(pageInfo, contextInfo.InnerXml, out yes, out no);

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeStyleName))
                {
                    styleName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
            }

            return ParseImpl(pageInfo, contextInfo, styleName, yes, no);
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
