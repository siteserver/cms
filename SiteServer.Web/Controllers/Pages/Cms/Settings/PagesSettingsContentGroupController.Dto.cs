using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsContentGroupController
    {
        public class GetResult
        {
            public IEnumerable<ContentGroup> Groups { get; set; }
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