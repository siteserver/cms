using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Admin.Settings.Logs
{
    public partial class LogsUserController
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
