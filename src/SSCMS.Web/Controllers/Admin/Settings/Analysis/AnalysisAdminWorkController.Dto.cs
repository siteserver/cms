using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    public partial class AnalysisAdminWorkController
    {
        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class QueryResultItem
        {
            public string UserName { get; set; }
            public int AddCount { get; set; }
            public int UpdateCount { get; set; }
        }

        public class QueryResult
        {
            public List<string> NewX { get; set; }
            public List<string> NewY { get; set; }
            public List<string> UpdateY { get; set; }
            public List<QueryResultItem> Items { get; set; }
            public IEnumerable<Cascade<int>> SiteOptions { get; set; }
            public int SiteId { get; set; }
        }
    }
}
