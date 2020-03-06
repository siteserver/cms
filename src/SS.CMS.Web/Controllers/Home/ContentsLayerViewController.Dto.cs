using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Home
{
    public partial class ContentsLayerViewController
    {
        public class GetRequest : ChannelRequest
        {
            public int ContentId { set; get; }
        }

        public class GetResult
        {
            public Content Content { set; get; }
            public string ChannelName { set; get; }
            public List<ContentColumn> Attributes { set; get; }
        }
    }
}
