using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerCopyController
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