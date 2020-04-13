using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    public partial class LibraryFileController
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
            public IEnumerable<LibraryFile> Items { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class DownloadRequest : SiteRequest
        {
            public int LibraryId { get; set; }
        }

        public class GroupRequest
        {
            public int SiteId { get; set; }
            public string Name { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int Id { get; set; }

            public string Title { get; set; }

            public int GroupId { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int Id { get; set; }
        }

        public class DeleteGroupRequest : SiteRequest
        {
            public int Id { get; set; }
        }
    }
}
