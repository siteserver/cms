using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_WxReplyRule")]
    public class WxReplyRule : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string RuleName { get; set; }

        [DataColumn]
        public bool Random { get; set; }

        [DataIgnore]
        public List<WxReplyKeyword> Keywords { get; set; }

        [DataIgnore]
        public List<WxReplyMessage> Messages { get; set; }
    }
}