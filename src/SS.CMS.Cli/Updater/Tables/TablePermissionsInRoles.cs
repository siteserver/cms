using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TablePermissionsInRoles
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("generalPermissions")]
        public string GeneralPermissions { get; set; }
    }

    public partial class TablePermissionsInRoles
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "bairong_PermissionsInRoles",
            "siteserver_PermissionsInRoles"
        };

        public static ConvertInfo GetConverter(IPermissionRepository permissionRepository) => new ConvertInfo
        {
            NewTableName = permissionRepository.TableName,
            NewColumns = permissionRepository.TableColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(PermissionInfo.AppPermissions), nameof(GeneralPermissions)},
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
