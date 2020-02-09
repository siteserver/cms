using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerStateController
    {
        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public List<ContentCheck> ContentChecks { get; set; }
            public Content Content { get; set; }
        }
    }
}