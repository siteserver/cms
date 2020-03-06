using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Preview
{
    public partial class PreviewController
    {
        public class GetRequest : SiteRequest
        {
            public int PageIndex { get; set; }
        }

        public class GetChannelRequest : ChannelRequest
        {
            public int PageIndex { get; set; }
        }

        public class GetContentRequest : ChannelRequest
        {
            public int ContentId { get; set; }
            public int PreviewId { get; set; }
            public int PageIndex { get; set; }
        }

        public class GetFileRequest : SiteRequest
        {
            public int FileTemplateId { get; set; }
            public int PageIndex { get; set; }
        }
    }
}
