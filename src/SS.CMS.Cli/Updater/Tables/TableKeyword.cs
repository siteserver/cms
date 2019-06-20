using System.Collections.Generic;
using Newtonsoft.Json;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableKeyword
    {
        [JsonProperty("keywordID")]
        public long KeywordId { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("alternative")]
        public string Alternative { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }
    }

    public partial class TableKeyword
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_Keyword",
            "wcm_Keyword"
        };

        public static ConvertInfo Converter => new ConvertInfo
        {
            IsAbandon = true
        };
    }
}
