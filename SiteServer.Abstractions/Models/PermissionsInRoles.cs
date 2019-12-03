using System;
using System.Collections.Generic;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_PermissionsInRoles")]
    public class PermissionsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn(Text = true)]
        public string GeneralPermissions { get; set; }

        public List<string> GeneralPermissionList
        {
            get => StringUtils.GetStringList(GeneralPermissions);
            set => GeneralPermissions = StringUtils.Join(value);
        }
    }
}