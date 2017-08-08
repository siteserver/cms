using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "依申请公开提交", Description = "通过 stl:govPublicApply 标签在模板中实现依申请公开功能")]
    public class StlGovPublicApply
	{
        private StlGovPublicApply() { }
        public const string ElementName = "stl:govPublicApply";

        public const string AttributeStyleName = "styleName";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeStyleName, "样式名称"}
	    };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var styleName = string.Empty;

            string inputTemplateString;
            string loadingTemplateString;
            string successTemplateString;
            string failureTemplateString;
            StlInnerUtility.GetTemplateLoadingYesNo(pageInfo, contextInfo.InnerXml, out inputTemplateString, out loadingTemplateString, out successTemplateString, out failureTemplateString);

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeStyleName))
                {
                    styleName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
            }

            return ParseImpl(pageInfo, contextInfo, styleName, inputTemplateString, successTemplateString, failureTemplateString);
        }

        public static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string styleName, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BAjaxUpload);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

            var styleInfo = TagStyleManager.GetTagStyleInfo(pageInfo.PublishmentSystemId, ElementName, styleName) ??
                            new TagStyleInfo();
            var applyInfo = new TagStyleGovPublicApplyInfo(styleInfo.SettingsXML);

            var applyTemplate = new GovPublicApplyTemplate(pageInfo.PublishmentSystemInfo, styleInfo, applyInfo);
            var contentBuilder = new StringBuilder(applyTemplate.GetTemplate(styleInfo.IsTemplate, inputTemplateString, successTemplateString, failureTemplateString));

            StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            var parsedContent = contentBuilder.ToString();

            return parsedContent;
        }
	}
}
