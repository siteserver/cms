using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_WxAccount")]
    public class WxAccount : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string MpAppId { get; set; }

        [DataColumn]
        public string MpAppSecret { get; set; }

        [DataColumn]
        public string MpToken { get; set; }

        [DataColumn]
        public bool MpIsEncrypt { get; set; }

        [DataColumn]
        public string MpEncodingAESKey { get; set; }

        [DataColumn]
        public int MpReplyBeAddedMessageId { get; set; }

        [DataColumn]
        public int MpReplyAutoMessageId { get; set; }

        [DataColumn]
        public bool TenPayConnected { get; set; }

        [DataColumn]
        public string TenPayAppId { get; set; }

        [DataColumn]
        public string TenPayAppSecret { get; set; }

        [DataColumn]
        public string TenPayMchId { get; set; }

        [DataColumn]
        public string TenPayKey { get; set; }

        [DataColumn]
        public string TenPayAuthorizeUrl { get; set; }

        [DataColumn]
        public string TenPayNotifyUrl { get; set; }
    }
}
