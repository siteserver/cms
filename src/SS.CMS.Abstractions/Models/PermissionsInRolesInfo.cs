using SS.CMS.Data;

namespace SS.CMS.Abstractions.Models
{
    [Table("siteserver_PermissionsInRoles")]
    public class PermissionsInRolesInfo : Entity
    {
        [TableColumn]
        public string RoleName { get; set; }

        [TableColumn(Text = true)]
        public string GeneralPermissions { get; set; }
    }
}
