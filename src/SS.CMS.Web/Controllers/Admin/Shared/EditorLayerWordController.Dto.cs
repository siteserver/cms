using System.Collections.Generic;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Shared
{
    public partial class EditorLayerWordController
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
