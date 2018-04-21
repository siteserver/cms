using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverAd
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("adName")]
        public string AdName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("adType")]
        public string AdType { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("textWord")]
        public string TextWord { get; set; }

        [JsonProperty("textLink")]
        public string TextLink { get; set; }

        [JsonProperty("textColor")]
        public string TextColor { get; set; }

        [JsonProperty("textFontSize")]
        public long TextFontSize { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("imageLink")]
        public string ImageLink { get; set; }

        [JsonProperty("imageWidth")]
        public long ImageWidth { get; set; }

        [JsonProperty("imageHeight")]
        public long ImageHeight { get; set; }

        [JsonProperty("imageAlt")]
        public string ImageAlt { get; set; }

        [JsonProperty("isEnabled")]
        public string IsEnabled { get; set; }

        [JsonProperty("isDateLimited")]
        public string IsDateLimited { get; set; }

        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTimeOffset EndDate { get; set; }
    }

    public partial class SiteserverAd
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
