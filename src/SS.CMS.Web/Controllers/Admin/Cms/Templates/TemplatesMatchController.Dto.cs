using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesMatchController
    {
        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public IEnumerable<Template> ChannelTemplates { get; set; }
            public IEnumerable<Template> ContentTemplates { get; set; }
        }

        public class MatchRequest
        {
            public int SiteId { get; set; }
            public List<int> ChannelIds { get; set; }
            public bool IsChannelTemplate { get; set; }
            public int TemplateId { get; set; }
        }

        public class CreateRequest
        {
            public int SiteId { get; set; }
            public List<int> ChannelIds { get; set; }
            public bool IsChannelTemplate { get; set; }
            public bool IsChildren { get; set; }
        }
    }
}
