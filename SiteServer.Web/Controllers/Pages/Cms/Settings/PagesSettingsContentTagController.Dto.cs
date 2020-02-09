using System.Collections.Generic;
using SiteServer.CMS.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    public partial class PagesSettingsContentTagController
    {
        public class GetRequest: SiteRequest
        {
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class GetResult
        {
            public int Total { get; set; }
            public IEnumerable<string> TagNames { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public List<string> TagNames { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string TagName { get; set; }
        }
    }
}