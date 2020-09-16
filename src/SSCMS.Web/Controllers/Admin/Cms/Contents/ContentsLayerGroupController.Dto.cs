using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerGroupController
    {
        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public bool IsCancel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
        }

        public class AddRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }
    }
}