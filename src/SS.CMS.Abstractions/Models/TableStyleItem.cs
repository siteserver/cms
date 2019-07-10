using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_TableStyleItem")]
    public class TableStyleItem : Entity
    {
        [DataColumn]
        public int TableStyleId { get; set; }

        [DataColumn]
        public string ItemTitle { get; set; }

        [DataColumn]
        public string ItemValue { get; set; }

        [DataColumn]
        public bool IsSelected { get; set; }
    }
}
