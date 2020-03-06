using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Home
{
    public partial class ContentsLayerColumnsController
    {
        public class GetResult
        {
            public List<ContentColumn> Attributes { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string AttributeNames { get; set; }
        }
    }
}
