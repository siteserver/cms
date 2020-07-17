using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Common.Library
{
    public partial class LayerWordController
    {
        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public bool IsClearFormat { get; set; }
            public bool IsFirstLineIndent { get; set; }
            public bool IsClearFontSize { get; set; }
            public bool IsClearFontFamily { get; set; }
            public bool IsClearImages { get; set; }
            public List<NameTitle> Files { get; set; }
        }

        public class NameTitle
        {
            public string FileName { get; set; }
            public string Title { get; set; }
        }
    }
}
