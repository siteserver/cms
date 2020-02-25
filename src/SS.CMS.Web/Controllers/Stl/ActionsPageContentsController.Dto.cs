using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsPageContentsController
    {
        public class SubmitRequest : SiteRequest
        {
            public int PageChannelId { get; set; }
            public int TemplateId { get; set; }
            public int TotalNum { get; set; }
            public int PageCount { get; set; }
            public int CurrentPageIndex { get; set; }
            public string StlPageContentsElement { get; set; }
        }
    }
}
