using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SS.CMS.Framework;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableAdministratorsInRoles
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }
    }

    public partial class TableAdministratorsInRoles
    {
        public const string OldTableName = "bairong_AdministratorsInRoles";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.AdministratorsInRolesRepository.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.AdministratorsInRolesRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
