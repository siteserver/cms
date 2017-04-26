using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement.WCM
{
	public class StlGovPublicApply
	{
        private StlGovPublicApply() { }
        public const string ElementName = "stl:govpublicapply";     //依申请公开

        public const string Attribute_StyleName = "stylename";		        //样式名称

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
                var applyInfo = new TagStyleGovPublicApplyInfo(string.Empty);

                var inputTemplateString = string.Empty;
                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;
                StlParserUtility.GetInnerTemplateStringOfInput(node, out inputTemplateString, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

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
            var parsedContent = string.Empty;

            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BAjaxUpload);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

            var styleInfo = TagStyleManager.GetTagStyleInfo(pageInfo.PublishmentSystemId, ElementName, styleName);
            if (styleInfo == null)
            {
                styleInfo = new TagStyleInfo();
            }
            var applyInfo = new TagStyleGovPublicApplyInfo(styleInfo.SettingsXML);

            var applyTemplate = new GovPublicApplyTemplate(pageInfo.PublishmentSystemInfo, styleInfo, applyInfo);
            var contentBuilder = new StringBuilder(applyTemplate.GetTemplate(styleInfo.IsTemplate, inputTemplateString, successTemplateString, failureTemplateString));

            StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            parsedContent = contentBuilder.ToString();

            return parsedContent;
        }
	}
}
