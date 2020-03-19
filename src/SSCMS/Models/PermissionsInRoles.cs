using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS
{
    [DataTable("siteserver_PermissionsInRoles")]
    public class PermissionsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn(Text = true)]
        public List<string> GeneralPermissions { get; set; }
    }
}