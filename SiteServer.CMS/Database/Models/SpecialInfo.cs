using System;
using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Special")]
    public class SpecialInfo : DynamicEntity
    {
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string Title { get; set; }

        [TableColumn]
        public string Url { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }
    }
}
