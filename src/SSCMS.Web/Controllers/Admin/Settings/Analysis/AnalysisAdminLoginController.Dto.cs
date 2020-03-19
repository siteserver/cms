using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    public partial class AnalysisAdminLoginController
    {
        public class QueryRequest
        {
            public string DateFrom { get; set; }

            public string DateTo { get; set; }

            public string XType { get; set; }
        }

        public class QueryResult
        {
            public List<string> DateX { get; set; }

            public List<string> DateY { get; set; }

            public List<string> NameX { get; set; }

            public List<string> NameY { get; set; }
        }
    }
}
