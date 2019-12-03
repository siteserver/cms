using System.Collections.Generic;
using SiteServer.CMS.Dto;

namespace SiteServer.API.Controllers.Pages.Settings.Analysis
{
    public partial class PagesAnalysisSiteController
    {
        public class QueryRequest
        {
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class QueryResultItem
        {
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public string SiteUrl { get; set; }
            public int AddCount { get; set; }
            public int UpdateCount { get; set; }
            public int TotalCount { get; set; }
        }

        public class QueryResult
        {
            public List<string> NewX { get; set; }
            public List<int> NewY { get; set; }
            public List<string> UpdateX { get; set; }
            public List<int> UpdateY { get; set; }
            public List<QueryResultItem> Items { get; set; }
        }
    }
}
