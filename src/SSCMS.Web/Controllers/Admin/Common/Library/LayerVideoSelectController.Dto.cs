using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Common.Library
{
    public partial class LayerVideoSelectController
    {
        public class QueryRequest
        {
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public IEnumerable<LibraryGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<LibraryVideo> Items { get; set; }
        }

        public class SelectRequest : SiteRequest
        {
            public int LibraryId { get; set; }
        }
    }
}
