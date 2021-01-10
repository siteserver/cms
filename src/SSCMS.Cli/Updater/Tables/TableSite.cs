using Newtonsoft.Json;
using SSCMS.Services;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableSite
    {
        private readonly IDatabaseManager _databaseManager;

        public TableSite(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("publishmentSystemName")]
        public string PublishmentSystemName { get; set; }

        [JsonProperty("auxiliaryTableForContent")]
        public string AuxiliaryTableForContent { get; set; }

        [JsonProperty("auxiliaryTableForGovPublic")]
        public string AuxiliaryTableForGovPublic { get; set; }

        [JsonProperty("auxiliaryTableForGovInteract")]
        public string AuxiliaryTableForGovInteract { get; set; }

        [JsonProperty("auxiliaryTableForVote")]
        public string AuxiliaryTableForVote { get; set; }

        [JsonProperty("auxiliaryTableForJob")]
        public string AuxiliaryTableForJob { get; set; }

        [JsonProperty("isCheckContentUseLevel")]
        public string IsCheckContentUseLevel { get; set; }

        [JsonProperty("checkContentLevel")]
        public long CheckContentLevel { get; set; }

        [JsonProperty("publishmentSystemDir")]
        public string PublishmentSystemDir { get; set; }

        [JsonProperty("publishmentSystemUrl")]
        public string PublishmentSystemUrl { get; set; }

        [JsonProperty("isHeadquarters")]
        public string IsHeadquarters { get; set; }

        [JsonProperty("isRoot")]
        public string IsRoot { get; set; }

        [JsonProperty("parentPublishmentSystemID")]
        public long ParentPublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }
}
