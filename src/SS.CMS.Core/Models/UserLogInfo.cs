using System;
using SS.CMS.Data;
using SS.CMS.Plugin;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_UserLog")]
    public class UserLogInfo : Entity, ILogInfo
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
