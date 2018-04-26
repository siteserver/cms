using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverGatherFileRule
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("gatherRuleName")]
        public string GatherRuleName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("gatherUrl")]
        public string GatherUrl { get; set; }

        [JsonProperty("charset")]
        public string Charset { get; set; }

        [JsonProperty("lastGatherDate")]
        public DateTimeOffset LastGatherDate { get; set; }

        [JsonProperty("isToFile")]
        public string IsToFile { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("isSaveRelatedFiles")]
        public string IsSaveRelatedFiles { get; set; }

        [JsonProperty("isRemoveScripts")]
        public string IsRemoveScripts { get; set; }

        [JsonProperty("styleDirectoryPath")]
        public string StyleDirectoryPath { get; set; }

        [JsonProperty("scriptDirectoryPath")]
        public string ScriptDirectoryPath { get; set; }

        [JsonProperty("imageDirectoryPath")]
        public string ImageDirectoryPath { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("isSaveImage")]
        public string IsSaveImage { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("contentExclude")]
        public string ContentExclude { get; set; }

        [JsonProperty("contentHtmlClearCollection")]
        public string ContentHtmlClearCollection { get; set; }

        [JsonProperty("contentHtmlClearTagCollection")]
        public string ContentHtmlClearTagCollection { get; set; }

        [JsonProperty("contentTitleStart")]
        public string ContentTitleStart { get; set; }

        [JsonProperty("contentTitleEnd")]
        public string ContentTitleEnd { get; set; }

        [JsonProperty("contentContentStart")]
        public string ContentContentStart { get; set; }

        [JsonProperty("contentContentEnd")]
        public string ContentContentEnd { get; set; }

        [JsonProperty("contentAttributes")]
        public string ContentAttributes { get; set; }

        [JsonProperty("contentAttributesXML")]
        public string ContentAttributesXml { get; set; }
    }

    public partial class SiteserverGatherFileRule
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
