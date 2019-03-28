using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_SitePermissions")]
    public class SitePermissionsInfo : DynamicEntity
    {
        [TableColumn]
        public string RoleName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn(Text = true)]
        public string ChannelIdCollection { get; set; }

        [TableColumn(Text = true)]
        public string ChannelPermissions { get; set; }

        [TableColumn(Text = true)]
        public string WebsitePermissions { get; set; }
    }
}
