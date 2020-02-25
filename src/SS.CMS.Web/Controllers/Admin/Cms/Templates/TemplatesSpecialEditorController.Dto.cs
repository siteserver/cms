using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialEditorController
    {
        public class GetRequest : SiteRequest
        {
            public int SpecialId { get; set; }
        }

        public class GetResult
        {
            public Special Special { get; set; }
            public string SpecialUrl { get; set; }
            public string Html { get; set; }
        }
    }
}