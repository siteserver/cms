using System;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_UserLog")]
    public class UserLogInfo : DynamicEntity, ILogInfo
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
