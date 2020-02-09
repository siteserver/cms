using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerTaxisController
    {
        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public bool IsUp { get; set; }
            public int Taxis { get; set; }
        }
    }
}