using System.Text;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ParseApi : IParseApi
    {
        public void GetTemplateLoadingYesNo(string innerXml, out string template, out string loading, out string yes, out string no)
        {
            StlInnerUtility.GetTemplateLoadingYesNo(innerXml, out template, out loading, out yes, out no);
        }

        public string Parse(string innerXml, int publishmentSystemId, int channelId, int contentId)
        {
            return StlParserManager.ParseInnerContent(innerXml, publishmentSystemId, channelId, contentId);
        }

        public void Parse(StringBuilder builder, int publishmentSystemId, int channelId, int contentId)
        {
            StlParserManager.ParseInnerContent(builder, publishmentSystemId, channelId, contentId);
        }
    }
}
