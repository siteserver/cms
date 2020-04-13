using System.Collections.Generic;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    public partial class EditorLayerTextController
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
            public IEnumerable<LibraryGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<LibraryText> Items { get; set; }
        }

        public class GroupRequest
        {
            public int SiteId { get; set; }
            public string Name { get; set; }
        }
    }
}
