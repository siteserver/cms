using Datory;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_UserGroup")]
    public class UserGroupInfo : Entity
    {
        [TableColumn]
        public string GroupName { get; set; }

        [TableColumn]
        public string AdminName { get; set; }
    }
}
