using SS.CMS.Data;

namespace SS.CMS.Models
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
