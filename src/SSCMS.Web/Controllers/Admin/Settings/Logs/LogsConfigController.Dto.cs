using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    public partial class LogsConfigController
    {
        public class GetResult
        {
            public Config Config { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsTimeThreshold { get; set; }
            public int TimeThreshold { get; set; }
            public bool IsLogSite { get; set; }

            public bool IsLogAdmin { get; set; }
            public bool IsLogUser { get; set; }
            public bool IsLogError { get; set; }
        }
    }
}
