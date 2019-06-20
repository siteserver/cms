using System.Collections.Generic;
using Newtonsoft.Json;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableTemplateMatch
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("ruleID")]
        public long RuleId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("channelTemplateID")]
        public long ChannelTemplateId { get; set; }

        [JsonProperty("contentTemplateID")]
        public long ContentTemplateId { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("channelFilePathRule")]
        public string ChannelFilePathRule { get; set; }

        [JsonProperty("contentFilePathRule")]
        public string ContentFilePathRule { get; set; }
    }

    public partial class TableTemplateMatch
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_TemplateMatch",
            "wcm_TemplateMatch"
        };

        public static ConvertInfo Converter => new ConvertInfo
        {
            IsAbandon = true
        };
    }
}
