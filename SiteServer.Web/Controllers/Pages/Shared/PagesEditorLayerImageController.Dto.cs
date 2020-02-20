using System.Collections.Generic;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Shared
{
    public partial class PagesEditorLayerImageController
    {
        public class SubmitRequest : SiteRequest
        {
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
            public string ImageUrl { get; set; }
            public string PreviewUrl { get; set; }
        }
    }
}