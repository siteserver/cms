using SS.CMS.Data;

namespace SS.CMS.Abstractions.Models
{
    [Table("siteserver_UserMenu")]
    public class UserMenuInfo : Entity
    {
        [TableColumn]
        public string SystemId { get; set; }

        [TableColumn]
        public string GroupIdCollection { get; set; }

        [TableColumn]
        public bool Disabled { get; set; }

        [TableColumn]
        public int ParentId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public string Text { get; set; }

        [TableColumn]
        public string IconClass { get; set; }

        [TableColumn]
        public string Href { get; set; }

        [TableColumn]
        public string Target { get; set; }
    }
}
