using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableSite
    {
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

        [JsonProperty("parentPublishmentSystemID")]
        public long ParentPublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }

    public partial class TableSite
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_PublishmentSystem",
            "wcm_PublishmentSystem"
        };

        public static ConvertInfo GetConverter(ISiteRepository siteRepository) => new ConvertInfo
        {
            NewTableName = siteRepository.TableName,
            NewColumns = siteRepository.TableColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(Site.Id), nameof(PublishmentSystemId)},
                {nameof(Site.SiteName), nameof(PublishmentSystemName)},
                {nameof(Site.TableName), nameof(AuxiliaryTableForContent)},
                {nameof(Site.SiteDir), nameof(PublishmentSystemDir)},
                {nameof(Site.IsRoot), nameof(IsHeadquarters)},
                {nameof(Site.ParentId), nameof(ParentPublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
