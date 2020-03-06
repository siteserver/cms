using System.Collections.Generic;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerTagController
    {
        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public bool IsCancel { get; set; }
            public IEnumerable<string> TagNames { get; set; }
        }
    }
}