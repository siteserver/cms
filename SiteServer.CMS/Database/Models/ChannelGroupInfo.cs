using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_ChannelGroup")]
    public class ChannelGroupInfo : DynamicEntity
    {
        [TableColumn]
        public string GroupName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn(Text = true)]
        public string Description { get; set; }
    }
}
