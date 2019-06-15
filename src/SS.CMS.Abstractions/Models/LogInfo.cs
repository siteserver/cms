using System;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Models
{
    [Table("siteserver_Log")]
    public class LogInfo : Entity
    {
        [TableColumn]
        public string UserName { get; set; }

        [TableColumn]
        public string IpAddress { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }

        [TableColumn]
        public string Action { get; set; }

        [TableColumn]
        public string Summary { get; set; }
    }
}
