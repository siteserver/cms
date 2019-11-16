using Datory;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_UserGroup")]
    public class UserGroup : Entity
    {
        [DataColumn]
        public string GroupName { get; set; }

        [DataColumn]
        public string AdminName { get; set; }
    }
}
