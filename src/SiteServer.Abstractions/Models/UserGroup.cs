using Datory;
using Datory.Annotations;

namespace SiteServer.Abstractions
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
