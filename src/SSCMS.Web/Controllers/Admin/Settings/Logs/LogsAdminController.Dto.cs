using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsAdminController
    {
        public class SearchRequest : PageRequest
        {
            public string UserName { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }
    }
}
