using Datory;

namespace SSCMS.Core.StlParser.Models
{
    public class StlSearchRequest : Entity
    {
        public bool IsAllSites { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteDir { get; set; }
        public string SiteIds { get; set; }
        public string ChannelIndex { get; set; }
        public string ChannelName { get; set; }
        public string ChannelIds { get; set; }
        public string Type { get; set; }
        public string Word { get; set; }
        public string DateAttribute { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Since { get; set; }
        public int PageNum { get; set; }
        public bool IsHighlight { get; set; }
        public string AjaxDivId { get; set; }
        public string Template { get; set; }
        public int Page { get; set; }
    }
}
