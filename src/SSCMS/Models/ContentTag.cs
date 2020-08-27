using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_ContentTag")]
    public class ContentTag : Entity
    {
        [DataColumn]
        public string TagName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn(Text = true)]
        public List<int> ContentIds { get; set; }

        [DataColumn]
        public int UseNum { get; set; }

        public int Level { get; set; }
    }
}
