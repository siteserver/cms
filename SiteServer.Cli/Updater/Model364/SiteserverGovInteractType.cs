using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverGovInteractType
    {
        [JsonProperty("typeID")]
        public long TypeId { get; set; }

        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }
    }

    public partial class SiteserverGovInteractType
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
