using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongUserBinding
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("bindingType")]
        public string BindingType { get; set; }

        [JsonProperty("bindingID")]
        public long BindingId { get; set; }

        [JsonProperty("bindingName")]
        public string BindingName { get; set; }
    }

    public partial class BairongUserBinding
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
