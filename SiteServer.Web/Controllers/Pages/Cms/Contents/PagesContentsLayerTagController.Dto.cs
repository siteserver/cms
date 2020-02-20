using System.Collections.Generic;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerTagController
    {
        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public bool IsCancel { get; set; }
            public IEnumerable<string> TagNames { get; set; }
        }
    }
}