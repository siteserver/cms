using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerHitsController
    {
        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public bool IsAdd { get; set; }
            public int Hits { get; set; }
        }
    }
}