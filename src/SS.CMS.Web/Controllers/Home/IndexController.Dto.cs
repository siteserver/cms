using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Home
{
    public partial class IndexController
    {
        public class GetRequest : ChannelRequest
        {
            public string PageName { get; set; }
            public int ContentId { get; set; }
        }
    }
}
