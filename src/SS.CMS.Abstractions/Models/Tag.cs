using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Tag")]
    public class Tag : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn(Text = true)]
        public string ContentIdCollection { get; set; }

        [DataColumn]
        public string Value { get; set; }

        [DataColumn]
        public int UseNum { get; set; }

        [DataColumn]
        public int Level { get; set; }
    }
}
