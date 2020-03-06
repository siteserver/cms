using System.Collections.Generic;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsLayerCreateController
    {
        public class CreateRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
            public bool IsIncludeChildren { get; set; }
            public bool IsCreateContents { get; set; }
        }
    }
}