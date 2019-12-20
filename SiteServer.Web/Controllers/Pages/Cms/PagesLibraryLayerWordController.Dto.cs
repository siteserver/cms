using System.Collections.Generic;

namespace SiteServer.API.Controllers.Pages.Cms
{
    public partial class PagesLibraryLayerWordController
    {
        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public bool IsClearFormat { get; set; }
            public bool IsFirstLineIndent { get; set; }
            public bool IsClearFontSize { get; set; }
            public bool IsClearFontFamily { get; set; }
            public bool IsClearImages { get; set; }
            public List<string> FileNames { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}
