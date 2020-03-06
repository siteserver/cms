using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentGroupController
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