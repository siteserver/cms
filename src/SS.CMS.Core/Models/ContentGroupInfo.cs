using SS.CMS.Data;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_ContentGroup")]
    public class ContentGroupInfo : Entity
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
