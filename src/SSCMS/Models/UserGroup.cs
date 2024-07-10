using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_UserGroup")]
    public class UserGroup : Entity
    {
        [DataColumn]
        public string GroupName { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string AdminName { get; set; }

        [DataColumn]
        public string Description { get; set; }

        public bool IsDefault { get; set; }

        public bool IsManager { get; set; }
    }
}
