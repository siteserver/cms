using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Common.Library
{
    public partial class EditorController
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
    }
}
