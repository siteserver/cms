using System.Collections.Generic;
using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Home
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
