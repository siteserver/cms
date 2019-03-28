using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_UserGroup")]
    public class UserGroupInfo : DynamicEntity
    {
        [TableColumn]
        public string GroupName { get; set; }

        [TableColumn]
        public string AdminName { get; set; }
    }
}
