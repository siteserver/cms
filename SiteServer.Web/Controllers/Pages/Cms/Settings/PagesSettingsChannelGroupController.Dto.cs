using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsChannelGroupController
    {
        public class GetResult
        {
            public IEnumerable<ChannelGroup> Groups { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string GroupName { get; set; }
        }

        public class OrderRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public int Taxis { get; set; }
            public bool IsUp { get; set; }
        }
    }
}