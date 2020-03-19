using System.Collections.Generic;
using SSCMS;
using SSCMS.Dto;
using SSCMS.Dto.Request;
using SSCMS.Dto.Result;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsErrorController
    {
        public class SearchRequest : PageRequest
        {
            public string Category { get; set; }
            public string PluginId { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class SearchResult : PageResult<ErrorLog>
        {
            public List<Select<string>> Categories { get; set; }
            public List<Select<string>> PluginIds { get; set; }
        }
    }
}
