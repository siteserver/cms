using System.Collections.Generic;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginParseContext: IParseContext
    {
        public PluginParseContext(Dictionary<string, string> stlAttributes, string stlInnerXml, PageInfo pageInfo, ContextInfo contextInfo)
        {
            SiteId = contextInfo.SiteInfo.Id;
            ChannelId = contextInfo.ChannelId;
            ContentId = contextInfo.ContentId;
            ContentInfo = contextInfo.ContentInfo;
            TemplateType = pageInfo.TemplateInfo.TemplateType;
            TemplateId = pageInfo.TemplateInfo.Id;

            StlPageHead = pageInfo.HeadCodes;
            StlPageBody = pageInfo.BodyCodes;
            StlPageFoot = pageInfo.FootCodes;
            StlAttributes = stlAttributes;
            StlInnerXml = stlInnerXml;
            StlItems = pageInfo.PluginItems;
            IsStlEntity = contextInfo.IsStlEntity;
        }

        public int SiteId  { get; set; }

        public int ChannelId  { get; set; }

        public int ContentId { get; set; }

        public IContentInfo ContentInfo { get; set; }

        public TemplateType TemplateType { get; set; }

        public int TemplateId { get; set; }

        public SortedDictionary<string, string> StlPageHead { get; }

        public SortedDictionary<string, string> StlPageBody { get; }

        public SortedDictionary<string, string> StlPageFoot { get; }

        public Dictionary<string, string> StlAttributes { get; set; }

        public string StlInnerXml { get; set; }

        public Dictionary<string, object> StlItems { get; }

        public bool IsStlEntity { get; }
    }
}
