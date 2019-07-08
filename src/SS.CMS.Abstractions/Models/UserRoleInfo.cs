using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [Table("siteserver_UserRole")]
    public class UserRoleInfo : Entity
    {
        [TableColumn]
        public int UserId { get; set; }

        [TableColumn]
        public int RoleId { get; set; }
    }
}
