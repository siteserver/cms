using Datory;

namespace SiteServer.CMS.Model
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
