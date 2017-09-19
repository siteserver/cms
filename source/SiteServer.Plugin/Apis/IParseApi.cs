using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IParseApi
    {
        void GetTemplateLoadingYesNo(string innerXml, out string template, out string loading, out string yes,
            out string no);

        string ParseInnerXml(string innerXml, PluginParseContext context);

        string ParseAttributeValue(string attributeValue, PluginParseContext context);

        string HtmlToXml(string html);

        string XmlToHtml(string xml);
    }
}
