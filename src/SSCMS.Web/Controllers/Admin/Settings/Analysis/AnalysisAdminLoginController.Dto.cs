using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    public partial class AnalysisAdminLoginController
    {
        public class GetRequest
        {
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class GetStat
        {
            public string Date { get; set; }
            public int Success { get; set; }
            public int Failure { get; set; }
        }

        public class GetResult
        {
            public List<string> Days { get; set; }
            public List<int> SuccessCount { get; set; }
            public List<int> FailureCount { get; set; }
        }
    }
}
