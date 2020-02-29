using Newtonsoft.Json;

namespace SS.CMS.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicCategoryClass
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("classCode")]
        public string ClassCode { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("isSystem")]
        public string IsSystem { get; set; }

        [JsonProperty("isEnabled")]
        public string IsEnabled { get; set; }

        [JsonProperty("contentAttributeName")]
        public string ContentAttributeName { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
