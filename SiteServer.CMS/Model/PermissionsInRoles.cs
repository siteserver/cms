using System.Collections.Generic;
using Datory;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_PermissionsInRoles")]
    public class PermissionsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn(Text = true)]
        public string GeneralPermissions { get; set; }

        public List<string> GeneralPermissionList
        {
            get => TranslateUtils.StringCollectionToStringList(GeneralPermissions);
            set => GeneralPermissions = string.Join(",", value);
        }
    }
}