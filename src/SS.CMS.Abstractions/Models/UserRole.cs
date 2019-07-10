using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_UserRole")]
    public class UserRole : Entity
    {
        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public int RoleId { get; set; }
    }
}
