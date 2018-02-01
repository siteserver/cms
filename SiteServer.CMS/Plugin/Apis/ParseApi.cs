using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ParseApi : IParseApi
    {
        private ParseApi() { }

        private static ParseApi _instance;
        public static ParseApi Instance => _instance ?? (_instance = new ParseApi());

        public Dictionary<string, string> GetStlElements(string innerXml, List<string> stlElementNames)
        {
            return StlInnerUtility.GetStlElements(innerXml, stlElementNames);
        }

        public string ParseInnerXml(string innerXml, IParseContext context)
        {
            return StlParserManager.ParseInnerContent(innerXml, context);
        }

        public string ParseAttributeValue(string attributeValue, IParseContext context)
        {
            var siteInfo = SiteManager.GetSiteInfo(context.SiteId);
            var templateInfo = new TemplateInfo
            {
                Id = context.TemplateId,
                TemplateType = context.TemplateType
            };
            var pageInfo = new PageInfo(context.ChannelId, context.ContentId, siteInfo, templateInfo);
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

        public string GetCurrentUrl(IParseContext context)
        {
            var siteInfo = SiteManager.GetSiteInfo(context.SiteId);
            return StlUtility.GetStlCurrentUrl(siteInfo, context.ChannelId, context.ContentId,
                context.ContentInfo, context.TemplateType, context.TemplateId, false);
        }
    }
}
