using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Common.Library
{
    public partial class LayerImageController
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
