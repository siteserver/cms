using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.CMS.StlParser.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginParseContext: IParseContext
    {
        public PluginParseContext(string stlOuterHtml, string stlInnerHtml, NameValueCollection stlAttributes, PageInfo pageInfo, ContextInfo contextInfo)
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
            StlOuterHtml = stlOuterHtml;
            StlInnerHtml = stlInnerHtml;
            StlAttributes = stlAttributes;
            
            StlItems = pageInfo.PluginItems;
            IsStlElement = !contextInfo.IsStlEntity;
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

        public string StlOuterHtml { get; set; }

        public string StlInnerHtml { get; set; }

        public NameValueCollection StlAttributes { get; set; }

        public Dictionary<string, object> StlItems { get; }

        public bool IsStlElement { get; }
    }
}
