using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.StlParser.Models;

namespace SS.CMS.Core.StlParser
{
    public partial class ParseContext
    {
        public int SiteId => PageInfo.SiteId;

        public SiteInfo SiteInfo => PageInfo.SiteInfo;

        public int PageChannelId => PageInfo.PageChannelId;

        public int PageContentId => PageInfo.PageContentId;

        public TemplateInfo TemplateInfo => PageInfo.TemplateInfo;

        public SortedDictionary<string, string> HeadCodes => PageInfo.HeadCodes;

        public SortedDictionary<string, string> BodyCodes => PageInfo.BodyCodes;

        public SortedDictionary<string, string> FootCodes => PageInfo.FootCodes;

        public bool IsLocal => PageInfo.IsLocal;

        public int UniqueId => PageInfo.UniqueId;

        public string ApiUrl => PageInfo.ApiUrl;

        public PageInfo PageInfo { get; set; }
    }
}
