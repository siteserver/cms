using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_PermissionsInRoles")]
    public class PermissionsInRolesInfo : Entity
    {
        [TableColumn]
        public string RoleName { get; set; }

        [TableColumn(Text = true)]
        private string GeneralPermissions { get; set; }

        public List<string> GeneralPermissionList
        {
            get => TranslateUtils.StringCollectionToStringList(GeneralPermissions);
            set => GeneralPermissions = TranslateUtils.ObjectCollectionToString(value);
        }
    }
}
