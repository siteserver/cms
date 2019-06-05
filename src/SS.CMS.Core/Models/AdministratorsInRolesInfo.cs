using SS.CMS.Data;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_AdministratorsInRoles")]
    public class AdministratorsInRolesInfo : Entity
    {
        [TableColumn]
        public string RoleName { get; set; }

        [TableColumn]
        public string UserName { get; set; }
    }
}
