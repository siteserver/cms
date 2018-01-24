using System.Collections.Generic;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginParseContext: IParseContext
    {
        public PluginParseContext(Dictionary<string, string> attributes, string innerXml, int siteId, int channelId, int contentId, IContentInfo contentInfo, TemplateType templateType, int templateId)
        {
            Attributes = attributes;
            InnerXml = innerXml;
            SiteId = siteId;
            ChannelId = channelId;
            ContentId = contentId;
            ContentInfo = contentInfo;
            TemplateType = templateType;
            TemplateId = templateId;
        }

        public Dictionary<string, string> Attributes { get; set; }
        public string InnerXml { get; set; }
        public int SiteId  { get; set; }
        public int ChannelId  { get; set; }
        public int ContentId { get; set; }
        public IContentInfo ContentInfo { get; set; }
        public TemplateType TemplateType { get; set; }
        public int TemplateId { get; set; }
    }
}
