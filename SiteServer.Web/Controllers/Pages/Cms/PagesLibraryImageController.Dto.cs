using System.Collections.Generic;
using SiteServer.CMS.Model;

namespace SiteServer.API.Controllers.Pages.Cms
{
    public partial class PagesLibraryImageController
    {
        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public IEnumerable<LibraryGroupInfo> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<LibraryImageInfo> Items { get; set; }
        }

        public class GroupRequest
        {
            public int SiteId { get; set; }
            public string Name { get; set; }
        }
    }
}
