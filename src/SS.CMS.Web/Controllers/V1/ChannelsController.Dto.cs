using System.Collections.Generic;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        public class CreateRequest : Dictionary<string, object>
        {
            public int SiteId { get; set; }
            public int ParentId { get; set; }
            public string ContentModelPluginId { get; set; }
            public List<string> ContentRelatedPluginIds { get; set; }
            public string ChannelName { get; set; }
            public string IndexName { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
            public List<string> GroupNames { get; set; }
            public string ImageUrl { get; set; }
            public string Content { get; set; }
            public string Keywords { get; set; }
            public string Description { get; set; }
            public string LinkUrl { get; set; }
            public LinkType LinkType { get; set; }
            public int ChannelTemplateId { get; set; }
            public int ContentTemplateId { get; set; }
        }

        public class UpdateRequest : Dictionary<string, object>
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public string ChannelName { get; set; }
            public string IndexName { get; set; }
            public string ContentModelPluginId { get; set; }
            public string ContentRelatedPluginIds { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
            public List<string> GroupNames { get; set; }
            public string ImageUrl { get; set; }
            public string Content { get; set; }
            public string Keywords { get; set; }
            public string Description { get; set; }
            public string LinkUrl { get; set; }
            public string LinkType { get; set; }
            public int? ChannelTemplateId { get; set; }
            public int? ContentTemplateId { get; set; }
        }
    }
}
