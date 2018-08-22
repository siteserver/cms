using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model40
{
    public partial class TableSite
    {
        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("publishmentSystemName")]
        public string PublishmentSystemName { get; set; }

        [JsonProperty("publishmentSystemType")]
        public string PublishmentSystemType { get; set; }

        [JsonProperty("auxiliaryTableForContent")]
        public string AuxiliaryTableForContent { get; set; }

        [JsonProperty("auxiliaryTableForGoods")]
        public string AuxiliaryTableForGoods { get; set; }

        [JsonProperty("auxiliaryTableForBrand")]
        public string AuxiliaryTableForBrand { get; set; }

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

        [JsonProperty("groupSN")]
        public string GroupSn { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class TableSite
    {
        public const string OldTableName = "PublishmentSystem";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.SiteDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.SiteDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(SiteInfo.Id), nameof(PublishmentSystemId)},
                {nameof(SiteInfo.SiteDir), nameof(PublishmentSystemDir)},
                {nameof(SiteInfo.SiteName), nameof(PublishmentSystemName)},
                {nameof(SiteInfo.TableName), nameof(AuxiliaryTableForContent)},
                {nameof(SiteInfo.IsRoot), nameof(IsHeadquarters)},
                {nameof(SiteInfo.ParentId), nameof(ParentPublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
