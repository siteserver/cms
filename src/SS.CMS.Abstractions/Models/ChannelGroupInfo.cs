using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_ChannelGroup")]
    public class ChannelGroupInfo : Entity
    {
        [TableColumn]
        public string GroupName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn(Length = 2000)]
        public string Description { get; set; }
    }
}
