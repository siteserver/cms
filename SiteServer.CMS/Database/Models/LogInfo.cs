using System;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Log")]
    public class LogInfo : DynamicEntity, ILogInfo
    {
        public const string AdminLogin = "后台管理员登录";

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
