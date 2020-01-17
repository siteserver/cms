using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;


namespace SiteServer.Abstractions
{
    [DataTable("siteserver_PermissionsInRoles")]
    public class PermissionsInRoles : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn(Text = true)]
        [JsonIgnore]
        private string GeneralPermissions { get; set; }

        public List<string> GeneralPermissionList
        {
            get => StringUtils.GetStringList(GeneralPermissions);
            set => GeneralPermissions = StringUtils.Join(value);
        }
    }
}