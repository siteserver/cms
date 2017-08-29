using System.Text;

namespace SiteServer.Plugin.Apis
{
    public interface IParseApi
    {
        void GetTemplateLoadingYesNo(string innerXml, out string template, out string loading, out string yes,
            out string no);

        string Parse(string innerXml, int publishmentSystemId, int channelId, int contentId);

        void Parse(StringBuilder builder, int publishmentSystemId, int channelId, int contentId);

        string HtmlToXml(string html);

        string XmlToHtml(string xml);
    }
}
