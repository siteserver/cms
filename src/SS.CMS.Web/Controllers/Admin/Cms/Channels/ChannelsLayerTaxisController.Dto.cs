using System.Collections.Generic;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsLayerTaxisController
    {
        public class SubmitRequest : SiteRequest
        {
            public IEnumerable<int> ChannelIds { get; set; }
            public bool IsUp { get; set; }
            public int Taxis { get; set; }
        }
    }
}