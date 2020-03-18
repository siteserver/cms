using System.Collections.Generic;
using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Admin.Shared
{
    public partial class GroupContentLayerAddController
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
