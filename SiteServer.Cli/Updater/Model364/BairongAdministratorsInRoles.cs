using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongAdministratorsInRoles
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }
    }

    public partial class BairongAdministratorsInRoles
    {
        public static readonly string NewTableName = DataProvider.AdministratorsInRolesDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.AdministratorsInRolesDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
