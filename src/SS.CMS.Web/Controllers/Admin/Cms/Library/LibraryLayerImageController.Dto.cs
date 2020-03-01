using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Library
{
    public partial class LibraryLayerImageController
    {
        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public bool IsThumb { get; set; }
            public int ThumbWidth { get; set; }
            public int ThumbHeight { get; set; }
            public bool IsLinkToOriginal { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Url { get; set; }
        }

        public class SubmitResult
        {
            public string Url { get; set; }
            public string ThumbUrl { get; set; }
        }
    }
}
