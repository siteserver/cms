using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverAdvertisement
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("advertisementName")]
        public string AdvertisementName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("advertisementType")]
        public string AdvertisementType { get; set; }

        [JsonProperty("navigationUrl")]
        public string NavigationUrl { get; set; }

        [JsonProperty("isDateLimited")]
        public string IsDateLimited { get; set; }

        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTimeOffset EndDate { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("nodeIDCollectionToChannel")]
        public string NodeIdCollectionToChannel { get; set; }

        [JsonProperty("nodeIDCollectionToContent")]
        public string NodeIdCollectionToContent { get; set; }

        [JsonProperty("fileTemplateIDCollection")]
        public string FileTemplateIdCollection { get; set; }

        [JsonProperty("settings")]
        public string Settings { get; set; }
    }

    public partial class SiteserverAdvertisement
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
