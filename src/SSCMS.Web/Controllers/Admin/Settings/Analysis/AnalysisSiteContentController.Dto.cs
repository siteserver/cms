using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    public partial class AnalysisSiteContentController
    {
        public class GetRequest
        {
            public int SiteId { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class GetStat
        {
            public string Date { get; set; }
            public int Add { get; set; }
            public int Edit { get; set; }
        }

        public class GetResult
        {
            public List<Cascade<int>> Sites { get; set; }
            public List<string> Days { get; set; }
            public List<int> AddCount { get; set; }
            public List<int> EditCount { get; set; }
        }
    }
}