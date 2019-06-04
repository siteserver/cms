using SS.CMS.Plugin.Data;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_RelatedField")]
    public class RelatedFieldInfo : Entity
    {
        [TableColumn]
        public string Title { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int TotalLevel { get; set; }

        [TableColumn]
        public string Prefixes { get; set; }

        [TableColumn]
        public string Suffixes { get; set; }
    }
}
