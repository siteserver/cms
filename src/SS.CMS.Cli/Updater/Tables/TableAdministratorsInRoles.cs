using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Repositories;

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
        public static readonly List<string> OldTableNames = new List<string>
        {
            "bairong_AdministratorsInRoles",
            "siteserver_AdministratorsInRoles"
        };

        public static ConvertInfo GetConverter(IUserRoleRepository userRoleRepository)
        {
            return new ConvertInfo
            {
                NewTableName = userRoleRepository.TableName,
                NewColumns = userRoleRepository.TableColumns,
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        private static readonly Dictionary<string, string> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
