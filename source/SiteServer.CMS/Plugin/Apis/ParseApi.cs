using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin.Apis;

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

        public string ParseInnerXml(string innerXml, int publishmentSystemId, int channelId, int contentId)
        {
            return StlParserManager.ParseInnerContent(innerXml, publishmentSystemId, channelId, contentId);
        }

        public string ParseAttributeValue(string attributeValue, int publishmentSystemId, int channelId, int contentId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var pageInfo = new PageInfo(channelId, contentId, publishmentSystemInfo, null, null);
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
