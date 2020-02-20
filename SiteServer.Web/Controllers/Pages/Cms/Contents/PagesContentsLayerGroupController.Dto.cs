using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerGroupController
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