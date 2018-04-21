using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongPermissionsInRoles
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("generalPermissions")]
        public string GeneralPermissions { get; set; }
    }

    public partial class BairongPermissionsInRoles
    {
        public static readonly string NewTableName = DataProvider.PermissionsInRolesDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.PermissionsInRolesDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
