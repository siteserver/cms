using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    public partial class AnalysisUserController
    {
        public class GetRequest
        {
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class GetStat
        {
            public string Date { get; set; }
            public int Register { get; set; }
            public int Login { get; set; }
        }

        public class GetResult
        {
            public List<string> Days { get; set; }
            public List<int> RegisterCount { get; set; }
            public List<int> LoginCount { get; set; }
        }
    }
}