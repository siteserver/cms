using System.Collections.Generic;
using SiteServer.CMS.StlParser.Editor;

namespace SiteServer.API.Controllers.Pages.Cms
{
    public partial class PagesEditorLayerImageController
    {
        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
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
            public string VueType { get; set; }
            public string VueValue { get; set; }
        }
    }
}
