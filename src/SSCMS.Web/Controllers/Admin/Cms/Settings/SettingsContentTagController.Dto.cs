using System.Collections.Generic;
using SSCMS.Dto.Request;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentTagController
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