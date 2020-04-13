using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ContentsLayerGroupController
    {
        public class GetResult
        {
            public List<string> GroupNames { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public List<int> ContentIds { get; set; }
            public string PageType { get; set; }
            public List<string> GroupNames { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }
    }
}
