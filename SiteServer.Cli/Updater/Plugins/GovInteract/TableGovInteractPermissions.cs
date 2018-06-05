using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovInteract
{
    public partial class TableGovInteractPermissions
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("permissions")]
        public string Permissions { get; set; }
    }

    public partial class TableGovInteractPermissions
    {
        public const string OldTableName = "GovInteractPermissions";

        public static readonly string NewTableName = "ss_govinteract_permissions";

        public static readonly List<TableColumnInfo> NewColumns = new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = "UserName",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "ChannelId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "Permissions",
                DataType = DataType.Text
            }
        };

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {"ChannelId", nameof(NodeId)}
            };
    }
}
