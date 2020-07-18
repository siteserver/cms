using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Common.Form
{
    public partial class LayerImageUploadController
    {
        public class Options
        {
            public bool IsEditor { get; set; }
            public bool IsLibrary { get; set; }
            public bool IsThumb { get; set; }
            public int ThumbWidth { get; set; }
            public int ThumbHeight { get; set; }
            public bool IsLinkToOriginal { get; set; }
        }

        public class SubmitRequest : Options
        {
            public int SiteId { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class SubmitResult
        {
            public string ImageUrl { get; set; }
            public string ImageVirtualUrl { get; set; }
            public string PreviewUrl { get; set; }
            public string PreviewVirtualUrl { get; set; }
        }
    }
}