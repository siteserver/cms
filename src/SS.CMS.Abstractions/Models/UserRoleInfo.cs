using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [Table("siteserver_UserRole")]
    public class UserRoleInfo : Entity
    {
        [TableColumn]
        public string UserName { get; set; }

        [TableColumn]
        public string RoleName { get; set; }
    }
}
