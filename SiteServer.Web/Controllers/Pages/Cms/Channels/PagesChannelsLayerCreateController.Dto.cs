using System.Collections.Generic;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    public partial class PagesChannelsLayerCreateController
    {
        public class CreateRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
            public bool IsIncludeChildren { get; set; }
            public bool IsCreateContents { get; set; }
        }
    }
}