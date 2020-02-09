using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerCopyController
    {
        public class GetRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<Content> Contents { get; set; }
            public List<Select<int>> TransSites { get; set; }
        }

        public class GetOptionsRequest : ChannelRequest
        {
            public int TransSiteId { get; set; }
        }

        public class GetOptionsResult
        {
            public Cascade<int> TransChannels { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public int TransSiteId { get; set; }
            public int TransChannelId { get; set; }
            public TranslateContentType CopyType { get; set; }
        }
    }
}