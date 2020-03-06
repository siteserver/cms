using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsSiteController
    {
        public class SearchRequest : PageRequest
        {
            public List<int> SiteIds { get; set; }
            public string LogType { get; set; }
            public string UserName { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class SiteLogResult : SiteLog
        {
            public string WebUrl { get; set; }

            public string SiteName { get; set;  }
        }

        public class SiteLogPageResult : PageResult<SiteLogResult>
        {
            public IEnumerable<Cascade<int>> SiteOptions { get; set; }
        }
    }
}
