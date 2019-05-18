using System;
using Datory;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
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
