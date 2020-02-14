using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Shared
{
    public partial class PagesGroupContentLayerAddController
    {
        public class GetRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class GetResult
        {
            public string GroupName { get; set; }
            public string Description { get; set; }
        }

        public class AddRequest : SiteRequest
        {
            public string GroupName { get; set; }
            public string Description { get; set; }
        }

        public class EditRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }

        public class ListResult
        {
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<ContentGroup> Groups { get; set; }
        }
    }
}
