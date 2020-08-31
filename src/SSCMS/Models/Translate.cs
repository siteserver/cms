using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_Translate")]
    public class Translate : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int TargetSiteId { get; set; }

        [DataColumn]
        public int TargetChannelId { get; set; }

        [DataColumn]
        public TranslateType TranslateType { get; set; }

        [DataIgnore]
        public string Summary { get; set; }
    }
}