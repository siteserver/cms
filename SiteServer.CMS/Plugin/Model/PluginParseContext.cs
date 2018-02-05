using System.Collections.Generic;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginParseContext: IParseContext
    {
        public PluginParseContext(Dictionary<string, string> stlElementAttributes, string stlElementInnerXml, PageInfo pageInfo, int channelId, int contentId, IContentInfo contentInfo)
        {
            StlElementAttributes = stlElementAttributes;
            StlElementInnerXml = stlElementInnerXml;
            SiteId = pageInfo.SiteId;
            ChannelId = channelId;
            ContentId = contentId;
            ContentInfo = contentInfo;
            TemplateType = pageInfo.TemplateInfo.TemplateType;
            TemplateId = pageInfo.TemplateInfo.Id;

            HeadCodes = pageInfo.HeadCodes;
            BodyCodes = pageInfo.BodyCodes;
            FootCodes = pageInfo.FootCodes;
        }

        public Dictionary<string, string> StlElementAttributes { get; set; }
        public string StlElementInnerXml { get; set; }
        public int SiteId  { get; set; }
        public int ChannelId  { get; set; }
        public int ContentId { get; set; }
        public IContentInfo ContentInfo { get; set; }
        public TemplateType TemplateType { get; set; }
        public int TemplateId { get; set; }

        public SortedDictionary<string, string> HeadCodes { get; }

        public SortedDictionary<string, string> BodyCodes { get; }

        public SortedDictionary<string, string> FootCodes { get; }
    }
}
