using SiteServer.CMS.Database.Wrapper;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_UserMenu")]
    public class UserMenuInfo : DynamicEntity
    {
        [TableColumn]
        public string SystemId { get; set; }

        [TableColumn]
        public string GroupIdCollection { get; set; }

        [TableColumn]
        private string IsDisabled { get; set; }

        public bool Disabled
        {
            get => IsDisabled == "True";
            set => IsDisabled = value.ToString();
        }

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
