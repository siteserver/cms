using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Library
{
    public partial class LibraryEditorController
    {
        public class GetRequest : SiteRequest
        {
            public int Id { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public string Title { get; set; }

            public int GroupId { get; set; }

            public string ImageUrl { get; set; }

            public string Summary { get; set; }

            public string Content { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int Id { get; set; }

            public string Title { get; set; }

            public string ImageUrl { get; set; }

            public string Summary { get; set; }

            public string Content { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public IFormFile File { set; get; }
        }
    }
}
