using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ContentsLayerStateController
    {
        public class GetRequest : ChannelRequest
        {
            public int ContentId { set; get; }
        }

        public class GetResult
        {
            public List<ContentCheck> ContentChecks { get; set; }
            public string Title { set; get; }
            public string CheckState { set; get; }
        }
    }
}
