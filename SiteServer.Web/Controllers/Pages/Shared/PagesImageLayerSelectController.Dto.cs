using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Shared
{
    public partial class PagesImageLayerSelectController
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
            public IEnumerable<LibraryImage> Items { get; set; }
        }

        public class SelectRequest : SiteRequest
        {
            public int LibraryId { get; set; }
        }
    }
}
