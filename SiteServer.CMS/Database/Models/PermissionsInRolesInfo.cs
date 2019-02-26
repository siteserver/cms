using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_PermissionsInRoles")]
    public class PermissionsInRolesInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string RoleName { get; set; }

        [Text]
        private string GeneralPermissions { get; set; }

        [Computed]
        public List<string> GeneralPermissionList
        {
            get => TranslateUtils.StringCollectionToStringList(GeneralPermissions);
            set => GeneralPermissions = TranslateUtils.ObjectCollectionToString(value);
        }
    }
}
