using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_UserMenu")]
    public class UserMenu : Entity
    {
        [DataColumn]
        public string SystemId { get; set; }

        [DataColumn(Text = true)]
        public string GroupIdCollection { get; set; }

        [DataColumn]
        public bool IsDisabled { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string Text { get; set; }

        [DataColumn]
        public string IconClass { get; set; }

        [DataColumn]
        public string Href { get; set; }

        [DataColumn]
        public string Target { get; set; }
    }
}
