using System.Collections.Generic;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    public partial class PagesChannelsController
    {
        public class ChannelIdsRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
        }

        public class AppendRequest : SiteRequest
        {
            public bool IsIndexName { get; set; }
            public string Channels { get; set; }
        }
    }
}
