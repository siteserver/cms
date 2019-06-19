using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_Log")]
    public class LogInfo : Entity
    {
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
