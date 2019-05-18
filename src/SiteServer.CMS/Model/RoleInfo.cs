using System;
using Datory;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [Table("siteserver_Role")]
    public class RoleInfo : Entity
    {
        [TableColumn]
        public string RoleName { get; set; }

        [TableColumn]
        public string CreatorUserName { get; set; }

        [TableColumn]
        public string Description { get; set; }
    }
}
