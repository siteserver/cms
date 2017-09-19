using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ParseApi : IParseApi
    {
        private ParseApi() { }

        public static ParseApi Instance { get; } = new ParseApi();

        public void GetTemplateLoadingYesNo(string innerXml, out string template, out string loading, out string yes, out string no)
        {
            StlInnerUtility.GetTemplateLoadingYesNo(innerXml, out template, out loading, out yes, out no);
        }

        public string ParseInnerXml(string innerXml, PluginParseContext context)
        {
            return StlParserManager.ParseInnerContent(innerXml, context);
        }

        public string ParseAttributeValue(string attributeValue, PluginParseContext context)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(context.PublishmentSystemId);
            var templateInfo = new TemplateInfo
            {
                TemplateId = context.TemplateId,
                TemplateType = ETemplateTypeUtils.GetEnumType(context.TemplateType)
            };
            var pageInfo = new PageInfo(context.ChannelId, context.ContentId, publishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo);
            return StlEntityParser.ReplaceStlEntitiesForAttributeValue(attributeValue, pageInfo, contextInfo);
        }

        public string HtmlToXml(string html)
        {
            return StlParserUtility.HtmlToXml(html);
        }

        public string XmlToHtml(string xml)
        {
            return StlParserUtility.XmlToHtml(xml);
        }
    }
}
