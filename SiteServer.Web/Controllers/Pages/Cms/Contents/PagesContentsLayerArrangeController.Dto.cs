using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerArrangeController
    {
        public class SubmitRequest : ChannelRequest
        {
            public string AttributeName { get; set; }
            public bool IsDesc { get; set; }
        }
    }
}