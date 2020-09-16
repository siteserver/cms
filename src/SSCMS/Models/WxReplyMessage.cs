using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_WxReplyMessage")]
    public class WxReplyMessage : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int RuleId { get; set; }

        [DataColumn]
        public MaterialType MaterialType { get; set; }

        [DataColumn]
        public int MaterialId { get; set; }

        [DataColumn]
        public string Text { get; set; }

        [DataIgnore]
        public string MediaId { get; set; }

        [DataIgnore]
        public List<MaterialMessageItem> Items { get; set; }

        [DataIgnore]
        public MaterialImage Image { get; set; }

        [DataIgnore]
        public MaterialAudio Audio { get; set; }

        [DataIgnore]
        public MaterialVideo Video { get; set; }
    }
}