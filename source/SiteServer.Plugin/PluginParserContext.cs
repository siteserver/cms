using System.Collections.Generic;

namespace SiteServer.Plugin
{
    public class PluginParserContext
    {
        public PluginParserContext(string elementName, Dictionary<string, string> attributes, string innerXml, int siteId, int channelId, int contentId)
        {
            ElementName = elementName;
            Attributes = attributes;
            InnerXml = innerXml;
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
        }

        public string ElementName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string InnerXml { get; set; }
        public int SiteId  { get; set; }
        public int ChannelId  { get; set; }
        public int ContentId { get; set; }
    }
}
