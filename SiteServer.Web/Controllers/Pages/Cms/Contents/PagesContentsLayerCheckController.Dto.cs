using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerCheckController
    {
        public class GetRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<Content> Contents { get; set; }
            public IEnumerable<Select<int>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
            public IEnumerable<Select<int>> TransSites { get; set; }
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
            public int CheckedLevel { get; set; }
            public string Reasons { get; set; }
            public bool IsTranslate { get; set; }
            public int TransSiteId { get; set; }
            public int TransChannelId { get; set; }
            
        }
    }
}