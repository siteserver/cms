using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Tag")]
    public class TagInfo : DynamicEntity
    {
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string ContentIdCollection { get; set; }

        [TableColumn]
        public string Tag { get; set; }

        [TableColumn]
        public int UseNum { get; set; }

        [TableColumn]
        public int Level { get; set; }
    }
}
