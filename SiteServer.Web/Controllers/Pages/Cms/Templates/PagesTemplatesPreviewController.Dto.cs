using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    public partial class PagesTemplatePreviewController
    {
        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public string Content { get; set; }
        }

        public class CacheRequest
        {
            public int SiteId { get; set; }
            public string Content { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public TemplateType TemplateType { get; set; }
            public int ChannelId { get; set; }
            public string Content { get; set; }
        }
    }
}
