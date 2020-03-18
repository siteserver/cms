using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Home
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
