using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_UserRole")]
    public class UserRoleInfo : Entity
    {
        [TableColumn]
        public string UserName { get; set; }

        [TableColumn]
        public string RoleName { get; set; }
    }
}
