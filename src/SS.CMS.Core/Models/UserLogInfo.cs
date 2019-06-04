using System;
using SS.CMS.Plugin;
using SS.CMS.Plugin.Data;

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
