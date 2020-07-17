using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_OpenAccount")]
    public class OpenAccount : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public bool WxConnected { get; set; }

        [DataColumn]
        public string WxAppId { get; set; }

        [DataColumn]
        public string WxAppSecret { get; set; }

        [DataColumn]
        public string WxUrl { get; set; }

        [DataColumn]
        public string WxToken { get; set; }

        [DataColumn]
        public bool WxIsEncrypt { get; set; }

        [DataColumn]
        public string WxEncodingAESKey { get; set; }

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
