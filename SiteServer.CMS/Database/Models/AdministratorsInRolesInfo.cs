using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_AdministratorsInRoles")]
    public class AdministratorsInRolesInfo : DynamicEntity
    {
        [TableColumn]
        public string RoleName { get; set; }

        [TableColumn]
        public string UserName { get; set; }
    }
}
