using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_UserLog")]
    public class UserLogInfo : Entity
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
