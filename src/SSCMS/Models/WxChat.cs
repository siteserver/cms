using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_WxChat")]
    public class WxChat : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string OpenId { get; set; }

        [DataColumn]
        public bool IsReply { get; set; }

        [DataColumn]
        public int ReplyMessageId { get; set; }

        [DataColumn]
        public string Text { get; set; }
    }
}
