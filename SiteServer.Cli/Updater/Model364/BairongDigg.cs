using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongDigg
    {
        [JsonProperty("diggID")]
        public long DiggId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("relatedIdentity")]
        public long RelatedIdentity { get; set; }

        [JsonProperty("good")]
        public long Good { get; set; }

        [JsonProperty("bad")]
        public long Bad { get; set; }
    }

    public partial class BairongDigg
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
