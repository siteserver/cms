using SSCMS.Dto.Request;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class RedirectController
    {
        public class SubmitRequest : ChannelRequest
        {
            public int ContentId { get; set; }
            public int FileTemplateId { get; set; }
            public int SpecialId { get; set; }
            public bool IsLocal { get; set; }
        }
    }
}
