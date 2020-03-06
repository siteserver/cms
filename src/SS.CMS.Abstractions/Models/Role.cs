using Datory;
using Datory.Annotations;

namespace SS.CMS.Abstractions
{
    [DataTable("siteserver_Role")]
    public class Role : Entity
    {
        [DataColumn]
        public string RoleName  { get; set; }

        [DataColumn]
        public string CreatorUserName { get; set; }

        [DataColumn]
        public string Description { get; set; }
    }
}
