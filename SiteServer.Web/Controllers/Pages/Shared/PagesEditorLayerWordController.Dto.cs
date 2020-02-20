using System.Collections.Generic;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Shared
{
    public partial class PagesEditorLayerWordController
    {
        public class SubmitRequest : SiteRequest
        {
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
