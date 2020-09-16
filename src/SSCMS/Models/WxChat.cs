using Datory;
using Datory.Annotations;
using SSCMS.Enums;

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
        public bool IsStar { get; set; }

        [DataColumn]
        public MaterialType MaterialType { get; set; }

        [DataColumn]
        public int MaterialId { get; set; }

        [DataColumn]
        public string Text { get; set; }
    }
}
