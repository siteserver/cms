using System.Collections.Generic;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    public partial class PagesChannelsLayerGroupController
    {
        public class SubmitRequest : ChannelRequest
        {
            public List<int> ChannelIds { get; set; }
            public bool IsCancel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
        }

        public class AddRequest : ChannelRequest
        {
            public List<int> ChannelIds { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }
    }
}