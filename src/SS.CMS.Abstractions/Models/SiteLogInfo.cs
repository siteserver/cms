using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_SiteLog")]
    public class SiteLogInfo : Entity
    {
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int ChannelId { get; set; }

        [TableColumn]
        public int ContentId { get; set; }

        [TableColumn]
        public string UserName { get; set; }

        [TableColumn]
        public string IpAddress { get; set; }

        [TableColumn]
        public string Action { get; set; }

        [TableColumn]
        public string Summary { get; set; }
    }
}
