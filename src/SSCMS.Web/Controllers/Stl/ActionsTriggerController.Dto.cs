using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsTriggerController
    {
        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
            public int FileTemplateId { get; set; }
            public int SpecialId { get; set; }
            public bool IsRedirect { get; set; }
            public string ReturnUrl { get; set; }
        }
    }
}
