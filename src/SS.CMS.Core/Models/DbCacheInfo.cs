using System;
using SS.CMS.Data;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_DbCache")]
    public class DbCacheInfo : Entity
    {
        [TableColumn]
        public string CacheKey { get; set; }

        [TableColumn]
        public string CacheValue { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }
    }
}
