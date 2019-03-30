using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Tables.GovInteract
{
    public partial class TableGovInteractChannel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("applyStyleID")]
        public long ApplyStyleId { get; set; }

        [JsonProperty("queryStyleID")]
        public long QueryStyleId { get; set; }

        [JsonProperty("departmentIDCollection")]
        public string DepartmentIdCollection { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }

    public partial class TableGovInteractChannel
    {
        public const string OldTableName = "wcm_GovInteractChannel";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = "ss_govinteract_channel";

        private static readonly List<DatoryColumn> NewColumns = new List<DatoryColumn>
        {
            new DatoryColumn
            {
                AttributeName = "Id",
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new DatoryColumn
            {
                AttributeName = "ChannelId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "SiteId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "ApplyStyleId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "QueryStyleId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "DepartmentIdCollection",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "Summary",
                DataType = DataType.VarChar
            }
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {"ChannelId", nameof(NodeId)},
                {"SiteId", nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
