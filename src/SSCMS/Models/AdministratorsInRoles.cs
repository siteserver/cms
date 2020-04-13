using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_AdministratorsInRoles")]
    public class AdministratorsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public string UserName { get; set; }
	}
}
