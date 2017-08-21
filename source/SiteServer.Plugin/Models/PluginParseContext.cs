using System.Collections.Generic;

namespace SiteServer.Plugin.Models
{
    public class PluginParseContext
    {
        public PluginParseContext(Dictionary<string, string> attributes, string innerXml, int publishmentSystemId, int channelId, int contentId)
        {
            Attributes = attributes;
            InnerXml = innerXml;
            PublishmentSystemId = publishmentSystemId;
            ChannelId = channelId;
            ContentId = contentId;
        }

        public Dictionary<string, string> Attributes { get; set; }
        public string InnerXml { get; set; }
        public int PublishmentSystemId  { get; set; }
        public int ChannelId  { get; set; }
        public int ContentId { get; set; }
    }
}
