using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_Permission")]
    public class PermissionInfo : Entity
    {
        [TableColumn]
        public string RoleName { get; set; }

        [TableColumn(Text = true)]
        public string AppPermissions { get; set; }

        [TableColumn]
        public int SiteId { get; set; }
        [TableColumn(Text = true)]
        public string SitePermissions { get; set; }

        [TableColumn(Text = true)]
        public int ChannelId { get; set; }

        [TableColumn(Text = true)]
        public string ChannelPermissions { get; set; }
    }
}
