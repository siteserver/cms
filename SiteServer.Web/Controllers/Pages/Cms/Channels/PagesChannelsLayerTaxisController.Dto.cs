using System.Collections.Generic;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Channels
{
    public partial class PagesChannelsLayerTaxisController
    {
        public class SubmitRequest : SiteRequest
        {
            public IEnumerable<int> ChannelIds { get; set; }
            public bool IsUp { get; set; }
            public int Taxis { get; set; }
        }
    }
}