using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_DbCache")]
    public class DbCacheInfo : Entity
    {
        [TableColumn]
        public string CacheKey { get; set; }

        [TableColumn]
        public string CacheValue { get; set; }
    }
}
