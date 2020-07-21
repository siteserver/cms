using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    public partial class EditorController
    {
        public class GetRequest : SiteRequest
        {
            public int LibraryId { get; set; }
        }

        public class GetResult
        {
            public LibraryCard Library { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public string Title { get; set; }

            public int GroupId { get; set; }

            public string ImageUrl { get; set; }

            public string Summary { get; set; }

            public string Body { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int LibraryId { get; set; }

            public string Title { get; set; }

            public string ImageUrl { get; set; }

            public string Summary { get; set; }

            public string Body { get; set; }
        }
    }
}
