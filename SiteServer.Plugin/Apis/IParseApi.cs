using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IParseApi
    {
        Dictionary<string, string> GetStlElements(string innerXml, List<string> stlElementNames);

        string ParseInnerXml(string innerXml, PluginParseContext context);

        string ParseAttributeValue(string attributeValue, PluginParseContext context);

        string HtmlToXml(string html);

        string XmlToHtml(string xml);

        string GetCurrentUrl(PluginParseContext context);
    }
}
