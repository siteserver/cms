using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Home
{
    public partial class ContentsLayerArrangeController
    {
        public class SubmitRequest : ChannelRequest
        {
            public string AttributeName { get; set; }
            public bool IsDesc { get; set; }
        }
    }
}
