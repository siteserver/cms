using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    public partial class ContentsLayerAttributesController
    {
        public class SubmitRequest : ChannelRequest
        {
            public List<int> ContentIds { get; set; }
            public string PageType { get; set; }
            public bool IsRecommend { get; set; }
            public bool IsHot { get; set; }
            public bool IsColor { get; set; }
            public bool IsTop { get; set; }
            public int Hits { get; set; }
        }
    }
}
